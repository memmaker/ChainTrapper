using System;
using System.Collections.Generic;
using Box2DX.Dynamics;
using ChainTrapper.Basics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChainTrapper
{
    public class Wolf : Sheep
    {
        public Wolf(World world, Vector2 position, Texture2D texture) : base(world, position, texture)
        {
            
        }
    }
}
