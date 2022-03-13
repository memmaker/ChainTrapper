using System;
using System.Collections.Generic;
using Box2DX.Common;
using Box2DX.Dynamics;
using ChainTrapper.Basics;
using ChainTrapper.Globals;
using ChainTrapper.Traits;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace ChainTrapper
{
    public enum BehaviourType { None, MoveInDirection, StayAtLocation, FollowPath}
    public class BreadCrumb : GameObject
    {
        public static double LifeTime = 10.0d;
        private double mLifeTimeLeft = BreadCrumb.LifeTime;
        public double LifeTimeLeft => mLifeTimeLeft;

        public BreadCrumb(World world, Vector2 drawPosition, Texture2D texture) : base(world, drawPosition, texture)
        {
            CreatePhysicsRepresentation(
                4f, 
                2.0f, 
                0.9f, 
                0.9f, 
                0.1f, 
                false, 
                true,
                0.3f
            );
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Globals.Globals.DebugEnabled) return;
            base.Draw(spriteBatch);
            spriteBatch.DrawString(Globals.Globals.DefaultFont, LifeTimeLeft.ToString("0.0"), DrawPosition - (Vector2.UnitY * Constants.PixelPerMeter), Color.White);
        }

        public override void Update(GameTime gameTime, GameContext gameContext)
        {
            mLifeTimeLeft = LifeTimeLeft - gameTime.ElapsedGameTime.TotalSeconds;
            if (LifeTimeLeft <= 0.0d)
            {
                ShouldBeRemoved = true;
            }
        }
    }
}
