﻿using System;
using System.Collections.Generic;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using ChainTrapper.Basics;
using ChainTrapper.Traits;
using ChainTrapper.Traps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;

namespace ChainTrapper
{
    public enum TrapType { TimedBomb, RemoteBomb, Fire }

    public class ChainTrapperGame : Game, ContactListener
    {
        public static int ScreenWidth { get; private set; }
        public static int ScreenHeight { get; private set; }
        private Context mContext = new Context();

        private Dictionary<string, string> mDebugInfo = new Dictionary<string, string>();
        private GraphicsDeviceManager mGraphics;
        private SpriteBatch mSpriteBatch;
        private Player mPlayer;
        private SpriteFont mFont;
        private List<Enemy> mEnemies = new List<Enemy>();

        private List<GameObject> mAllGameObjects => mContext.AllGameObjects;
        private KeyboardState mOldKeyboardState;
        private MouseState mOldMouseState;
        private Vec2 mOldInput;
        
        private Texture2D mTrapTexture;

        private HashSet<ITriggered> mTrapsToTrigger = new HashSet<ITriggered>();

        private TrapType mSelectedTraps = TrapType.TimedBomb;
        private Texture2D mArrowTexture;
        private World mPhysicsWorld;

        private RenderTarget2D mUpperEdgeTexture;
        private Fixture mUpperFixture;

        public ChainTrapperGame()
        {
            mGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1.0d / 60.0d);
            ScreenWidth = mGraphics.PreferredBackBufferWidth;
            ScreenHeight = mGraphics.PreferredBackBufferHeight;

            mContext.AllGameObjects = new List<GameObject>();
            mOldKeyboardState = Keyboard.GetState();
        }

        protected override void Initialize()
        {
            CreatePhysicsWorld();
            base.Initialize();
        }

        private void CreatePhysicsWorld()
        {
            // WORLD CREATION
            // 32 pixel = 1 meter
            AABB worldBounds = new AABB()
            {
                UpperBound = new Vec2(ScreenWidth / (float)Constants.PixelPerMeter, ScreenHeight / (float)Constants.PixelPerMeter),
                LowerBound = new Vec2(0,0)
            };
            Vec2 gravity = Vec2.Zero;
            bool sleep = true;
            mPhysicsWorld = new World(worldBounds, gravity, sleep);
            
            mPhysicsWorld.SetContactListener(this);
            mPhysicsWorld.SetContactFilter(new CollisionFilter());
            
            BodyDef upperBound = new BodyDef() {Position = Vec2.Zero, FixedRotation = true};
            Body upperBody = mPhysicsWorld.CreateBody(upperBound);
            FixtureDef upperFixtureDef = new EdgeDef()
            {
                Type = ShapeType.EdgeShape, 
                Vertex1 = Vec2.Zero, 
                Vertex2 = new Vec2(ScreenWidth / (float)Constants.PixelPerMeter, 0),
                Restitution = 0.6f
            };
            mUpperFixture = upperBody.CreateFixture(upperFixtureDef);
            //upperFixture.UserData = mUpperEdgeTexture;
            
            BodyDef lowerBound = new BodyDef() {Position = Vec2.Zero, FixedRotation = true};
            Body lowerBody = mPhysicsWorld.CreateBody(lowerBound);
            FixtureDef lowerFixtureDef = new EdgeDef()
            {
                Type = ShapeType.EdgeShape, 
                Vertex1 = new Vec2(0, ScreenHeight / (float)Constants.PixelPerMeter),
                Vertex2 = new Vec2(ScreenWidth / (float)Constants.PixelPerMeter, ScreenHeight / (float)Constants.PixelPerMeter),
                Restitution = 0.6f
            };
            lowerBody.CreateFixture(lowerFixtureDef);
            
            BodyDef leftBound = new BodyDef() {Position = Vec2.Zero, FixedRotation = true};
            Body leftBody = mPhysicsWorld.CreateBody(leftBound);
            FixtureDef leftFixtureDef = new EdgeDef()
            {
                Type = ShapeType.EdgeShape, 
                Vertex1 = Vec2.Zero,
                Vertex2 = new Vec2(0, ScreenHeight / (float)Constants.PixelPerMeter),
                Restitution = 0.6f
            };
            leftBody.CreateFixture(leftFixtureDef);
            
            BodyDef rightBound = new BodyDef() {Position = Vec2.Zero, FixedRotation = true};
            Body rightBody = mPhysicsWorld.CreateBody(rightBound);
            FixtureDef rightFixtureDef = new EdgeDef()
            {
                Type = ShapeType.EdgeShape, 
                Vertex1 = new Vec2(ScreenWidth / (float)Constants.PixelPerMeter, 0),
                Vertex2 = new Vec2(ScreenWidth / (float)Constants.PixelPerMeter, ScreenHeight / (float)Constants.PixelPerMeter),
                Restitution = 0.6f
            };
            rightBody.CreateFixture(rightFixtureDef);
        }

