using System;
using Box2DX.Collision;
using Box2DX.Dynamics;
using ChainTrapper.Globals;
using ChainTrapper.Traits;
using ChainTrapper.Traps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace ChainTrapper.Basics
{
    public enum WolfState { Sleeping, Awake, Chasing, Attacking, Pain, Dying}
    public class Wolf : GameObject, IVictimCollisionListener, IWoundable
    {
        private int mReactionDelay = Helper.RandomInt(1, 4);
        private double mReactionCounter;
        private WolfState mState = WolfState.Awake;
        private Sheep mDesiredSheep = null;
        private int mCurrentHealth = 5;

        public Wolf(World world, Vector2 position, Texture2D texture) : base(world, position, texture)
        {
            mReactionCounter = mReactionDelay;
            Speed = 32;
            CreatePhysicsRepresentation(
                3f, 
                3.0f, 
                3f, 
                0.9f, 
                0.0f, 
                false, 
                false
            );
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            var wolfDebug = Enum.GetName(typeof(WolfState), mState);
            spriteBatch.DrawString(Globals.Globals.DefaultFont, mCurrentHealth.ToString(), Position - (Vector2.UnitY * Constants.PixelPerMeter), Color.White);
            spriteBatch.DrawString(Globals.Globals.DefaultFont, wolfDebug, Position - (Vector2.UnitY * (Constants.PixelPerMeter * 1.5f)), Color.White);
        }

        public void WakeUp()
        {
            mState = WolfState.Awake;
        }
        public override void Update(GameTime gameTime, GameContext gameContext)
        {
            base.Update(gameTime, gameContext);
            switch (mState)
            {
                case WolfState.Sleeping:
                    return;
                case WolfState.Awake:
                    UpdateReactionCounter(gameTime, gameContext);
                    break;
                case WolfState.Chasing:
                    ChaseSheep();
                    break;
                case WolfState.Attacking:
                    UpdateReactionCounter(gameTime, gameContext);
                    break;
            }
        }

        private void AttackSheep()
        {
            if (mDesiredSheep != null && !mDesiredSheep.IsDead && IsNextTo(mDesiredSheep))
            {
                mDesiredSheep.TakeDamage(1);
                mState = WolfState.Chasing;
            }
            else 
            {
                mDesiredSheep = null;
                mState = WolfState.Awake;
                return;
            }
        }

        private void ChaseSheep()
        {
            if (mDesiredSheep == null || mDesiredSheep.IsDead)
            {
                mDesiredSheep = null;
                mState = WolfState.Awake;
                return;
            }

            if (IsNextTo(mDesiredSheep))
            {
                mState = WolfState.Attacking;
                return;
            }
            
            var desiredDirection = mDesiredSheep.Position - Position;
            desiredDirection.Normalize();
            
            ApplyForce(desiredDirection * Speed);
        }

        private void UpdateReactionCounter(GameTime gameTime, GameContext gameContext)
        {
            if (mReactionCounter > 0.0f)
            {
                mReactionCounter -= gameTime.ElapsedGameTime.TotalSeconds;
                if (mReactionCounter <= 0.0f)
                {
                    if (mState == WolfState.Awake)
                    {
                        LookForSheep(gameContext);
                    }
                    else if (mState == WolfState.Attacking)
                    {
                        AttackSheep();
                    }
                    mReactionCounter = mReactionDelay;
                }
            }
        }

        private void LookForSheep(GameContext gameContext)
        {
            var sortedSheep = gameContext.AllGameObjects.FindAll(go => go is Sheep);
            sortedSheep.Sort((go1, go2) => Vector2.DistanceSquared(go1.Position, Position)
                .CompareTo(Vector2.DistanceSquared(go2.Position, Position)));
            
            foreach (var o in sortedSheep)
            {
                var possibleSheep = (Sheep) o;
                Fixture[] hitFixtures;
                mWorld.Raycast(
                    new Segment()
                    {
                        P1 = mBody.GetPosition(),
                        P2 = possibleSheep.Body.GetPosition()
                    },
                    out hitFixtures,
                    20,
                    true,
                    null
                );

                for (int i = 0; i < hitFixtures.Length; i++)
                {
                    if (hitFixtures[i] == null) continue;
                        
                    var firstVisibleFixture = hitFixtures[i];
                    var firstVisibleGo = firstVisibleFixture.UserData;
                    if (firstVisibleGo is Wall) break;
                    if (firstVisibleGo is Sheep sheep)
                    {
                        mDesiredSheep = sheep;
                        mState = WolfState.Chasing;
                        return;
                    }
                }
            }
        }

        public void OnVictimEntered(GameContext gameContext, GameObject victim)
        {
            if (victim is Sheep)
            {
                mDesiredSheep = (Sheep) victim;
                mState = WolfState.Attacking;
                AttackSheep();
            }
        }

        public void TakeDamage(int damage)
        {
            mCurrentHealth -= damage;
            if (IsDead)
                ShouldBeRemoved = true;
        }

        public bool IsDead
        {
            get { return mCurrentHealth <= 0; }
        }
        public int CurrentHealth
        {
            get { return mCurrentHealth; }
        }
    }
}
