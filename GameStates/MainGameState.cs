using System;
using System.Collections.Generic;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using ChainTrapper.Basics;
using ChainTrapper.Globals;
using ChainTrapper.Physics;
using ChainTrapper.Traps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using IDrawable = ChainTrapper.Traits.IDrawable;

namespace ChainTrapper.GameStates
{
    public enum TrapType { FireTrap, SpikedHole, ExplodingBarrel }
    public class MainGameState : IGameState
    {
        private GameContext mGameContext = new GameContext();
        private Input mInput = new Input();

        private World PhysicsWorld => mGameContext.PhysicsWorld;
        
        private Player mPlayer;
        
        private List<Wolf> mWolves = new List<Wolf>();

        private List<GameObject> mAllGameObjects => mGameContext.AllGameObjects;
        private List<IDrawable> mDrawables = new List<IDrawable>();

        private TrapType mSelectedTraps = TrapType.FireTrap;

        private Texture2D mArrowTexture;
        private Texture2D mWallTexture;
        private Texture2D mTrapTexture;

        private DebugDrawer mDebugDrawer;
        private Dictionary<string, string> mDebugInfo = new Dictionary<string, string>();
        private SpriteBatch mSpriteBatch;
        private GraphicsDeviceManager mGraphics;
        private ContentManager mContent;
        private StateManager mStateManager;
        private Map mCurrentMap;

        public MainGameState(StateContext context)
        {
            mSpriteBatch = context.SpriteBatch;
            mGraphics = context.GraphicsDeviceManager;
            mContent = context.ContentManager;
            mStateManager = context.StateManager;
        }

        public void OnBegin()
        {
            Initialization();
            CreatePhysicsWorld();
            LoadMap("Maps/map-test.bin");
        }

        private void Initialization()
        {
            mGameContext.AllGameObjects = new List<GameObject>();
            mGameContext.QueuedActions = new Queue<Action>();
    
            mInput.Movement += MovePlayer;
            mInput.NextWeapon += SelectNextTrap;
            mInput.Fire += PlaceTrap;
            mInput.BeginSheepWalk += BeginSheepWalk;
        }

        private void BeginSheepWalk()
        {
            mAllGameObjects.FindAll(go => go is Sheep).ForEach(sheep => ((Sheep)sheep).StartWalk());
        }

        private void MovePlayer(Vec2 direction)
        {
            mPlayer.SetVelocity(direction * 7);
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
                case TrapType.ExplodingBarrel:
                    trap = new TimedBomb(PhysicsWorld, mPlayer.Position, mTrapTexture);
                    break;
                case TrapType.FireTrap:
                    trap = new FireTrap(PhysicsWorld, mPlayer.Position, mTrapTexture, mTrapTexture);
                    break;
                case TrapType.SpikedHole:
                    trap = new SpikedHole(PhysicsWorld, mPlayer.Position, mTrapTexture);
                    break;
            }

            if (trap != null)
                mAllGameObjects.Add(trap);
        }


