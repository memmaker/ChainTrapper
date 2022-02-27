using System;
using Box2DX.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChainTrapper
{
    public class Player : GameObject
    {
        public Player(World world, Vector2 position, Texture2D texture) : base(world, position, texture, 1.8f)
        {  
        }
    }
}
