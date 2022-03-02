using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ChainTrapper.Basics
{
    [Serializable()]
    public class Map
    {
        // How many Sheep to Spawn
        public int SheepCount { get; set; }
        
        // By convention: SheepPath[0] = Start, SheepPath[Length-1] = Goal
        public List<Vector2> SheepPath { get; set; } 
        
        // List of Spawn positions for the wolves. Amount of wolves = WolfSpawns.Length
        public List<Vector2> WolfSpawns { get; set; }
        
        
    }
}