        private void CreatePhysicsWorld()
        {
            // WORLD CREATION
            // 32 pixel = 1 meter
            var restitution = 0.6f;
            
            AABB worldBounds = new AABB()
            {
                UpperBound = new Vec2(Globals.Globals.ScreenWidth / (float)Constants.PixelPerMeter, Globals.Globals.ScreenHeight / (float)Constants.PixelPerMeter),
                LowerBound = new Vec2(0,0)
            };
            Vec2 gravity = Vec2.Zero;
            bool sleep = true;
            mGameContext.PhysicsWorld = new World(worldBounds, gravity, sleep);
            
            PhysicsWorld.SetContactListener(new CollisionHandler(mGameContext));
            PhysicsWorld.SetContactFilter(new CollisionFilter());
            
            BodyDef upperBound = new BodyDef() {Position = Vec2.Zero, FixedRotation = true};
            Body upperBody = PhysicsWorld.CreateBody(upperBound);
            
            FixtureDef upperFixtureDef = new EdgeDef()
            {
                Type = ShapeType.EdgeShape, 
                Vertex1 = Vec2.Zero, 
                Vertex2 = new Vec2(Globals.Globals.ScreenWidth / (float)Constants.PixelPerMeter, 0),
                Restitution = restitution
            };
            upperBody.CreateFixture(upperFixtureDef);
            
            BodyDef lowerBound = new BodyDef() {Position = Vec2.Zero, FixedRotation = true};
            Body lowerBody = PhysicsWorld.CreateBody(lowerBound);
            FixtureDef lowerFixtureDef = new EdgeDef()
            {
                Type = ShapeType.EdgeShape, 
                Vertex1 = new Vec2(0, Globals.Globals.ScreenHeight / (float)Constants.PixelPerMeter),
                Vertex2 = new Vec2(Globals.Globals.ScreenWidth / (float)Constants.PixelPerMeter, Globals.Globals.ScreenHeight / (float)Constants.PixelPerMeter),
                Restitution = restitution
            };
            lowerBody.CreateFixture(lowerFixtureDef);
            
            BodyDef leftBound = new BodyDef() {Position = Vec2.Zero, FixedRotation = true};
            Body leftBody = PhysicsWorld.CreateBody(leftBound);
            FixtureDef leftFixtureDef = new EdgeDef()
            {
                Type = ShapeType.EdgeShape, 
                Vertex1 = Vec2.Zero,
                Vertex2 = new Vec2(0, Globals.Globals.ScreenHeight / (float)Constants.PixelPerMeter),
                Restitution = restitution
            };
            leftBody.CreateFixture(leftFixtureDef);
            
            BodyDef rightBound = new BodyDef() {Position = Vec2.Zero, FixedRotation = true};
            Body rightBody = PhysicsWorld.CreateBody(rightBound);
            FixtureDef rightFixtureDef = new EdgeDef()
            {
                Type = ShapeType.EdgeShape, 
                Vertex1 = new Vec2(Globals.Globals.ScreenWidth / (float)Constants.PixelPerMeter, 0),
                Vertex2 = new Vec2(Globals.Globals.ScreenWidth / (float)Constants.PixelPerMeter, Globals.Globals.ScreenHeight / (float)Constants.PixelPerMeter),
                Restitution = restitution
            };
            rightBody.CreateFixture(rightFixtureDef);
            
            EnablePhysicsDebugDrawer();
        }

        private void EnablePhysicsDebugDrawer()
        {
            mDebugDrawer = new DebugDrawer(mGraphics.GraphicsDevice);
            mDebugDrawer.Flags = Box2DX.Dynamics.DebugDraw.DrawFlags.Shape;
            PhysicsWorld.SetDebugDraw(mDebugDrawer);
        }

