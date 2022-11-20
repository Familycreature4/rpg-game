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
using RPG.Editor.Entities;

namespace RPG.Editor
{
    /// <summary>
    /// Saves and loads levels on the disk
    /// </summary>
    public static class Serializer
    {
        public static string extension = "world";
        public static string DirectoryPath => $"{Application.persistentDataPath}/Worlds";
        public static bool SaveWorldJSON(string path, World world)
        {
            bool saved = false;

            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                fileStream.SetLength(0);
                fileStream.Position = 0;

                JObject json = new JObject();
                JArray chunksArray = new JArray();
                json.Add("chunks", chunksArray);
                json.Add("chunks-encoding", "base64");

                foreach (Chunk chunk in world.Chunks.Values)
                {
                    JObject jChunk = new JObject();
                    jChunk.Add("coords", EncodeVector3(chunk.coords));

                    // Encode tile data
                    byte[] bytes = EncodeTileData(chunk.tiles);
                    string tileData = System.Convert.ToBase64String(bytes);
                    jChunk.Add("tiles", tileData);

                    chunksArray.Add(jChunk);
                }

                // Write all entities in the scene to world
                JArray entitiesJson = new JArray();
                json.Add("entities", entitiesJson);
                Entity[] entities = GameObject.FindObjectsOfType<Entity>();
                foreach (Entity entity in entities)
                {
                    JObject entityJson = new JObject();
                    entity.OnSerialize(entityJson);

                    entitiesJson.Add(entityJson);
                }

                StreamWriter streamWriter = new StreamWriter(fileStream);
                streamWriter.Write(JsonConvert.SerializeObject(json));
                streamWriter.Close();
            }

            return saved;
        }
        public static bool LoadWorldJSON(string path, World world)
        {
            if (File.Exists(path) == false)
                return false;

            Dictionary<Vector3Int, Chunk> chunks = null;
            bool loaded = false;
            chunks = new Dictionary<Vector3Int, Chunk>();

            string jsonText = System.IO.File.ReadAllText(path);

            Newtonsoft.Json.Linq.JObject token = Newtonsoft.Json.Linq.JObject.Parse(jsonText);

            foreach (JObject child in token["chunks"].Children<JObject>())
            {
                JProperty tiles = child.Property("tiles");
                // For each chunk
                Chunk chunk = new Chunk();
                chunk.coords = Vector3Int.FloorToInt(DecodeVector3(child["coords"].ToString()));
                string tileStringData = tiles.Value.ToString();
                chunk.tiles = DecodeTileData(System.Convert.FromBase64String(tileStringData));

                chunks.Add(chunk.coords, chunk);
            }

            world.SetChunks(chunks);

            // Clear existing entities before loading new ones
            Entity.ClearEntities();

            if (token.ContainsKey("entities"))
            {
                foreach (JObject entityJson in token["entities"].Children<JObject>())
                {
                    // Determine the type of entity
                    // Create a gameObject and appropriate component
                    GameObject gameObject = new GameObject(entityJson["name"].ToString());
                    System.Type type = System.Type.GetType(entityJson["type"].ToString());

                    Entity entity = gameObject.AddComponent(type) as Entity;
                    entity.OnDeserialize(entityJson);
                }
            }
            
            loaded = true;

            return loaded;
        }
        public static bool SaveWorldExplorer(World world)
        {
            string filePath = UnityEditor.EditorUtility.SaveFilePanel("Save World", Serializer.DirectoryPath, "world", Serializer.extension);
            if (filePath != "")
            {
                return Serializer.SaveWorldJSON(filePath, World.Current);
            }

            return false;
        }
        public static bool LoadWorldExplorer(World world)
        {
            string filePath = UnityEditor.EditorUtility.OpenFilePanel("Open World", DirectoryPath, extension);
            return LoadWorldJSON(filePath, world);
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
        public static string EncodeVector3(Vector3 v)
        {
            return $"{v.x},{v.y},{v.z}";
        }
        public static Vector3 DecodeVector3(string text)
        {
            string[] split = text.Split(",");
            return new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));
        }
    }
}
