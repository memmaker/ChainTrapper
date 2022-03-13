using System;
using Box2DX.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChainTrapper
{
    public class Projectile : GameObject
    {
        public Projectile(World world, Vector2 drawPosition, Texture2D texture) : base(world, drawPosition, texture)
        {
            CreatePhysicsRepresentation(
                0.2f, 
                1.0f, 
                0.9f, 
                0.9f, 
                0.1f, 
                true, 
                false
            );
        }
    }
}
