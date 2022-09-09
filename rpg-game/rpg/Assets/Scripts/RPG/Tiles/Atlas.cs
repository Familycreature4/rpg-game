using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public static class Atlas
    {
        public static void GenerateTextureAtlas()
        {
            //// Foreach material



            //// Get unique textures and their materials that use it
            //Dictionary<Texture2D, List<int>> _textures = new Dictionary<Texture2D, List<int>>();
            //for (int i = 0; i < TileMaterial.materials.Count; i++)
            //{
            //    Material _material = materials[i];
            //    if (_material.texture != null)
            //    {
            //        if (_textures.ContainsKey(_material.texture) == false)
            //        {
            //            _textures.Add(_material.texture, new List<int>() { i });
            //        }
            //        else
            //        {
            //            _textures[_material.texture].Add(i);
            //        }
            //    }
            //}

            //// Texture2D.PackTextures...

            //// Round up to nearest power of 2
            //int _horizontalSize = (int)Mathf.Pow(2, Mathf.Ceil(Mathf.Log(textureSize * _textures.Count, 2)));
            //atlas = new Texture2D(_horizontalSize, textureSize);
            //Color32[] _atlasPixels = new Color32[atlas.width * atlas.height];

            //int _counter = 0;
            //foreach (KeyValuePair<Texture2D, List<int>> _pair in _textures)
            //{
            //    Texture2D _texture = _pair.Key;
            //    if (_pair.Key.width != textureSize || _pair.Key.height != textureSize)
            //    {
            //        // Create a scaled copy
            //        _texture = TextureScaler.scaled(_texture, textureSize, textureSize);
            //    }

            //    // Transfer texture's pixels to atlas
            //    Color32[] _pixels = _texture.GetPixels32();
            //    for (int x = 0; x < _texture.width; x++)
            //    {
            //        for (int y = 0; y < _texture.height; y++)
            //        {
            //            Color32 _pixel = _pixels[y * _texture.width + x];
            //            _atlasPixels[y * atlas.width + (x + _counter * textureSize)] = _pixel;
            //        }
            //    }

            //    for (int i = 0; i < _pair.Value.Count; i++)
            //    {
            //        materials[_pair.Value[i]].atlasIndex = _counter;
            //    }
            //    _counter++;
            //}

            //atlas.filterMode = FilterMode.Point;
            //atlas.SetPixels32(_atlasPixels);
            //atlas.Apply(true);
            //atlas.Compress(false);

            //atlasMaterial = Resources.Load<UnityEngine.Material>("Materials/Atlas");
            //atlasMaterial.mainTexture = atlas;
            //atlasMaterial.SetInt("_AtlasCount", atlas.width / textureSize);

            //Resources.Load<UnityEngine.Material>("Materials/Terrain").mainTexture = atlas;
        }
    }
}