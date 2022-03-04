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
            mDesiredDirection = Vector2.Zero;
            mDesiredPosition = Position;
            mBehaviourType = BehaviourType.None;
            Speed = Helper.RandomSpeed();
            
            CreatePhysicsRepresentation(
                2f, 
                2.0f, 
                1.9f, 
                0.9f, 
                0.1f, 
                false, 
                false
            );
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            var sheepDebug = "TargetNode: " + mCurrentPathNode.ToString();
            spriteBatch.DrawString(Globals.Globals.DefaultFont, mCurrentHealth.ToString(), Position - (Vector2.UnitY * Constants.PixelPerMeter), Color.White);
            spriteBatch.DrawString(Globals.Globals.DefaultFont, sheepDebug, Position - (Vector2.UnitY * (Constants.PixelPerMeter * 1.5f)), Color.White);
        }

        public override void Update(GameTime gameTime, GameContext gameContext)
        {
            base.Update(gameTime, gameContext);

            if (mBehaviourType == BehaviourType.None) return;
            
            if (IsAtPosition(mDesiredPosition, Constants.PixelPerMeter))
            {
                mCurrentPathNode = (mCurrentPathNode + 1) % Path.Count;
                mDesiredPosition = Path[mCurrentPathNode];
                mDesiredDirection = Vector2.Zero;
            }
            else
            {
                mDesiredDirection = mDesiredPosition - Position;
                mDesiredDirection.Normalize();
            }
            
            ApplyForce(mDesiredDirection * Speed);
        }

        public void TakeDamage(int damage)
        {
            mCurrentHealth -= damage;
            if (IsDead)
                ShouldBeRemoved = true;
        }

        public bool IsDead => mCurrentHealth <= 0;
        public int CurrentHealth => mCurrentHealth;

        public List<Vector2> Path
        {
            get => mPath;
            set
            {
                mPath = value;
                mCurrentPathNode = 0;
                mDesiredPosition = mPath[mCurrentPathNode];
            }
        }

        public void StartWalk()
        {
            mBehaviourType = BehaviourType.FollowPath;
        }
    }
}
