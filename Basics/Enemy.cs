using System;
using System.Collections.Generic;
using System.Linq;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using ChainTrapper.Globals;
using ChainTrapper.Traits;
using ChainTrapper.Traps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Math = System.Math;

namespace ChainTrapper.Basics
{
    public enum EnemyState { Sleeping, Awake, Chasing, Attacking, Pain, Dying, GoHome }
    public enum EnemyType { Predator, Prey }
    public class Enemy : GameObject, IGameObjectCollisionListener, IWoundable
    {
        private int mPlayerCollisionDamageDelay = 2;
        private int mReactionDelay = Helper.RandomInt(1, 4);
        private double mReactionCounter;
        private EnemyState mState = EnemyState.GoHome;
        private Player mPlayer = null;
        private int mCurrentHealth = 5;
        private Vector2 mHome;
        private bool mCollidingWithPlayer;
        private double mLastDamageToPlayer;
        private Vec2 mLastKnownPlayerPosition;
        private float mBreadCrumbDetectionRadius;
        private float mMaxLookAhead;
        private bool mCanSmellBreadCrumbs;
        private EnemyType mEnemyType;
        private Vec2 mLastStuckPos;
        private double mStuckTime = 0.0d;

        public Enemy(World world, Vector2 drawPosition, Texture2D texture, Player player) : base(world, drawPosition, texture)
        {
            mPlayer = player;
            mHome = drawPosition;
            mReactionCounter = mReactionDelay;
            Speed = 28;
            mBreadCrumbDetectionRadius = 5.0f;
            mMaxLookAhead = 1.0f;
            
            mEnemyType = EnemyType.Predator;
            mCanSmellBreadCrumbs = true;
            
            CreatePhysicsRepresentation(
                5f, 
                3.0f, 
                2f, 
                0.9f, 
                0.0f, 
                false, 
                false
            );
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            var wolfDebug = Enum.GetName(typeof(EnemyState), mState) + " / Stuck: " + mStuckTime.ToString("0.0");
            spriteBatch.DrawString(Globals.Globals.DefaultFont, mCurrentHealth.ToString(), DrawPosition - (Vector2.UnitY * Constants.PixelPerMeter), Color.White);
            spriteBatch.DrawString(Globals.Globals.DefaultFont, wolfDebug, DrawPosition - (Vector2.UnitY * (Constants.PixelPerMeter * 1.5f)), Color.White);
        }

        public void WakeUp()
        {
            mState = EnemyState.Awake;
        }
        public override void Update(GameTime gameTime, GameContext gameContext)
        {
            if (mPlayer == null) return;
            
            base.Update(gameTime, gameContext);

            if (IsAtPosition(mLastStuckPos))
            {
                mStuckTime += gameTime.ElapsedGameTime.TotalSeconds;
                if (mStuckTime >= 5.0f && mState == EnemyState.Chasing)
                {
                    mState = EnemyState.GoHome;
                }
            }
            else
            {
                mStuckTime = 0;
                mLastStuckPos = Position;
            }
            
            CheckForPlayerCollisionDamage(gameTime);
            
            switch (mState)
            {
                case EnemyState.Sleeping:
                    return;
                case EnemyState.Awake:
                    UpdateReactionCounter(gameTime, gameContext);
                    break;
                case EnemyState.Chasing:
                    ChasePlayer();
                    break;
                case EnemyState.Attacking:
                    UpdateReactionCounter(gameTime, gameContext);
                    break;
                case EnemyState.GoHome:
                    GoHome();
                    UpdateReactionCounter(gameTime, gameContext);
                    break;
            }
        }

        private void CheckForPlayerCollisionDamage(GameTime gameTime)
        {
            if (mPlayer != null && mCollidingWithPlayer && (gameTime.TotalGameTime.TotalSeconds - mLastDamageToPlayer) >= mPlayerCollisionDamageDelay)
            {
                mPlayer.TakeDamage(1);
                mLastDamageToPlayer = gameTime.TotalGameTime.TotalSeconds;
            }
        }

