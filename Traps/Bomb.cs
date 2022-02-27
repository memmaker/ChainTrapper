using System;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using ChainTrapper.Basics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChainTrapper.Traps
{
    public abstract class Bomb : GameObject
    {
        private float mExplosionRadius = 128.0f;
        private float mStrength = 0.7f;

        public Bomb(World world, Vector2 position, Texture2D texture) : base(world, position, texture)
        {
            
        }

        protected void Explode(Context context)
        {
            ShouldBeRemoved = true;
            var explosionArea = new AABB()
            {
                UpperBound = new Vec2((Position.X + mExplosionRadius) / Constants.PixelPerMeter, (Position.Y + mExplosionRadius) / Constants.PixelPerMeter),
                LowerBound = new Vec2((Position.X - mExplosionRadius) / Constants.PixelPerMeter, (Position.Y - mExplosionRadius) / Constants.PixelPerMeter)
            };
            Fixture[] affectedShapes = new Fixture[50];
            var affectedCount = mWorld.Query(explosionArea, affectedShapes, 50);

            for (var index = 0; index < affectedCount; index++)
            {
                var fixture = affectedShapes[index];
                
                if (fixture.UserData == null) 
                    continue;
                
                var gameObject = (GameObject)fixture.UserData;;
                if (gameObject == this)
                    continue;

                var distance = Vector2.Distance(Position, gameObject.Position);

                if (distance <= mExplosionRadius)
                {
                    float distFactor =
                        (mExplosionRadius - distance) / mExplosionRadius; // 128 - 0..128 -> 0..128 -> 0,0..1,0
                    var forceVector = gameObject.Position - Position;
                    gameObject.ApplyImpulse(forceVector * mStrength * distFactor);
                }
            }
        }
    }


}
