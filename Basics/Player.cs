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
        private Texture2D mDuckTexture;
        private Vec2 mRollDirection;
        public bool IsCrouching { get; set; }
        public bool IsDead => mCurrentHealth <= 0;
        public int CurrentHealth => mCurrentHealth;
        public bool IsRolling { get; set; }
        private int mRollCounter = 0;
        public bool IsHidingInGrass => IsCrouching && Grass != null;
        
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
            IsCrouching = false;
            IsRolling = false;
        }

        public void SetDuckTexture(Texture2D texture)
        {
            mDuckTexture = texture;
        }
        public override void Update(GameTime gameTime, GameContext gameContext)
        {
            base.Update(gameTime, gameContext);
            PlaceBreadCrumbs(gameTime, gameContext);
            if (IsRolling)
            {
                UpdateRolling();
            }
        }

        private void UpdateRolling()
        {
            ApplyForce(mRollDirection);
            mRollCounter++;
            if (mRollCounter >= 4)
            {
                mRollCounter = 0;
                IsRolling = false;
            }
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
            Color drawColor = Color.White;
            if (IsBurning)
            {
                drawColor = Helper.Percent(50) ? Color.OrangeRed : Color.Red;
            }

            var offset = Vector2.Zero;
            Texture2D textureToDraw = mSprite;
            
            if (IsCrouching)
            {
                textureToDraw = mDuckTexture;
                offset = new Vector2(0, 0.5f * Constants.PixelPerMeter);
            }
            
            spriteBatch.Draw(textureToDraw, DrawPosition + offset, null, drawColor, 0.0f, mSprite.Bounds.Center.ToVector2(), Vector2.One, SpriteEffects.None, 0.0f);
            
            spriteBatch.DrawString(Globals.Globals.DefaultFont, mCurrentHealth.ToString(), DrawPosition - (Vector2.UnitY * Constants.PixelPerMeter), Color.White);
            spriteBatch.DrawString(Globals.Globals.DefaultFont, "Hiding: " + IsHidingInGrass, DrawPosition - (Vector2.UnitY * 2 * Constants.PixelPerMeter), Color.White);
        }
        
        public void TakeDamage(int damage)
        {
            mCurrentHealth -= damage;
        }

        public void MovePlayer(Vec2 direction)
        {
            if (IsDead || IsRolling) return;
            var speed = IsCrouching ? Speed * 0.25f : Speed;
            direction.SetMagnitude(speed);
            ApplyForce(direction);
        }

        public void DiveRoll()
        {
            if (IsDead || IsRolling) return;
            mRollDirection = this.Velocity;
            mRollDirection.SetMagnitude(Speed * 9f);
            ApplyForce(mRollDirection);
            IsRolling = true;
        }
    }
}
