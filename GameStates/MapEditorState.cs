using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;
using Box2DX.Common;
using Box2DX.Dynamics;
using ChainTrapper.Basics;
using ChainTrapper.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Box2DX.Dynamics.Color;
using Math = System.Math;

namespace ChainTrapper.GameStates
{
    public enum ObjectTypes { WolfSpawn, SheepPath, Walls }
    public enum EditorMode { Create, Modify }
    public class MapEditorState : IGameState
    {
        private Map mCurrentMap;
        private int mSelectedIndex = -1;
        private Input mInput;
        private DebugDrawer mDebugDrawer;
        private Color mWhiteColor = new Color(1.0f, 1.0f, 1.0f);
        private Color mGrayColor = new Color(0.3f, 0.3f, 0.3f);
        private Color mRedColor = new Color(1.0f, 0.15f, 0.15f);
        private ObjectTypes mObjectType = ObjectTypes.WolfSpawn;
        private EditorMode mEditorMode = EditorMode.Create;
        
        private Vector2 mRectCorner1;
        private Vector2 mRectCorner2;
        private bool mDrawRubberRect = false;
        private bool mDragSelection = false;
        private Vector2 mLastMousePosition;

        public MapEditorState(StateContext context)
        {
            mCurrentMap = new Map();
            
            mInput = new Input();
            mInput.MousePressed += OnMousePressed;
            mInput.MouseReleased += OnMouseReleased;
            mInput.MouseMoved += OnMouseMoved;
            mInput.MouseBeginDrag += OnBeginDrag;
            mInput.NextWeapon += OnNextTool;
            mInput.Fire += OnSwitchMode;
            mInput.Remove += OnRemove;
            mInput.SaveMap += OnSaveMap;
            mInput.LoadMap += OnLoadMap;
            
            mDebugDrawer = new DebugDrawer(context.GraphicsDeviceManager.GraphicsDevice);
        }

        private void OnLoadMap()
        {
            Map loadedMap = Map.FromFile("Maps/map-test.bin");
            mCurrentMap = loadedMap;
        }

        private void OnSaveMap()
        {
            Directory.CreateDirectory("Maps");
            mCurrentMap.SaveToFile("Maps/map-test.bin");
        }

        private void OnBeginDrag(Vector2 position)
        {
            if (mEditorMode == EditorMode.Modify)
            {
                mDragSelection = true;
            }
        }

        private void OnRemove()
        {
            if (mEditorMode == EditorMode.Create)
            {
                switch (mObjectType)
                {
                    case ObjectTypes.WolfSpawn:
                        mCurrentMap.WolfSpawns.RemoveAt(mCurrentMap.WolfSpawns.Count - 1);
                        break;
                    case ObjectTypes.SheepPath:
                        mCurrentMap.SheepPath.RemoveAt(mCurrentMap.SheepPath.Count - 1);
                        break;
                    case ObjectTypes.Walls:
                        mCurrentMap.Walls.RemoveAt(mCurrentMap.Walls.Count - 1);
                        break;
                }
            }
            else if (mEditorMode == EditorMode.Modify)
            {
                switch (mObjectType)
                {
                    case ObjectTypes.WolfSpawn:
                        mCurrentMap.WolfSpawns.RemoveAt(mSelectedIndex);
                        break;
                    case ObjectTypes.SheepPath:
                        mCurrentMap.SheepPath.RemoveAt(mSelectedIndex);
                        break;
                    case ObjectTypes.Walls:
                        mCurrentMap.Walls.RemoveAt(mSelectedIndex);
                        break;
                }
            }

            if (mSelectedIndex > -1)
            {
                mSelectedIndex--;
            }
        }


        private void OnNextTool()
        {
            mObjectType = (ObjectTypes)(((int)mObjectType + 1) % Enum.GetNames(typeof(ObjectTypes)).Length);
            mSelectedIndex = -1;
            mDrawRubberRect = false;
            mDragSelection = false;
        }
        
        private void OnSwitchMode()
        {
            mEditorMode = (EditorMode)(((int)mEditorMode + 1) % Enum.GetNames(typeof(EditorMode)).Length);
            mSelectedIndex = -1;
            mDrawRubberRect = false;
            mDragSelection = false;
        }

        private void OnMousePressed(Vector2 position)
        {
            if (mEditorMode == EditorMode.Create)
            {
                CreateObject(position);
            }
            else if (mEditorMode == EditorMode.Modify)
            {
                SelectObject(position);
            }
        }

