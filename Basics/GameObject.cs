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
        public Vector2 Position { get; set; }
        public float Speed { get; set; }
        public bool ShouldBeRemoved { get; set; }
        public float Rotation => mBody.GetAngle();
        public bool IsBurning { get; set; }
        public Body Body => mBody;

        protected World mWorld;
        protected Fixture mShape;
        protected Body mBody;
        private double mBurnDelay = 3.0d;
        private int mBurnCounter = 0;
        private double mBurnTimer = 3.0d;

        public GameObject(World world, Vector2 position, Texture2D texture)
        {
            ShouldBeRemoved = false;
            Speed = 3;
            Position = position;
            mSprite = texture;
            mWorld = world;
        }

        public void CreatePhysicsRepresentation(float mass, float density, float damping, float friction, float restitution, bool isBullet, bool isSensor)
        {
            // CREATING A DYNAMIC BODY
            BodyDef dynamicBodyDef = new BodyDef()
            {
                MassData = new MassData()
                {
                    Center = Constants.UnitCenter, 
                    Mass = mass
                },
                Position = new Vec2(Position.X / Constants.PixelPerMeter,Position.Y / Constants.PixelPerMeter),
                IsBullet = isBullet,
                LinearDamping = damping
            };
            
            mBody = mWorld.CreateBody(dynamicBodyDef);
            mBody.SetUserData(this);
            FixtureDef shapeDef = new CircleDef()
            {
                Density = density,
                Friction = friction,
                Type = ShapeType.CircleShape,
                Radius = 0.5f,
                IsSensor = isSensor,
                Restitution = restitution
            };
            mShape = mBody.CreateFixture(shapeDef);
            mShape.UserData = this;
        }

        public void ApplyImpulse(Vector2 change)
        {
            mBody.ApplyImpulse(change.ToVec2(), mBody.GetWorldCenter());
        }

        public void SetVelocity(Vec2 velocity)
        {
            mBody.SetLinearVelocity(velocity);
        }
        internal void ApplyForce(Vector2 change)
        {
            mBody.ApplyForce(change.ToVec2(), mBody.GetWorldCenter());
        }

        public virtual void Update(GameTime gameTime, GameContext gameContext)
        {
            Position = new Vector2(mBody.GetPosition().X * Constants.PixelPerMeter, mBody.GetPosition().Y * Constants.PixelPerMeter);

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
            spriteBatch.Draw(mSprite, Position, null, drawColor, mBody.GetAngle(), mSprite.Bounds.Center.ToVector2(), Vector2.One, SpriteEffects.None, 0.0f);
        }
        public bool IsAtPosition(Vector2 position, float tolerance = 1.5f)
        {
            return Vector2.Distance(Position, position) <= tolerance;
        }
        protected bool IsNextTo(GameObject other)
        {
            return Vector2.Distance(Position, other.Position) <= Constants.PixelPerMeter * 1.5f;
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
    }
}
