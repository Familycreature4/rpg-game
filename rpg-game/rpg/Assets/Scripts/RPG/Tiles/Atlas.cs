using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    //https://docs.unity3d.com/ScriptReference/Graphics.CopyTexture.html
    //https://en.m.wikipedia.org/wiki/Rectangle_packing
    /// <summary>
    /// Collection of tile textures mapped to a single image
    /// </summary>
    public class Atlas
    {
        public enum Type
        {
            Diffuse = 1 << 0,
            Specular = 1 << 1,
            Normal = 1 << 2
        }
        public Type load = Type.Diffuse | Type.Normal | Type.Specular;
        public Texture2D diffuseTexture;
        public Texture2D specularTexture;
        public Texture2D normalTexture;
        Dictionary<TileMaterial, Bounds> textureBounds;
        public Bounds GetBounds(TileMaterial mat)
        {
            if (textureBounds == null)
                return default;

            if (textureBounds.TryGetValue(mat, out Bounds bounds))
                return bounds;

            return new Bounds { min = Vector2.zero, max = Vector2.one };
        }
        public void GenerateTextureAtlas()
        {
            #region Atlas Creation

            textureBounds = new Dictionary<TileMaterial, Bounds>();
            List<AtlasImage> textures = new List<AtlasImage>();

            Vector2Int offset = Vector2Int.zero;
            Vector2Int atlasSize = Vector2Int.zero;
            // Map each TileMaterial to a position
            foreach (TileMaterial material in Resources.LoadAll<TileMaterial>("Tiles/Materials"))
            {
                AtlasImage atlasImage = new AtlasImage { tileMaterial = material, offset = offset };
                textures.Add(atlasImage);
                textureBounds.Add(material, new Bounds { min = offset, max = offset + new Vector2Int(atlasImage.Width, atlasImage.Height) });

                // Adjust offset for next image
                offset.x += atlasImage.Width;
                atlasSize.x += atlasImage.Width;

                if (atlasSize.y < atlasImage.Height)
                    atlasSize.y = atlasImage.Height;

            }

            // Round each dimension to the next highest power of two
            for (int i = 0; i < 2; i++)
                atlasSize[i] = (int)Mathf.Pow(2.0f, Mathf.CeilToInt(Mathf.Log(atlasSize[i], 2.0f)));

            #endregion

            #region Texture Transfering
            Texture2D CreateTexture(Type type)
            {
                Texture2D texture = new Texture2D(atlasSize.x, atlasSize.y, UnityEngine.TextureFormat.RGBA32, 8, true);

                foreach (AtlasImage image in textures)
                {
                    TileMaterial tileMaterial = image.tileMaterial;
                    System.Func<int, int, Color32> pixelGetter = null;
                    Texture2D transferTexture = null;

                    // Determine how each map of the given TileMaterial will be transferred

                    #region Pixel Getters

                    switch (type)
                    {
                        case Type.Diffuse:
                            if (tileMaterial.diffuse == null)
                            {
                                pixelGetter = delegate (int x, int y) { return new Color32(255, 0, 128, 255); };
                            }
                            else
                            {
                                //Color32[] colors = tileMaterial.diffuse.GetPixels32();
                                //pixelGetter = delegate (int x, int y) { return colors[y * tileMaterial.diffuse.width + x]; };
                                transferTexture = tileMaterial.diffuse;
                            }
                            break;
                        case Type.Normal:
                            if (tileMaterial.normal == null)
                            {
                                pixelGetter = delegate (int x, int y) { return new Color32(128, 128, 255, 255); };
                            }
                            else
                            {
                                //Color32[] colors = tileMaterial.normal.GetPixels32();
                                //pixelGetter = delegate (int x, int y) { return colors[y * tileMaterial.normal.width + x]; };
                                transferTexture = tileMaterial.normal;
                            }
                            break;
                        case Type.Specular:
                            Func<int, int, byte> roughnessGetter = null;
                            if (tileMaterial.roughness == null)
                            {
                                roughnessGetter = delegate (int x, int y) { return (byte)(tileMaterial.roughnessFallback * 255); };
                            }
                            else
                            {
                                Color32[] colors = tileMaterial.roughness.GetPixels32();
                                roughnessGetter = delegate (int x, int y) { return colors[y * tileMaterial.roughness.width + x].r; };
                            }
                            if (tileMaterial.specular == null)
                            {
                                pixelGetter = delegate (int x, int y) {
                                    byte spec = tileMaterial.deriveSpecularFromRoughness ? (byte)(255 - roughnessGetter(x, y)) : (byte)(tileMaterial.specularFallback * 255);
                                    return new Color32(spec, spec, spec, (byte)(255 - roughnessGetter(x, y))); 
                                };
                            }
                            else
                            {
                                Color32[] colors = tileMaterial.specular.GetPixels32();
                                pixelGetter = delegate (int x, int y) {
                                    Color32 color = colors[y * tileMaterial.specular.width + x];
                                    color.a = (byte)(255 - roughnessGetter(x, y));
                                    return color;
                                };
                            }
                            break;
                    }

                    #endregion

                    // Transfer pixels

                    // Create color array

                    if (pixelGetter != null)
                    {
                        Color32[] colorData = new Color32[image.Width * image.Height];
                        for (int x = 0; x < image.Width; x++)
                        {
                            for (int y = 0; y < image.Height; y++)
                            {
                                colorData[y * image.Width + x] = pixelGetter(x, y);
                            }
                        }

                        texture.SetPixels32(image.offset.x, image.offset.y, image.Width, image.Height, colorData, 0);
                    }
                    else if (transferTexture != null)  // Use Graphics API
                    {
                        Graphics.CopyTexture(transferTexture, 0, 0, 0, 0, image.Width, image.Height, texture, 0, 0, image.offset.x, image.offset.y);
                    }
                }

                texture.filterMode = FilterMode.Point;
                //texture.Compress(false);
                texture.Apply(true, true);

                return texture;
            }

            //System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

            //watch.Start();
            if (load.HasFlag(Type.Diffuse))
                diffuseTexture = CreateTexture(Type.Diffuse);
            //watch.Stop();
            //Debug.Log($"Took {watch.ElapsedMilliseconds}ms to create diffuse texture 💀");
            //watch.Restart();
            if (load.HasFlag(Type.Specular))
                specularTexture = CreateTexture(Type.Specular);
            //watch.Stop();
            //Debug.Log($"Took {watch.ElapsedMilliseconds}ms to create specular/roughness texture 💀");
            //watch.Restart();
            if (load.HasFlag(Type.Normal))
                normalTexture = CreateTexture(Type.Normal);
            //watch.Stop();
            //Debug.Log($"Took {watch.ElapsedMilliseconds}ms to create normal texture 💀");

            #endregion

            UnityEngine.Material mat = Resources.Load<UnityEngine.Material>("Materials/Atlas");
            if (load.HasFlag(Type.Diffuse))
                mat.SetTexture("_MainTex", diffuseTexture);
            if (load.HasFlag(Type.Specular))
                mat.SetTexture("_SpecGlossMap", specularTexture);
            if (load.HasFlag(Type.Normal))
                mat.SetTexture("_BumpMap", normalTexture);

            mat.shader = mat.shader;
        }
        public struct Bounds
        {
            public Vector2 min;
            public Vector2 max;
        }
        public class AtlasImage
        {
            Texture2D GetMainTexture
            {
                get
                {
                    if (tileMaterial.diffuse != null)
                        return tileMaterial.diffuse;
                    if (tileMaterial.normal != null)
                        return tileMaterial.normal;
                    if (tileMaterial.specular != null)
                        return tileMaterial.specular;
                    if (tileMaterial.roughness != null)
                        return tileMaterial.roughness;
                    return null;
                }
            }
            public int Height => GetMainTexture.height;
            public int Width => GetMainTexture.width;

            public TileMaterial tileMaterial;
            public Vector2Int offset;
        }
    }
}