        private void GoHome()
        {
            if (!IsAtPosition(mHome))
            {
                var desiredDirection = (mHome - DrawPosition).ToPhysics();
                var dist = desiredDirection.Length();
                desiredDirection.Normalize();
                var speed = Speed;
                if (dist <= 3.0f)
                {
                    speed = (dist / 3.0f) * Speed;
                }

                Vec2 avoidanceForce = LookForward(desiredDirection);

                desiredDirection += avoidanceForce;
                ApplySteering(desiredDirection * speed);
            }
            else
            {
                mStuckTime = 0.0f;
            }
        }

        private void AttackPlayer()
        {
            if (mPlayer != null && !mPlayer.IsDead && IsNextTo(mPlayer))
            {
                mPlayer.TakeDamage(1);
                mState = EnemyState.Chasing;
            }
            else 
            {
                mState = EnemyState.Awake;
            }
        }

        private void ChasePlayer()
        {
            if (mPlayer == null || mPlayer.IsDead)
            {
                mPlayer = null;
                mState = EnemyState.Awake;
                return;
            }

            if (IsNextTo(mPlayer))
            {
                mState = EnemyState.Attacking;
                mLastKnownPlayerPosition = mPlayer.Position;
                return;
            }

            if (CanSeeGameObject(mPlayer))
            {
                mLastKnownPlayerPosition = mPlayer.Position;
            }
            else if (mCanSmellBreadCrumbs)
            {
                LookForBreadCrumb();
            }
            
            var desiredDirection = mLastKnownPlayerPosition - Position;
            
            if (mEnemyType == EnemyType.Prey)
            {
                desiredDirection *= -1;
            }
            
            var avoidanceForce = LookForward(desiredDirection);

            desiredDirection.Normalize();
            
            desiredDirection += avoidanceForce;
            
            var desiredVelocity = desiredDirection * Speed;
         
            if (Globals.Globals.DebugEnabled)
            {
                Globals.Globals.DebugDrawer.DrawVectorAsArrow(Position, desiredDirection, new Box2DX.Dynamics.Color(0.2f, 0.2f, 1.0f));
                Globals.Globals.DebugDrawer.DrawVectorAsArrow(Position, avoidanceForce, new Box2DX.Dynamics.Color(0.2f, 0.2f, 1.0f));
                Globals.Globals.DebugDrawer.DrawVectorAsArrow(Position, desiredVelocity, new Box2DX.Dynamics.Color(0.2f, 1.0f, 0.2f));
                Globals.Globals.DebugDrawer.DrawCircle(mLastKnownPlayerPosition, 0.3f, new Box2DX.Dynamics.Color(0.4f, 0.4f, 1.0f));
            }

            ApplySteering(desiredVelocity);
        }

        private void LookForBreadCrumb()
        {
            List<BreadCrumb> detectedCrumbs = new List<BreadCrumb>();
            
            Fixture[] fixtures = new Fixture[100];
            int count = mWorld.Query(Position, mBreadCrumbDetectionRadius, fixtures);
            
            for (int i = 0; i < count; i++)
            {
                var fixture = fixtures[i];
                if (fixture.UserData is BreadCrumb breadCrumb)
                {
                    if (CanSeeGameObject(breadCrumb))
                    {
                        detectedCrumbs.Add(breadCrumb);
                    }
                }
            }

            if (detectedCrumbs.Count > 0)
            {
                detectedCrumbs.Sort((bc1, bc2) => bc2.LifeTimeLeft.CompareTo(bc1.LifeTimeLeft));
                var mostRecentBreadCrumb = detectedCrumbs[0];
                var lastKnownPlayerPosition = mostRecentBreadCrumb.Position;
                if (Globals.Globals.DebugEnabled)
                    Globals.Globals.DebugDrawer.DrawPoint(lastKnownPlayerPosition,
                    new Box2DX.Dynamics.Color(1.0f, 0.3f, 0.3f));
                mLastKnownPlayerPosition = lastKnownPlayerPosition;
                mState = EnemyState.Chasing;
            }
        }
        
