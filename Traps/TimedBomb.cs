using System;
using Box2DX.Dynamics;
using ChainTrapper.Basics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChainTrapper.Traps
{
    public class TimedBomb : Bomb
    {
        private double mDelay = 3.0f;

        public TimedBomb(World world, Vector2 position, Texture2D texture) : base(world, position, texture)
        {
            CreatePhysicsRepresentation(
                2f, 
                2.0f, 
                0.9f, 
                0.9f, 
                0.1f, 
                false,
                false
            );
        }

        public override void Update(GameTime gameTime, GameContext gameContext)
        {
            if (mDelay > 0.0f && !ShouldBeRemoved)
            {
                mDelay -= gameTime.ElapsedGameTime.TotalSeconds;
                if (mDelay <= 0.0f)
                {
                    Explode(gameContext);
                }
            }
            base.Update(gameTime, gameContext);
        }
    }
}
