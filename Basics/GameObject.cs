using System;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using ChainTrapper.Basics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Math = System.Math;

namespace ChainTrapper
{
    public enum StatusEffects
    {
        Burning,
        Frozen,
        Poisoned
    }
    public class GameObject
    {
        protected Texture2D mSprite;
        public Vector2 Position { get; set; }
        public float Speed { get; set; }
        public bool ShouldBeRemoved { get; set; }
        protected World mWorld;
        protected Fixture mShape;
        protected Body mBody;
        
        public GameObject(World world, Vector2 position, Texture2D texture, float friction = 0.3f, bool isBullet = false, bool isSensor = false)
        {
            ShouldBeRemoved = false;
            Speed = 3;
            Position = position;
            mSprite = texture;
            mWorld = world;
            CreatePhysicsRepresentation(friction, isBullet, isSensor);
        }

        private void CreatePhysicsRepresentation(float friction, bool isBullet, bool isSensor)
        {
            // CREATING A DYNAMIC BODY
            BodyDef dynamicBodyDef = new BodyDef()
            {
                MassData = new MassData()
                {
                    Center = Constants.UnitCenter, 
                    Mass = 1.0f
                },
                Position = new Vec2(Position.X / Constants.PixelPerMeter,Position.Y / Constants.PixelPerMeter),
                IsBullet = isBullet,
                LinearDamping = 0.8f
            };
            
            mBody = mWorld.CreateBody(dynamicBodyDef);
            mBody.SetUserData(this);
            FixtureDef shapeDef = new CircleDef()
            {
                Density = 1.0f,
                Friction = friction,
                Type = ShapeType.CircleShape,
                Radius = 0.5f,
                IsSensor = isSensor
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

        public virtual void Update(GameTime gameTime, Context context)
        {
            Position = new Vector2(mBody.GetPosition().X * Constants.PixelPerMeter, mBody.GetPosition().Y * Constants.PixelPerMeter);
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mSprite, Position, null, Color.White, mBody.GetAngle(), mSprite.Bounds.Center.ToVector2(), Vector2.One, SpriteEffects.None, 0.0f);
        }
        public bool IsAtPosition(Vector2 position)
        {
            return Vector2.Distance(Position, position) <= 1.5f;
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
