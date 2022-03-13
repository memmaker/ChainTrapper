using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using Microsoft.Xna.Framework;

namespace ChainTrapper.Basics
{
    [DataContract]
    public class Map
    {
        public Map()
        {
            EnemyPath = new List<Vector2>();
            WolfSpawns = new List<Vector2>();
            Walls = new List<Rectangle>();
            GrassRects = new List<Rectangle>();
        }
        public List<Rectangle> Walls { get; set; }

        // By convention: EnemyPath[0] = Start, EnemyPath[Length-1] = Goal
        public List<Vector2> EnemyPath { get; set; } 
        
        // List of Spawn positions for the wolves. Amount of wolves = WolfSpawns.Length
        public List<Vector2> WolfSpawns { get; set; }
        public List<Rectangle> GrassRects { get; set; }

        public void SaveToFile(string filename)
        {
            Stream stream = File.Open(filename, FileMode.Create);
            var writer = new BinaryWriter(stream);
            
            writer.Write(EnemyPath.Count);
            
            foreach (var sheepNode in EnemyPath)
            {
                writer.Write(sheepNode.X);
                writer.Write(sheepNode.Y);
            }
            
            writer.Write(WolfSpawns.Count);
            
            foreach (var wolfSpawn in WolfSpawns)
            {
                writer.Write(wolfSpawn.X);
                writer.Write(wolfSpawn.Y);
            }
            
            writer.Write(Walls.Count);
            
            foreach (var wall in Walls)
            {
                writer.Write(wall.X);
                writer.Write(wall.Y);
                writer.Write(wall.Width);
                writer.Write(wall.Height);
            }
            
            writer.Write(GrassRects.Count);
            
            foreach (var grassRect in GrassRects)
            {
                writer.Write(grassRect.X);
                writer.Write(grassRect.Y);
                writer.Write(grassRect.Width);
                writer.Write(grassRect.Height);
            }
            
            stream.Close();
        }

        public static Map FromFile(string filename)
        {
            Map loadedMap = new Map();
            
            var stream = File.Open(filename, FileMode.Open);
            var reader = new BinaryReader(stream);

            int sheepCount = reader.ReadInt32();

            for (int i = 0; i < sheepCount; i++)
            {
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                
                loadedMap.EnemyPath.Add(new Vector2(x, y));
            }
            
            int wolfCount = reader.ReadInt32();

            for (int i = 0; i < wolfCount; i++)
            {
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                
                loadedMap.WolfSpawns.Add(new Vector2(x, y));
            }
            
            int wallCount = reader.ReadInt32();

            for (int i = 0; i < wallCount; i++)
            {
                int x = reader.ReadInt32();
                int y = reader.ReadInt32();
                
                int width = reader.ReadInt32();
                int height = reader.ReadInt32();
                
                loadedMap.Walls.Add(new Rectangle(x, y, width, height));
            }
            
            int grassCount = reader.ReadInt32();

            for (int i = 0; i < grassCount; i++)
            {
                int x = reader.ReadInt32();
                int y = reader.ReadInt32();
                
                int width = reader.ReadInt32();
                int height = reader.ReadInt32();
                
                loadedMap.GrassRects.Add(new Rectangle(x, y, width, height));
            }
            
            stream.Close();

            return loadedMap;
        }
    }
}