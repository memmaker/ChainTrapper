using System;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using ChainTrapper.Basics;
using ChainTrapper.Globals;
using ChainTrapper.Traits;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using IDrawable = ChainTrapper.Traits.IDrawable;
using Math = System.Math;

namespace ChainTrapper
{
    public enum StatusEffects
    {
        Burning,
        Frozen,
        Poisoned
    }
    public class GameObject : IDrawable
    {
        protected Texture2D mSprite;
        public Vector2 DrawPosition { get; set; }
        public Vec2 Position => mBody.GetPosition();
        public float MaxSpeed { get; set; }
        public bool ShouldBeRemoved { get; set; }
        public float Rotation => mBody.GetAngle();
        public Vec2 Velocity => mBody.GetLinearVelocity();
        public bool IsBurning { get; set; }
        public Body Body => mBody;

        public Grass Grass => mGrass;

        protected World mWorld;
        protected Fixture mShape;
        protected Body mBody;
        private double mBurnDelay = 3.0d;
        private int mBurnCounter = 0;
        private double mBurnTimer = 3.0d;
        protected float mBurnSpeedFactor = 0.7f;
        private Grass mGrass;

        public GameObject(World world, Vector2 drawPosition, Texture2D texture)
        {
            ShouldBeRemoved = false;
            MaxSpeed = 3;
            DrawPosition = drawPosition;
            mSprite = texture;
            mWorld = world;
        }

        public void CreatePhysicsRepresentation(float mass, float density, float damping, float friction, float restitution, bool isBullet, bool isSensor, float radius = 0.5f)
        {
            // CREATING A DYNAMIC BODY
            BodyDef dynamicBodyDef = new BodyDef()
            {
                MassData = new MassData()
                {
                    Center = Constants.UnitCenter, 
                    Mass = mass
                },
                Position = new Vec2(DrawPosition.X / Constants.PixelPerMeter,DrawPosition.Y / Constants.PixelPerMeter),
                IsBullet = isBullet,
                LinearDamping = damping,
            };
            
            mBody = mWorld.CreateBody(dynamicBodyDef);
            mBody.SetUserData(this);
            FixtureDef shapeDef = new CircleDef()
            {
                Density = density,
                Friction = friction,
                Type = ShapeType.CircleShape,
                Radius = radius,
                IsSensor = isSensor,
                Restitution = restitution,
                //LocalPosition = new Vec2(0, radius/2.0f)
            };
            mShape = mBody.CreateFixture(shapeDef);
            mShape.UserData = this;
        }

        public void ApplyImpulse(Vector2 change)
        {
            mBody.ApplyImpulse(change.ToVec2(), mBody.GetWorldCenter());
        }

        internal void ApplyForce(Vector2 change)
        {
            mBody.ApplyForce(change.ToVec2(), mBody.GetWorldCenter());
        }
        
        internal void ApplyForce(Vec2 change)
        {
            mBody.ApplyForce(change, mBody.GetWorldCenter());
        }

        public virtual void Update(GameTime gameTime, GameContext gameContext)
        {
            mBody.SetAngle((float) Helper.AngleFromVector(Velocity));
            DrawPosition = Position.ToScreen();
            ApplyBurning(gameTime);
        }

        private void ApplyBurning(GameTime gameTime)
        {
            if (IsBurning && this is IWoundable && mBurnTimer > 0.0f)
            {
                mBurnTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                if (mBurnTimer <= 0.0f)
                {
                    var woundable = (IWoundable) this;
                    woundable.TakeDamage(1);
                    mBurnCounter++;
                    mBurnTimer = mBurnDelay;

                    if (mBurnCounter == 4)
                    {
                        IsBurning = false;
                        mBurnCounter = 0;
                    }
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Color drawColor = Color.White;
            if (IsBurning)
            {
                drawColor = Helper.Percent(50) ? Color.OrangeRed : Color.Red;
            }

            spriteBatch.Draw(mSprite, DrawPosition, null, drawColor, 0.0f, mSprite.Bounds.Center.ToVector2(), Vector2.One, SpriteEffects.None, 0.0f);
        }
        public bool IsAtPosition(Vector2 position, float tolerance = 1.5f)
        {
            return Vector2.Distance(DrawPosition, position) <= tolerance;
        }
        
        public bool IsAtPosition(Vec2 position, float tolerance = 0.5f)
        {
            return Vec2.Distance(Position, position) <= tolerance;
        }
        protected bool IsNextTo(GameObject other)
        {
            return Vector2.Distance(DrawPosition, other.DrawPosition) <= Constants.PixelPerMeter * 1.5f;
        }
        public void Cleanup()
        {
            mBody.DestroyFixture(mShape);
            mWorld.DestroyBody(mBody);
        }

        public void Destroy()
        {
            this.ShouldBeRemoved = true;
        }

        public void SetRotation(Vector2 direction)
        {
            float angle = (float) Math.Atan2(direction.Y, direction.X);
            mBody.SetFixedRotation(false);
            mBody.SetAngle(angle);
        }

        protected void ApplySteering(Vec2 desiredVelocity)
        {
            var steeringForce = desiredVelocity - mBody.GetLinearVelocity();
            var maxSteeringForceforce = 64f;
            if (steeringForce.Length() > maxSteeringForceforce)
            {
                steeringForce.Normalize();
                steeringForce *= maxSteeringForceforce;
            }
            Globals.Globals.DebugDrawer.DrawVectorAsArrow(mBody.GetPosition(), steeringForce, new Box2DX.Dynamics.Color(1.0f, 1.0f, 1.0f));
            mBody.ApplyForce(steeringForce, mBody.GetWorldCenter());
        }

        protected bool CanSeeGameObject(GameObject go)
        {
            var hitFixtures = mWorld.Raycast(DrawPosition, go.DrawPosition);

            foreach (var hitFixture in hitFixtures)
            {
                if (hitFixture == mShape) continue;
                
                var userData = hitFixture.UserData;
                if (userData is Wall) return false;
                if (userData is GameObject goFound && go == goFound)
                {
                    if (goFound is Player player)
                    {
                        if (player.IsHidingInGrass && player.Grass != Grass)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }

            return false;
        }

        public void SetGrass(Grass grass)
        {
            mGrass = grass;
        }
    }
}
