using System;
using Box2DX.Common;
using Box2DX.Dynamics;
using ChainTrapper.Basics;
using ChainTrapper.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Math = System.Math;

namespace ChainTrapper
{
    public class Throwable : GameObject
    {
        private Vec2 mTarget;
        private Vec2 mTargetDir;
        private float mDistanceToCover;
        private double mLifeTimeCounter = 10.0f;
        public bool IsAbleToCollide { get; private set; }
        public Throwable(World world, Vector2 drawPosition, Texture2D texture) : base(world, drawPosition, texture)
        {
            IsAbleToCollide = false;
            CreatePhysicsRepresentation(
                0.2f, 
                1.0f, 
                0.3f, 
                0.9f, 
                0.1f, 
                false, 
                false,
                0.2f
            );
        }

        public void SetTarget(Vec2 target)
        {
            mTarget = target;
            mTargetDir = target - Position;
            mDistanceToCover = mTargetDir.Length();
            mBody.SetLinearVelocity(mTargetDir * 0.8f);
        }

        public override void Update(GameTime gameTime, GameContext gameContext)
        {
            base.Update(gameTime, gameContext);
            if (IsAtPosition(mTarget))
            {
                IsAbleToCollide = true;
                mBody.SetLinearVelocity(Vec2.Zero);
                ApplyForce(mTargetDir);
                AlertEnemiesNearby();
            }

            mLifeTimeCounter -= gameTime.ElapsedGameTime.TotalSeconds;
            if (mLifeTimeCounter <= 0.0d)
            {
                ShouldBeRemoved = true;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float scale = 0.3f;
            if (!IsAbleToCollide) // during flight
            {
                var dist = (mTarget - Position).Length();
                var deltaCenter = (mDistanceToCover/2) - Math.Abs((mDistanceToCover/2) - dist);
                var scaledFactor = deltaCenter / (mDistanceToCover / 2); // 0..1
                scale += (scaledFactor * 0.3f);
            }
            spriteBatch.Draw(mSprite, DrawPosition, null, Color.LightGray, 0.0f, mSprite.Bounds.Center.ToVector2(), scale, SpriteEffects.None, 0.0f);
        }

        private void AlertEnemiesNearby()
        {
            Fixture[] fixtures = new Fixture[100];
            int count = mWorld.Query(Position, 7.0f, fixtures);
            for (int i = 0; i < count; i++)
            {
                var fixture = fixtures[i];
                if (fixture.UserData is Enemy enemy)
                {
                    enemy.SetPointOfInterest(Position);
                }
            }
        }
    }
}
