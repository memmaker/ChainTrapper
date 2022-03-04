using System;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using ChainTrapper.Basics;
using ChainTrapper.Globals;
using ChainTrapper.Traits;
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

        protected void Explode(GameContext gameContext)
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
                
                var userData = fixture.UserData;
                if (userData == this || !(userData is GameObject))
                    continue;
                
                var gameObject = (GameObject) userData;
                
                var distance = Vector2.Distance(Position, gameObject.Position);

                if (distance <= mExplosionRadius)
                {
                    float distFactor =
                        (mExplosionRadius - distance) / mExplosionRadius; // 128 - 0..128 -> 0..128 -> 0,0..1,0
                    var forceVector = gameObject.Position - Position;
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