        private void PhysicsUpdate()
        {
            // SIMULATION STEPS for each UPDATE
            float timeStep = 1.0f / 60.0f;
            int velocityIterations = 6;
            int positionIterations = 2;
            mPhysicsWorld.Step(timeStep, velocityIterations, positionIterations);
        }

        protected override void LoadContent()
        {
            mSpriteBatch = new SpriteBatch(GraphicsDevice);
            mFont = Content.Load<SpriteFont>("Fonts/Default");

            var unitSize = Constants.PixelPerMeter;
            var texture = new Texture2D(mGraphics.GraphicsDevice, unitSize, unitSize);
            Color[] redPixels = new Color[unitSize * unitSize];
            for (int i = 0; i < unitSize * unitSize; i++)
            {
                redPixels[i] = Color.Red;
            }
            texture.SetData<Color>(redPixels);

            var blueTexture = new Texture2D(mGraphics.GraphicsDevice, unitSize, unitSize);
            Color[] bluePixels = new Color[unitSize * unitSize];
            for (int i = 0; i < unitSize * unitSize; i++)
            {
                bluePixels[i] = Color.BlueViolet;
            }
            blueTexture.SetData<Color>(bluePixels);

            mTrapTexture = new Texture2D(mGraphics.GraphicsDevice, unitSize, unitSize);
            Color[] yellowPixels = new Color[unitSize * unitSize];
            for (int i = 0; i < unitSize * unitSize; i++)
            {
                yellowPixels[i] = Color.Orange;
            }
            mTrapTexture.SetData<Color>(yellowPixels);

            mArrowTexture = new Texture2D(mGraphics.GraphicsDevice, unitSize, 2);
            Color[] blackPixels = new Color[2 * unitSize];
            for (int i = 0; i < 2 * unitSize; i++)
            {
                blackPixels[i] = Color.Black;
            }
            mArrowTexture.SetData<Color>(blackPixels);

            mPlayer = new Player(mPhysicsWorld, new Vector2(100, 100), texture);
            mDebugInfo.Add("Pl_Pos", mPlayer.Position.ToString());
            mDebugInfo.Add("Trap", "");
            mAllGameObjects.Add(mPlayer);

            for (int i = 0; i < 7; i++)
            {
                var enemy = new Enemy(mPhysicsWorld, Helper.RandomPosition(), blueTexture);
                mEnemies.Add(enemy);
                mAllGameObjects.Add(enemy);
            }

            mUpperEdgeTexture = new RenderTarget2D(mGraphics.GraphicsDevice, ScreenWidth, Constants.PixelPerMeter);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            HandleInput();
            
            PhysicsUpdate();
            
            for (int i = mAllGameObjects.Count - 1; i >= 0; i--)
            {
                var gameObject = mAllGameObjects[i];
                gameObject.Update(gameTime, mContext);
                if (gameObject.ShouldBeRemoved)
                {
                    gameObject.Cleanup();
                    mAllGameObjects.RemoveAt(i);
                    // NOTE: Hackish
                    if (gameObject is RemoteBomb && mTrapsToTrigger.Contains((RemoteBomb)gameObject))
                    {
                        mTrapsToTrigger.Remove((RemoteBomb)gameObject);
                    }
                }
            }

            // DumbAndSlowCollisionDetection();

            mDebugInfo["Pl_Pos"] = mPlayer.Position.ToString();
            mDebugInfo["Trap"] = Enum.GetName(typeof(TrapType), mSelectedTraps);

            base.Update(gameTime);
        }

        private void HandleInput()
        {
            Vec2 input = Vec2.Zero;
            var keyState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            if (keyState.IsKeyDown(Keys.A))
            {
                // move left
                input.X -= 1;
            }

            if (keyState.IsKeyDown(Keys.D))
            {
                // move left
                input.X += 1;
            }

            if (keyState.IsKeyDown(Keys.W))
            {
                // move left
                input.Y -= 1;
            }

            if (keyState.IsKeyDown(Keys.S))
            {
                // move left
                input.Y += 1;
            }

            if (keyState.IsKeyDown(Keys.E) && mOldKeyboardState.IsKeyUp(Keys.E))
            {
                SelectNextTrap();
            }

            if (keyState.IsKeyDown(Keys.Space) && mOldKeyboardState.IsKeyUp(Keys.Space))
            {
                PlaceTrap();
            }

            if (keyState.IsKeyDown(Keys.X) && mOldKeyboardState.IsKeyUp(Keys.X))
            {
                ActivateTrigger();
            }

            if (mouseState.LeftButton == ButtonState.Pressed && mOldMouseState.LeftButton == ButtonState.Released)
            {
                FireArrow(mouseState.X, mouseState.Y);
            }

            //mPlayer.ApplyForce(input * 4.8f);

            if (input != Vec2.Zero || mOldInput != Vec2.Zero)
            {
                input.Normalize();
                mPlayer.SetVelocity(input * 7); // kinematic movement, but no forces from the outside applied
            }

            mOldInput = input;
            mOldKeyboardState = keyState;
            mOldMouseState = mouseState;
        }

