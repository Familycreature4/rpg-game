using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.IO.Compression;
using System.Text.Json.Serialization;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RPG.Editor
{
    /// <summary>
    /// Saves and loads levels on the disk
    /// </summary>
    public static class Serializer
    {
        static string DirectoryPath => $"{Application.persistentDataPath}/Worlds";
        public static bool SaveWorldJSON(string worldName, World world)
        {
            // Serialize world to MemoryStream
            // Compress to file
            // Ensure directory exists
            if (Directory.Exists(DirectoryPath) == false)
            {
                Directory.CreateDirectory(DirectoryPath);
            }

            string fileDirectory = $"{DirectoryPath}/{worldName}.json";
            bool saved = false;

            using (FileStream fileStream = new FileStream(fileDirectory, FileMode.Create))
            {
                fileStream.SetLength(0);
                JsonSerializer serializer = new JsonSerializer();
                using StreamWriter sw = new StreamWriter(fileStream);
                using JsonWriter writer = new JsonTextWriter(sw);

                writer.WriteStartObject();

                writer.WritePropertyName("chunks");
                writer.WriteStartArray();
                foreach (Chunk chunk in world.chunks.Values)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("coords");
                    writer.WriteValue($"{chunk.coords.x}.{chunk.coords.y}.{chunk.coords.z}");

                    // Encode tile data
                    byte[] bytes = EncodeTileData(chunk.tiles);
                    string tileData = System.Convert.ToBase64String(bytes);
                    writer.WritePropertyName("tiles", false);
                    writer.WriteValue(tileData);

                    writer.WriteEndObject();
                }
                writer.WriteEndArray();

                writer.WriteEndObject();
            }


            return saved;
        }
        public static bool LoadWorldJSON(string worldName, World world)
        {
            string fileName = $"{DirectoryPath}/{worldName}.json";
            if (File.Exists(fileName) == false)
                return false;

            Dictionary<Vector3Int, Chunk> chunks = null;
            bool loaded = false;
            chunks = new Dictionary<Vector3Int, Chunk>();

            string jsonText = System.IO.File.ReadAllText(fileName);

            Newtonsoft.Json.Linq.JObject token = Newtonsoft.Json.Linq.JObject.Parse(jsonText);

            foreach (JObject child in token["chunks"].Children<JObject>())
            {
                JProperty tiles = child.Property("tiles");
                // For each chunk
                Chunk chunk = new Chunk();
                string[] coordStrings = child["coords"].ToString().Split(".");
                chunk.coords = new Vector3Int(int.Parse(coordStrings[0]), int.Parse(coordStrings[1]), int.Parse(coordStrings[2]));
                string tileStringData = tiles.Value.ToString();
                chunk.tiles = DecodeTileData(System.Convert.FromBase64String(tileStringData));

                chunks.Add(chunk.coords, chunk);
            }

            world.SetChunks(chunks);
            loaded = true;

            return loaded;
        }
        static byte[] EncodeTileData(Tile[] tiles)
        {
            using (MemoryStream bytesStream = new MemoryStream())
            {
                using (DeflateStream deflate = new DeflateStream(bytesStream, System.IO.Compression.CompressionLevel.Optimal))
                {
                    using (MemoryStream writerStream = new MemoryStream())
                    {
                        BinaryWriter writer = new BinaryWriter(writerStream, System.Text.Encoding.UTF8);
                        writer.BaseStream.Position = 0;

                        for (int i = 0; i < tiles.Length; i++)
                        {
                            Tile tile = tiles[i];

                            if (tile.shape == null || tile.material == null)
                            {
                                writer.Write("air");
                            }
                            else
                            {
                                writer.Write(tile.shape.name);
                                writer.Write(tile.material.name);

                                Vector3 angles = tile.rotation.eulerAngles;
                                writer.Write(angles.x);
                                writer.Write(angles.y);
                                writer.Write(angles.z);
                            }
                        }

                        deflate.Write(writerStream.ToArray());
                    }
                }

                return bytesStream.ToArray();
            }
        }
        static Tile[] DecodeTileData(byte[] data)
        {
            using (MemoryStream decompressedStream = new MemoryStream())
            {
                using (MemoryStream stream = new MemoryStream(data))
                {
                    stream.Position = 0;
                    using (DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Decompress))
                    {
                        deflateStream.CopyTo(decompressedStream);
                    }
                }

                BinaryReader reader = new BinaryReader(decompressedStream, System.Text.Encoding.UTF8);
                reader.BaseStream.Position = 0;

                Tile[] tiles = new Tile[Chunk.sizeCubed];

                for (int i = 0; i < tiles.Length; i++)
                {
                    string header = reader.ReadString();

                    if (header == "air")
                    {
                        tiles[i] = Tile.Air;
                    }
                    else
                    {
                        Tile tileBuffer = default;
                        string shapeName = header;
                        string mat = reader.ReadString();

                        tileBuffer.shape = TileShape.GetShape(shapeName);
                        tileBuffer.material = TileMaterial.GetMaterial(mat);

                        tileBuffer.rotation = Quaternion.Euler(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                        tiles[i] = tileBuffer;
                    }
                    
                }

                return tiles;
            }
        }
    }
}
