using System;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using ChainTrapper.Basics;
using ChainTrapper.Globals;
using ChainTrapper.Traits;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace ChainTrapper.Traps
{
    public abstract class Bomb : GameObject
    {
        private float mExplosionRadius = 128.0f;
        private float mStrength = 0.7f;
        private bool mStartAnimation = false;
        private float mScale;

        public Bomb(World world, Vector2 drawPosition, Texture2D texture) : base(world, drawPosition, texture)
        {
            mScale = 1.0f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Color drawColor = Color.White;
            if (IsBurning)
            {
                drawColor = Helper.Percent(50) ? Color.OrangeRed : Color.Red;
            }
            spriteBatch.Draw(mSprite, DrawPosition, null, drawColor, mBody.GetAngle(), mSprite.Bounds.Center.ToVector2(), mScale, SpriteEffects.None, 0.0f);
        }

        public override void Update(GameTime gameTime, GameContext gameContext)
        {
            base.Update(gameTime, gameContext);
            if (mStartAnimation)
            {
                mScale += 1f;
                if (mScale >= 4.0f)
                {
                    ShouldBeRemoved = true;
                }
            }
        }

        protected void Explode(GameContext gameContext)
        {
            mStartAnimation = true;
            var explosionArea = new AABB()
            {
                UpperBound = new Vec2((DrawPosition.X + mExplosionRadius) / Constants.PixelPerMeter, (DrawPosition.Y + mExplosionRadius) / Constants.PixelPerMeter),
                LowerBound = new Vec2((DrawPosition.X - mExplosionRadius) / Constants.PixelPerMeter, (DrawPosition.Y - mExplosionRadius) / Constants.PixelPerMeter)
            };
            Fixture[] affectedShapes = new Fixture[50];
            var affectedCount = mWorld.Query(explosionArea, affectedShapes, 50);

            for (var index = 0; index < affectedCount; index++)
            {
                var fixture = affectedShapes[index];
                
                if (fixture.UserData == null) 
                    continue;
                
                var userData = fixture.UserData;
                if (userData == this || !(userData is GameObject))
                    continue;
                
                var gameObject = (GameObject) userData;
                
                var distance = Vector2.Distance(DrawPosition, gameObject.DrawPosition);

                if (distance <= mExplosionRadius)
                {
                    float distFactor =
                        (mExplosionRadius - distance) / mExplosionRadius; // 128 - 0..128 -> 0..128 -> 0,0..1,0
                    var forceVector = gameObject.DrawPosition - DrawPosition;
                    gameObject.ApplyImpulse(forceVector * mStrength * distFactor);
                    if (gameObject is IWoundable)
                    {
                        var woundable = (IWoundable) gameObject;
                        var damage = (int) (5*distFactor);
                        woundable.TakeDamage(damage);
                    }
                }
            }
        }
    }


}