        private void UpdateReactionCounter(GameTime gameTime, GameContext gameContext)
        {
            if (mReactionCounter > 0.0f)
            {
                mReactionCounter -= gameTime.ElapsedGameTime.TotalSeconds;
                if (mReactionCounter <= 0.0f)
                {
                    if (mState == EnemyState.Awake || mState == EnemyState.GoHome)
                    {
                        if (CanSeeGameObject(mPlayer))
                        {
                            mState = EnemyState.Chasing;
                            mLastKnownPlayerPosition = mPlayer.Position;
                        }
                        else if (mCanSmellBreadCrumbs)
                        {
                            LookForBreadCrumb();
                        }
                    }
                    else if (mState == EnemyState.Attacking)
                    {
                        AttackPlayer();
                    }
                    mReactionCounter = mReactionDelay;
                }
            }
        }

        private Vec2 LookForward(Vec2 forward)
        {
            Vec2 avoidanceForce = Vec2.Zero;

            var orthogonalPlaneVec = forward.RotatedLeft();
            
            var leftSensorDirection = (forward + orthogonalPlaneVec);
            var rightSensorDirection = (forward - orthogonalPlaneVec);
            
            forward.SetMagnitude(mMaxLookAhead);
            leftSensorDirection.SetMagnitude(mMaxLookAhead);
            rightSensorDirection.SetMagnitude(mMaxLookAhead);

            if (Globals.Globals.DebugEnabled)
            {
                Globals.Globals.DebugDrawer.DrawVectorAsArrow(Position, leftSensorDirection,
                    new Box2DX.Dynamics.Color(1.0f, 1.0f, 1.0f));
                Globals.Globals.DebugDrawer.DrawVectorAsArrow(Position, rightSensorDirection,
                    new Box2DX.Dynamics.Color(1.0f, 1.0f, 1.0f));
                Globals.Globals.DebugDrawer.DrawVectorAsArrow(Position, forward,
                    new Box2DX.Dynamics.Color(1.0f, 1.0f, 1.0f));
            }

            var lookAheadEndPoint = Position + forward;
            var leftEndPoint = Position + leftSensorDirection;
            var rightEndPoint = Position + rightSensorDirection;
            
            var fixtures = mWorld.Raycast(Position, lookAheadEndPoint);
            fixtures = fixtures.Concat(mWorld.Raycast(Position, leftEndPoint));
            fixtures = fixtures.Concat(mWorld.Raycast(Position, rightEndPoint));
            
            foreach (var fixture in fixtures)
            {
                if (fixture == mShape) continue;
                if (fixture.UserData is Wall || fixture.ShapeType == ShapeType.EdgeShape)
                {
                    var nearestPoint = fixture.GetNearestPoint(Position);
                    
                    var curAvoidanceForce = Position - nearestPoint;
                    
                    float dist = curAvoidanceForce.Length();
                    curAvoidanceForce.SetMagnitude(1 / (dist + 1));
                    
                    avoidanceForce += curAvoidanceForce;
                    if (Globals.Globals.DebugEnabled)
                        Globals.Globals.DebugDrawer.DrawVectorAsArrow(nearestPoint, curAvoidanceForce,
                        new Box2DX.Dynamics.Color(1.0f, 0.2f, 0.2f));
                }
            }

            return avoidanceForce;
        }

        public void OnCollisionBegin(GameContext gameContext, GameObject go)
        {
            if (go is Player player)
            {
                mCollidingWithPlayer = true;
            }
        }

        public void OnCollisionEnd(GameContext gameContext, GameObject go)
        {
            if (go is Player player)
            {
                mCollidingWithPlayer = false;
            }
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
