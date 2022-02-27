using System;
using Box2DX.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChainTrapper
{
    public class Projectile : GameObject
    {
        public Projectile(World world, Vector2 position, Texture2D texture) : base(world, position, texture, 0.3f, true)
        {
            
        }
    }
}