        private void SelectObject(Vector2 position)
        {
            switch (mObjectType)
            {
                case ObjectTypes.WolfSpawn:
                    SelectWolfSpawnAt(position);
                    break;
                case ObjectTypes.SheepPath:
                    SelectSheepPathAt(position);
                    break;
                case ObjectTypes.Walls:
                    SelectWallAt(position);
                    break;
            }
        }

        private void SelectWallAt(Vector2 position)
        {
            for (var index = 0; index < mCurrentMap.Walls.Count; index++)
            {
                var wall = mCurrentMap.Walls[index];
                if (wall.Contains(position))
                {
                    mSelectedIndex = index;
                    return;
                }
            }

            mSelectedIndex = -1;
        }

        private void SelectSheepPathAt(Vector2 position)
        {
            for (var index = 0; index < mCurrentMap.SheepPath.Count; index++)
            {
                var sheepPath = mCurrentMap.SheepPath[index];
                if (Vector2.Distance(position, sheepPath) < Constants.PixelPerMeter * 0.5f)
                {
                    mSelectedIndex = index;
                    return;
                }
            }

            mSelectedIndex = -1;
        }

        private void SelectWolfSpawnAt(Vector2 position)
        {
            for (var index = 0; index < mCurrentMap.WolfSpawns.Count; index++)
            {
                var wolfSpawn = mCurrentMap.WolfSpawns[index];
                if (Vector2.Distance(position, wolfSpawn) < Constants.PixelPerMeter * 0.5f)
                {
                    mSelectedIndex = index;
                    return;
                }
            }

            mSelectedIndex = -1;
        }

        private void CreateObject(Vector2 position)
        {
            switch (mObjectType)
            {
                case ObjectTypes.WolfSpawn:
                    mCurrentMap.WolfSpawns.Add(position);
                    break;
                case ObjectTypes.SheepPath:
                    mCurrentMap.SheepPath.Add(position);
                    break;
                case ObjectTypes.Walls:
                    BeginRubberRect(position);
                    break;
            }
        }

        private void BeginRubberRect(Vector2 position)
        {
            mRectCorner1 = position;
            mRectCorner2 = position;
            mDrawRubberRect = true;
        }

        private void OnMouseReleased(Vector2 position)
        {
            if (mObjectType == ObjectTypes.Walls && mEditorMode == EditorMode.Create)
            {
                mRectCorner2 = position;
                mDrawRubberRect = false;
                // Create a wall from the corners now..

                CreateWallFromRubberRect();
            }

            if (mDragSelection)
            {
                if (mSelectedIndex > -1)
                {
                    switch (mObjectType)
                    {
                        case ObjectTypes.WolfSpawn:
                            mCurrentMap.WolfSpawns[mSelectedIndex] = position;
                            break;
                        case ObjectTypes.SheepPath:
                            mCurrentMap.SheepPath[mSelectedIndex] = position;
                            break;
                        case ObjectTypes.Walls:
                            var currentWall = mCurrentMap.Walls[mSelectedIndex];
                            var offset = position - currentWall.Center.ToVector2();
                            currentWall.Offset(offset);
                            mCurrentMap.Walls.RemoveAt(mSelectedIndex);
                            mCurrentMap.Walls.Insert(mSelectedIndex, currentWall);
                            break;
                    }
                }
                mDragSelection = false;
            }
        }

        private void CreateWallFromRubberRect()
        {
            var centerPos = (mRectCorner1 + mRectCorner2) * 0.5f;
            int width = (int) Math.Abs(mRectCorner1.X - mRectCorner2.X);
            int height = (int) Math.Abs(mRectCorner1.Y - mRectCorner2.Y);
            mCurrentMap.Walls.Add(
                new Rectangle(
                    (int) (centerPos.X - (width * 0.5f)),
                    (int) (centerPos.Y - (height * 0.5f)),
                    width,
                    height
                )
            );
        }


        private void OnMouseMoved(Vector2 position)
        {
            if (mObjectType == ObjectTypes.Walls)
            {
                mRectCorner2 = position;
            }

            mLastMousePosition = position;
        }

