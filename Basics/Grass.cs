using System.Collections.Generic;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using ChainTrapper.Globals;
using ChainTrapper.Traps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using IDrawable = ChainTrapper.Traits.IDrawable;

namespace ChainTrapper.Basics
{
    public class Grass : IDrawable, IGameObjectCollisionListener
    {
        private World mWorld;
        private Texture2D mTexture;
        private Body mBody;
        private Fixture mShape;
        public Vector2 Position { get; set; }
        public HashSet<GameObject> Occupants { get; } 

        public Grass(World world, Vector2 position, Texture2D texture)
        {
            Occupants = new HashSet<GameObject>();
            mWorld = world;
            Position = position;
            mTexture = texture;
            CreatePhysicsRepresentation();
        }
        
        private void CreatePhysicsRepresentation()
        {
            // CREATING A STATIC BODY
            BodyDef dynamicBodyDef = new BodyDef()
            {
                Position = new Vec2(Position.X / Constants.PixelPerMeter,Position.Y / Constants.PixelPerMeter),
                IsBullet = false,
                FixedRotation = true,
            };
            
            mBody = mWorld.CreateBody(dynamicBodyDef);
            mBody.SetUserData(this);
            
            PolygonDef shapeDef = new PolygonDef()
            {
                Type = ShapeType.PolygonShape,
                IsSensor = true,
            };
            shapeDef.SetAsBox(mTexture.Width / (float)Constants.PixelPerMeter / 2, mTexture.Height / (float)Constants.PixelPerMeter / 2);
            mShape = mBody.CreateFixture(shapeDef);
            mShape.UserData = this;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Position = mBody.GetPosition().ToScreen();
            spriteBatch.Draw(mTexture, Position, null, Color.Green, mBody.GetAngle(), mTexture.Bounds.Center.ToVector2(), Vector2.One, SpriteEffects.None, 1.0f);
        }

        public void OnCollisionBegin(GameContext gameContext, GameObject go)
        {
            go.SetGrass(this);
            Occupants.Add(go);
        }

        public void OnCollisionEnd(GameContext gameContext, GameObject go)
        {
            if (go.Grass == this)
            {
                go.SetGrass(null);
            }
            Occupants.Remove(go);
        }
    }
}