        private void LoadMap(string filename)
        {
            mCurrentMap = Map.FromFile(filename);
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
            
            var whiteTexture = new Texture2D(mGraphics.GraphicsDevice, unitSize, unitSize);
            Color[] whitePixels = new Color[unitSize * unitSize];
            for (int i = 0; i < unitSize * unitSize; i++)
            {
                whitePixels[i] = Color.White;
            }
            whiteTexture.SetData<Color>(whitePixels);

            mTrapTexture = new Texture2D(mGraphics.GraphicsDevice, unitSize, unitSize);
            Color[] yellowPixels = new Color[unitSize * unitSize];
            for (int i = 0; i < unitSize * unitSize; i++)
            {
                yellowPixels[i] = Color.Orange;
            }
            mTrapTexture.SetData<Color>(yellowPixels);

            mArrowTexture = new Texture2D(mGraphics.GraphicsDevice, unitSize, unitSize);
            Color[] blackPixels = new Color[unitSize * unitSize];
            for (int x = 0; x < unitSize; x++)
            {
                for (int y = 0; y < unitSize; y++)
                {
                    int i = y * unitSize + x;
                    var color = (y == unitSize / 2) || (y == unitSize / 2 - 1) ? Color.Black : Color.Transparent;
                    blackPixels[i] = color;
                }
            }
            mArrowTexture.SetData<Color>(blackPixels);
            
            mPlayer = new Player(PhysicsWorld, new Vector2(100, 100), texture);
            mDebugInfo.Add("Pl_Pos", mPlayer.Position.ToString());
            mDebugInfo.Add("Trap", "");
            mAllGameObjects.Add(mPlayer);

            foreach (var wall in mCurrentMap.Walls)
            {
                var wallTexture = new Texture2D(mGraphics.GraphicsDevice, wall.Width, wall.Height);
                Color[] grayPixels = new Color[wall.Width * wall.Height];
                for (int i = 0; i < wall.Width * wall.Height; i++)
                {
                    grayPixels[i] = Color.Gray;
                }
                wallTexture.SetData<Color>(grayPixels);

                var oneWall = new Wall(PhysicsWorld, wall.Center.ToVector2(), wallTexture);
                mDrawables.Add(oneWall);
            }

            foreach (var wolfSpawn in mCurrentMap.WolfSpawns)
            {
                var enemy = new Wolf(PhysicsWorld, wolfSpawn, blueTexture);
                mWolves.Add(enemy);
                mAllGameObjects.Add(enemy);
            }
            
            for (int i = 0; i < 7; i++)
            {
                var sheep = new Sheep(PhysicsWorld, mCurrentMap.SheepPath[0], whiteTexture);
                sheep.Path = mCurrentMap.SheepPath;
                mAllGameObjects.Add(sheep);
            }
        }
        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                mStateManager.Pop();
            
            mInput.HandleInput(gameTime);
            
            PhysicsUpdate();

            while (mGameContext.QueuedActions.TryDequeue(out var queuedAction))
            {
                queuedAction.Invoke();
            }
            
            for (int i = mAllGameObjects.Count - 1; i >= 0; i--)
            {
                var gameObject = mAllGameObjects[i];
                gameObject.Update(gameTime, mGameContext);
                if (gameObject.ShouldBeRemoved)
                {
                    gameObject.Cleanup();
                    mAllGameObjects.RemoveAt(i);
                }
            }
      
            mDebugInfo["Pl_Pos"] = mPlayer.Position.ToString();
            mDebugInfo["Trap"] = Enum.GetName(typeof(TrapType), mSelectedTraps);
        }
        private void PhysicsUpdate()
        {
            // SIMULATION STEPS for each UPDATE
            float timeStep = 1.0f / 60.0f;
            int velocityIterations = 6;
            int positionIterations = 2;
            PhysicsWorld.Step(timeStep, velocityIterations, positionIterations);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.Clear(Color.CornflowerBlue);

            mSpriteBatch.Begin();

            // Things that don't get updated, eg. walls
            foreach (var drawable in mDrawables)
            {
                drawable.Draw(mSpriteBatch);
            }
            
            foreach (var gameObject in mAllGameObjects)
            {
                gameObject.Draw(mSpriteBatch);
            }           

            // text
            DebugDraw();
            
            // physics
            mDebugDrawer.FlushDrawing();
            
            mSpriteBatch.End();
        }
        
        private void DebugDraw()
        {
            Vector2 drawPos = Vector2.Zero;
            var lineHeight = Globals.Globals.DefaultFont.MeasureString("XMI").Y;
            foreach (var kvp in mDebugInfo)
            {
                string line = kvp.Key + ": " + kvp.Value;                
                mSpriteBatch.DrawString(Globals.Globals.DefaultFont, line, drawPos, Color.Black);
                mSpriteBatch.DrawString(Globals.Globals.DefaultFont, line, drawPos + Vector2.One, Color.White);
                drawPos += Vector2.UnitY * lineHeight;
            }
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
    }
}