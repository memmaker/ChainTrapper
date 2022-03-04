using System;
using Box2DX.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChainTrapper
{
    public class Player : GameObject
    {
        public Player(World world, Vector2 position, Texture2D texture) : base(world, position, texture)
        { 
            CreatePhysicsRepresentation(
                1.0f, 
                1.0f, 
                0.9f, 
                0.9f, 
                0.0f, 
                false, 
                false
                );
        }
    }
}
