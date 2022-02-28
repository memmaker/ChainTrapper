using Box2DX.Common;
using Box2DX.Dynamics;
using ChainTrapper.Traps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChainTrapper.Basics
{
    public class Fire : GameObject, IVictimCollisionListener
    {
        private double mDelay = Helper.RandomInt(5) + 3;
        public Fire(World world, Vector2 position, Texture2D texture) : base(world, position, texture, 1f, false, true)
        {
            
        }

        public void OnVictimEntered(Context context, GameObject victim)
        {
            victim.IsBurning = true;
        }

        public override void Update(GameTime gameTime, Context context)
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
                        Vector2 position = Position + (Helper.RandomDirection() * 32.0f);
                        var fire = new Fire(mWorld, position, mSprite);
                        context.AllGameObjects.Add(fire);
                        mDelay = Helper.RandomInt(5) + 3;
                    }
                }
            }
        }
    }
}