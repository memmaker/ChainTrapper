using Box2DX.Collision;
using Box2DX.Dynamics;
using ChainTrapper.Globals;
using ChainTrapper.Traps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChainTrapper.Basics
{
    public enum WolfState { Sleeping, Awake, Chasing, Attacking, Pain, Dying}
    public class Wolf : GameObject, IVictimCollisionListener
    {
        private int mReactionDelay = Helper.RandomInt(1, 4);
        private double mReactionCounter;
        private WolfState mState = WolfState.Awake;
        private Sheep mDesiredSheep = null;
        
        public Wolf(World world, Vector2 position, Texture2D texture) : base(world, position, texture)
        {
            mReactionCounter = mReactionDelay;
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
            if (mDesiredSheep == null || mDesiredSheep.IsDead)
            {
                mDesiredSheep = null;
                mState = WolfState.Awake;
                return;
            }
            
            mDesiredSheep.TakeDamage(1);
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
            
            ApplyForce(desiredDirection);
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
            foreach (var gameObject in gameContext.AllGameObjects)
            {
                if (gameObject is Sheep)
                {
                    Fixture[] hitFixtures;
                    mWorld.Raycast(
                        new Segment()
                        {
                            P1 = mBody.GetPosition(),
                            P2 = gameObject.Body.GetPosition()
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
                        if (firstVisibleGo is Sheep sheep)
                        {
                            mDesiredSheep = sheep;
                            mState = WolfState.Chasing;
                            return;
                        }
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
    }
}
