using System;
using Box2DX.Dynamics;
using ChainTrapper.Basics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChainTrapper.Traps
{
    public class BombTrap : Bomb, IGameObjectCollisionListener
    {
        public BombTrap(World world, Vector2 drawPosition, Texture2D texture) : base(world, drawPosition, texture)
        {
            CreatePhysicsRepresentation(
                2f, 
                2.0f, 
                0.9f, 
                0.9f, 
                0.1f, 
                false,
                true
            );
        }


        public void OnCollisionBegin(GameContext gameContext, GameObject go)
        {
            if (go is Enemy)
            {
                Explode(gameContext);
            }
        }
    }
}
