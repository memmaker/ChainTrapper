using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using ChainTrapper.Globals;
using ChainTrapper.Traps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChainTrapper.Basics
{
    public class Fire : GameObject, IGameObjectCollisionListener
    {
        private double mDelay = Helper.RandomInt(5) + 3;
        public Fire(World world, Vector2 drawPosition, Texture2D texture) : base(world, drawPosition, texture)
        {
            CreatePhysicsRepresentation(
                4f, 
                1.0f, 
                0.9f, 
                0.9f, 
                0.1f, 
                false, 
                true
            );
        }

        public void OnCollisionBegin(GameContext gameContext, GameObject go)
        {
            go.IsBurning = true;
        }

        public void OnCollisionEnd(GameContext gameContext, GameObject go)
        {
            
        }

        public override void Update(GameTime gameTime, GameContext gameContext)
        {
            if (mDelay > 0.0f)
            {
                mDelay -= gameTime.ElapsedGameTime.TotalSeconds;
                if (mDelay <= 0.0f)
                {
                    if (Helper.Percent(50))
                    {
                        ShouldBeRemoved = true;
                    }
                    else
                    {
                        Vector2 position = DrawPosition + (Helper.RandomDirection() * (Constants.PixelPerMeter * 1.5f));
                        Fixture[] fixtures = new Fixture[10];
                        int hitCount = mWorld.Query(new AABB()
                        {
                            LowerBound = new Vec2((position.X - (Constants.PixelPerMeter / 2.0f)) / Constants.PixelPerMeter, (position.Y - (Constants.PixelPerMeter / 2.0f)) / Constants.PixelPerMeter),
                            UpperBound = new Vec2((position.X + (Constants.PixelPerMeter / 2.0f)) / Constants.PixelPerMeter, (position.Y + (Constants.PixelPerMeter / 2.0f)) / Constants.PixelPerMeter)
                        }, fixtures, fixtures.Length);
                        for (var index = 0; index < hitCount; index++)
                        {
                            var fixture = fixtures[index];
                            if (fixture.UserData is Fire || fixture.UserData is Wall)
                            {
                                mDelay = Helper.RandomInt(5) + 3;
                                return;
                            }
                        }
                        
                        var fire = new Fire(mWorld, position, mSprite);
                        gameContext.AllGameObjects.Add(fire);
                        mDelay = Helper.RandomInt(5) + 3;
                    }
                }
            }
        }
    }
}