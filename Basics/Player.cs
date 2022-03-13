using System;
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
    public class Player : GameObject, IWoundable
    {
        private int mCurrentHealth = 7;
        private double mBreadCrumbTimer = 1.0f;
        private Vec2 mLastBreadCrumbPosition;
        private double mLastBreadCrumbTime;

        public Player(World world, Vector2 drawPosition, Texture2D texture) : base(world, drawPosition, texture)
        { 
            CreatePhysicsRepresentation(
                .05f, 
                1.0f, 
                10.0f, 
                0.9f, 
                0.0f, 
                false, 
                false
                );
        }

        public override void Update(GameTime gameTime, GameContext gameContext)
        {
            base.Update(gameTime, gameContext);

            PlaceBreadCrumbs(gameTime, gameContext);
        }

        private void PlaceBreadCrumbs(GameTime gameTime, GameContext gameContext)
        {
            bool placeBreadCrumb = false;
            if (Vec2.Distance(mLastBreadCrumbPosition, mBody.GetPosition()) < 1.0f &&
                gameTime.TotalGameTime.TotalSeconds - mLastBreadCrumbTime >= BreadCrumb.LifeTime)
            {
                placeBreadCrumb = true;
            }
            else if (Vec2.Distance(mLastBreadCrumbPosition, mBody.GetPosition()) > 1.0f &&
                     gameTime.TotalGameTime.TotalSeconds - mLastBreadCrumbTime >= mBreadCrumbTimer)
            {
                placeBreadCrumb = true;
            }

            if (placeBreadCrumb)
            {
                mLastBreadCrumbTime = gameTime.TotalGameTime.TotalSeconds;
                mLastBreadCrumbPosition = mBody.GetPosition();
                var breadCrumb = new BreadCrumb(mWorld, DrawPosition, mSprite);
                gameContext.AllGameObjects.Add(breadCrumb);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.DrawString(Globals.Globals.DefaultFont, mCurrentHealth.ToString(), DrawPosition - (Vector2.UnitY * Constants.PixelPerMeter), Color.White);
        }
        
        public void TakeDamage(int damage)
        {
            mCurrentHealth -= damage;
        }

        public bool IsDead => mCurrentHealth <= 0;
        public int CurrentHealth => mCurrentHealth;
    }
}
