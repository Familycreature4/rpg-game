using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public static class Atlas
    {
        public static Texture2D atlas;
        static Dictionary<Texture2D, Bounds> textureBounds;
        public static Bounds GetBounds(Texture2D texture)
        {
            if (textureBounds.TryGetValue(texture, out Bounds bounds))
                return bounds;

            return new Bounds { min = Vector2.zero, max = Vector2.one };
        }
        public static void GenerateTextureAtlas()
        {
            textureBounds = new Dictionary<Texture2D, Bounds>();
            List<AtlasImage> textures = new List<AtlasImage>();
            Vector2Int offset = Vector2Int.zero;
            Vector2Int atlasSize = Vector2Int.zero;
            // Link texture asset with bounds for uvs in atlas
            foreach (TileMaterial material in Resources.LoadAll<TileMaterial>("Tiles/Materials"))
            {
                AtlasImage atlasImage = new AtlasImage { texture = (Texture2D)material.texture, offset = offset };
                textures.Add(atlasImage);
                textureBounds.Add((Texture2D)material.texture, new Bounds { min = offset, max = offset + new Vector2Int(material.texture.width, material.texture.height) } );

                // Adjust offset for next image
                offset.x += material.texture.width;
                atlasSize.x += material.texture.width;

                if (atlasSize.y < material.texture.height)
                    atlasSize.y = material.texture.height;

            }

            // Round each dimension to the next highest power of two
            for (int i = 0; i < 2; i++)
                atlasSize[i] = (int)Mathf.Pow(2.0f, Mathf.CeilToInt(Mathf.Log(atlasSize[i], 2.0f)));

            atlas = new Texture2D(atlasSize.x, atlasSize.y,  UnityEngine.Experimental.Rendering.GraphicsFormat.R32G32B32A32_SFloat, 8, UnityEngine.Experimental.Rendering.TextureCreationFlags.MipChain);

            foreach (AtlasImage image in textures)
            {
                // Copy pixels from each image to atlas
                for (int x = 0; x < image.texture.width; x++)
                {
                    for (int y = 0; y < image.texture.height; y++)
                    {
                        Color pixel = image.texture.GetPixel(x, y);
                        atlas.SetPixel(x + image.offset.x, y + image.offset.y, pixel);
                    }
                }
            }

            atlas.filterMode = FilterMode.Point;
            atlas.Apply(true);
            atlas.Compress(false);

            Resources.Load<Material>("Materials/Atlas").mainTexture = atlas;
        }

        public struct Bounds
        {
            public Vector2 UvMin => new Vector2(min.x / atlas.width, min.y / atlas.height);
            public Vector2 UvMax => new Vector2(max.x / atlas.width, max.y / atlas.height);
            public Vector2 UvSize => new Vector2(Mathf.Abs(UvMax.x - UvMin.x), Mathf.Abs(UvMax.y - UvMin.y));
            public Vector2 min;
            public Vector2 max;
        }
        public class AtlasImage
        {
            public Texture2D texture;
            public Vector2Int offset;
        }
    }
}