        public void OnBegin()
        {
            
        }
        public void Update(GameTime gameTime)
        {
            mInput.HandleInput(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
            spriteBatch.Begin();

            var toolText = Enum.GetName(typeof(EditorMode), mEditorMode) + " > " + Enum.GetName(typeof(ObjectTypes), mObjectType);
            spriteBatch.DrawString(Globals.Globals.DefaultFont, toolText, Vector2.Zero, Microsoft.Xna.Framework.Color.White);

            var debugText = "Index: " + mSelectedIndex + ", Drag: " + mDragSelection.ToString();
            spriteBatch.DrawString(Globals.Globals.DefaultFont, debugText, Vector2.Zero + Vector2.UnitY * 16.0f, Microsoft.Xna.Framework.Color.White);

            DrawWolfSpawns(spriteBatch);

            DrawSheepPath(spriteBatch);

            DrawWalls();

            if (mDrawRubberRect)
            {
                DrawRubberRect();
            }

            spriteBatch.End();
            mDebugDrawer.FlushDrawing();
        }

        private void DrawRubberRect()
        {
            Vec2[] vertices = new[]
            {
                mRectCorner1.ToPhysics(),
                new Vec2(mRectCorner1.X / Constants.PixelPerMeter, mRectCorner2.Y / Constants.PixelPerMeter),
                mRectCorner2.ToPhysics(),
                new Vec2(mRectCorner2.X / Constants.PixelPerMeter, mRectCorner1.Y / Constants.PixelPerMeter),
            };
            mDebugDrawer.DrawPolygon(vertices, 4, mWhiteColor);
        }

        private void DrawWalls()
        {
            for (var index = 0; index < mCurrentMap.Walls.Count; index++)
            {
                var wall = mCurrentMap.Walls[index];
                
                Color drawColor = mGrayColor;
                if (mObjectType == ObjectTypes.Walls && mSelectedIndex == index)
                {
                    drawColor = mWhiteColor;
                    if (mDragSelection)
                    {
                        var offset = mLastMousePosition - wall.Center.ToVector2();
                        wall.Offset(offset);
                    }
                }
                Vec2[] vertices = new[]
                {
                    wall.Location.ToVector2().ToPhysics(),
                    new Vec2(wall.Right / (float) Constants.PixelPerMeter, wall.Top / (float) Constants.PixelPerMeter),
                    new Vec2(wall.Right / (float) Constants.PixelPerMeter,
                        wall.Bottom / (float) Constants.PixelPerMeter),
                    new Vec2(wall.Left / (float) Constants.PixelPerMeter,
                        wall.Bottom / (float) Constants.PixelPerMeter),
                };
                mDebugDrawer.DrawPolygon(vertices, 4, drawColor);
            }
        }

        private void DrawSheepPath(SpriteBatch spriteBatch)
        {
            for (var index = 0; index < mCurrentMap.SheepPath.Count; index++)
            {
                var sheepWaypoint = mCurrentMap.SheepPath[index];
                Color drawColor = mGrayColor;
                var drawPosition = sheepWaypoint.ToPhysics();
                
                if (mObjectType == ObjectTypes.SheepPath && mSelectedIndex == index)
                {
                    drawColor = mWhiteColor;
                    if (mDragSelection)
                    {
                        drawPosition = mLastMousePosition.ToPhysics();
                    }
                }
                
                mDebugDrawer.DrawCircle(drawPosition, 0.5f, drawColor);
                if (index > 0)
                {
                    var previousPoint = mCurrentMap.SheepPath[index - 1];
                    mDebugDrawer.DrawSegment(previousPoint.ToPhysics(), drawPosition, mGrayColor);
                }

                var text = index.ToString();
                var stringBounds = Globals.Globals.DefaultFont.MeasureString(text);
                spriteBatch.DrawString(Globals.Globals.DefaultFont, text, drawPosition.ToScreen(), Microsoft.Xna.Framework.Color.White,
                    0.0f, stringBounds * 0.5f, 1.0f, SpriteEffects.None, 1.0f);
            }
        }

        private void DrawWolfSpawns(SpriteBatch spriteBatch)
        {
            for (var index = 0; index < mCurrentMap.WolfSpawns.Count; index++)
            {
                var wolfSpawn = mCurrentMap.WolfSpawns[index];
                Color drawColor = mRedColor;
                Vec2 drawPosition = wolfSpawn.ToPhysics();
                if (mObjectType == ObjectTypes.WolfSpawn && mSelectedIndex == index)
                {
                    drawColor = mWhiteColor;
                    if (mDragSelection)
                    {
                        drawPosition = mLastMousePosition.ToPhysics();
                    }
                }
                mDebugDrawer.DrawCircle(drawPosition, 0.5f, drawColor);
                var text = index.ToString();
                var stringBounds = Globals.Globals.DefaultFont.MeasureString(text);
                spriteBatch.DrawString(Globals.Globals.DefaultFont, text, drawPosition.ToScreen(), Microsoft.Xna.Framework.Color.Red, 0.0f,
                    stringBounds * 0.5f, 1.0f, SpriteEffects.None, 1.0f);
            }
        }
    }
}