        private void FireArrow(int x, int y)
        {
            var direction = new Vector2(x - mPlayer.Position.X, y - mPlayer.Position.Y);
            direction.Normalize();

            var arrow = new Projectile(mPhysicsWorld, mPlayer.Position, mArrowTexture);
            arrow.SetRotation(direction);
            //arrow.ApplyForce(direction * 5);
            arrow.ApplyImpulse(direction * 50f);
            mAllGameObjects.Add(arrow);
        }

        private void SelectNextTrap()
        {
            mSelectedTraps = (TrapType)(((int)mSelectedTraps + 1) % Enum.GetNames(typeof(TrapType)).Length);
        }

        private void PlaceTrap()
        {
            GameObject trap = null;
            switch (mSelectedTraps)
            {
                case TrapType.TimedBomb:
                    trap = new TimedBomb(mPhysicsWorld, mPlayer.Position, mTrapTexture);
                    break;
                case TrapType.RemoteBomb:
                    trap = new RemoteBomb(mPhysicsWorld, mPlayer.Position, mTrapTexture);
                    mTrapsToTrigger.Add((RemoteBomb)trap);
                    break;
                case TrapType.Fire:
                    trap = new Fire(mPhysicsWorld, mPlayer.Position, mTrapTexture);
                    break;
            }

            if (trap != null)
                mAllGameObjects.Add(trap);
        }

        private void ActivateTrigger()
        {
            foreach (var trap in mTrapsToTrigger)
            {
                trap.ActivateTrigger(mContext);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            mSpriteBatch.Begin();
            
            mSpriteBatch.Draw(mUpperEdgeTexture, Vector2.Zero, Color.White);
            
            foreach (var gameObject in mAllGameObjects)
            {
                gameObject.Draw(mSpriteBatch);
            }           

            DebugDraw();

            mSpriteBatch.End();

            base.Draw(gameTime);
        }

        private void DebugDraw()
        {
            Vector2 drawPos = Vector2.Zero;
            var lineHeight = mFont.MeasureString("XMI").Y;
            foreach (var kvp in mDebugInfo)
            {
                string line = kvp.Key + ": " + kvp.Value;                
                mSpriteBatch.DrawString(mFont, line, drawPos, Color.Black);
                mSpriteBatch.DrawString(mFont, line, drawPos + Vector2.One, Color.White);
                drawPos += Vector2.UnitY * lineHeight;
            }
        }
        
        
        public void BeginContact(Contact contact)
        {
            GameObject go1 = null, go2 = null;
            
            if (contact.FixtureA.Body.GetUserData() != null)
            {
                go1 = (GameObject) contact.FixtureA.Body.GetUserData();
            }
            
            if (contact.FixtureB.Body.GetUserData() != null)
            {
                go2 = (GameObject) contact.FixtureB.Body.GetUserData();
            }

            if (go1 is Projectile && !(go2 is Player))
            {
                if (contact.FixtureB == mUpperFixture)
                {
                    HitUpperEdge(contact);
                }
                go1.Destroy();
            }

            if (go2 is Projectile && !(go1 is Player))
            {
                if (contact.FixtureA == mUpperFixture)
                {
                    HitUpperEdge(contact);
                }
                go2.Destroy();
            }
        }

        private void HitUpperEdge(Contact contact)
        {
            Color[] pixelData = new Color[mArrowTexture.Width * mArrowTexture.Height];
            mArrowTexture.GetData(pixelData);
            contact.GetWorldManifold(out var worldManifold);
            var firstContact = worldManifold.Points[0] * Constants.PixelPerMeter;
            Debug("Contact", firstContact.X + "/" + firstContact.Y);
            mUpperEdgeTexture.SetData(0, new Rectangle((int) (firstContact.X - 16), 0, 2, 32), pixelData, 0, pixelData.Length);
        }

        private void Debug(string key, string text)
        {
            if (mDebugInfo.ContainsKey(key))
            {
                mDebugInfo[key] = text;
            }
            else
            {
                mDebugInfo.Add(key, text);
            }
        }
        
        public void EndContact(Contact contact)
        {
            
        }

        public void PreSolve(Contact contact, Manifold oldManifold)
        {
            
        }

        public void PostSolve(Contact contact, ContactImpulse impulse)
        {
            
        }
    }

    internal class CollisionFilter : ContactFilter
    {
        public override bool ShouldCollide(Fixture fixtureA, Fixture fixtureB)
        {
            GameObject go1 = null, go2 = null;
            
            if (fixtureA.Body.GetUserData() != null)
            {
                go1 = (GameObject) fixtureA.Body.GetUserData();
            }
            
            if (fixtureB.Body.GetUserData() != null)
            {
                go2 = (GameObject) fixtureB.Body.GetUserData();
            }

            if ((go1 is Player && go2 is Projectile) || (go2 is Player && go1 is Projectile))
            {
                return false;
            }
            
            if ((go1 is Player && go2 is Bomb) || (go2 is Player && go1 is Bomb))
            {
                return false;
            }

            return true;
        }
    }
}
