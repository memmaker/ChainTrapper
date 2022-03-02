using System;
using System.Collections.Generic;
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
    public class Sheep : GameObject, IWoundable
    {
        private int mCurrentHealth = 5;
        private Vector2 mDesiredDirection;
        private Vector2 mDesiredPosition;
        private BehaviourType mBehaviourType;
        private List<Vector2> mPath = new List<Vector2>();
        private int mCurrentPathNode;

        public Sheep(World world, Vector2 position, Texture2D texture) : base(world, position, texture)
        {
            mDesiredDirection = Helper.Percent(50) ? Helper.RandomDirection() : Vector2.Zero;
            mDesiredPosition = Helper.Percent(50) ? Helper.RandomPosition() : Position;
            mBehaviourType = BehaviourType.FollowPath;//(BehaviourType) Helper.RandomInt(3);
            for (int i = 0; i < 3; i++)
            {
                mPath.Add(Helper.RandomPosition());
            }
            mCurrentPathNode = 0;
            mDesiredPosition = mPath[mCurrentPathNode];
            Speed = Helper.RandomSpeed();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.DrawString(Globals.Globals.DefaultFont, mCurrentHealth.ToString(), Position - (Vector2.UnitY * Constants.PixelPerMeter), Color.White);
        }

        public override void Update(GameTime gameTime, Context context)
        {
            base.Update(gameTime, context);

            if (mBehaviourType == BehaviourType.None)
                return;

            if (mBehaviourType == BehaviourType.StayAtLocation || mBehaviourType == BehaviourType.FollowPath)
            {
                if (IsAtPosition(mDesiredPosition))
                {
                    if (mBehaviourType == BehaviourType.FollowPath)
                    {
                        mCurrentPathNode = (mCurrentPathNode + 1) % mPath.Count;
                        mDesiredPosition = mPath[mCurrentPathNode];
                    }
                    mDesiredDirection = Vector2.Zero;
                }
                else
                {
                    mDesiredDirection = mDesiredPosition - Position;
                    mDesiredDirection.Normalize();
                }
            }
            
            ApplyForce(mDesiredDirection);
            
        }

        public void TakeDamage(int damage)
        {
            mCurrentHealth -= damage;
            if (IsDead)
                ShouldBeRemoved = true;
        }

        public bool IsDead => mCurrentHealth <= 0;
        public int CurrentHealth => mCurrentHealth;
    }
}
