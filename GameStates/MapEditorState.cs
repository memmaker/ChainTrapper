using System;
using System.Collections.Generic;
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
    public enum EditorState { SpawnWolves, SheepPath, Walls }
    public class MapEditorState : IGameState
    {
        private Map mCurrentMap;
        private Input mInput;
        private DebugDrawer mDebugDrawer;
        private Color mWhiteColor = new Color(1.0f, 1.0f, 1.0f);
        private Color mGrayColor = new Color(0.3f, 0.3f, 0.3f);
        private Color mRedColor = new Color(1.0f, 0.15f, 0.15f);
        private EditorState mState = EditorState.SpawnWolves;

        private Vector2 mRectCorner1;
        private Vector2 mRectCorner2;
        private bool mDrawRubberRect = false;
        public MapEditorState(StateContext context)
        {
            mCurrentMap = new Map();
            
            mInput = new Input();
            mInput.MousePressed += OnMousePressed;
            mInput.MouseReleased += OnMouseReleased;
            mInput.MouseMoved += OnMouseMoved;
            mInput.NextWeapon += OnNextTool;
            
            mDebugDrawer = new DebugDrawer(context.GraphicsDeviceManager.GraphicsDevice);
        }


        private void OnNextTool()
        {
            mState = (EditorState)(((int)mState + 1) % Enum.GetNames(typeof(EditorState)).Length);
        }

        private void OnMousePressed(Vector2 position)
        {
            switch (mState)
            {
                case EditorState.SpawnWolves:
                    mCurrentMap.WolfSpawns.Add(position);
                    break;
                case EditorState.SheepPath:
                    mCurrentMap.SheepPath.Add(position);
                    break;
                case EditorState.Walls:
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
            if (mState == EditorState.Walls)
            {
                mRectCorner2 = position;
                mDrawRubberRect = false;
                // Create a wall from the corners now..

                CreateWallFromRubberRect();
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
            if (mState == EditorState.Walls)
            {
                mRectCorner2 = position;
            }
        }

        public void OnBegin()
        {
            
        }
        public void Update(GameTime gameTime)
        {
            mInput.HandleInput();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
            spriteBatch.Begin();

            var toolText = Enum.GetName(typeof(EditorState), mState);
            spriteBatch.DrawString(Globals.Globals.DefaultFont, toolText, Vector2.Zero, Microsoft.Xna.Framework.Color.White);
            
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
            foreach (var wall in mCurrentMap.Walls)
            {
                Vec2[] vertices = new[]
                {
                    wall.Location.ToVector2().ToPhysics(),
                    new Vec2(wall.Right / (float) Constants.PixelPerMeter, wall.Top / (float) Constants.PixelPerMeter),
                    new Vec2(wall.Right / (float) Constants.PixelPerMeter, wall.Bottom / (float) Constants.PixelPerMeter),
                    new Vec2(wall.Left / (float) Constants.PixelPerMeter, wall.Bottom / (float) Constants.PixelPerMeter),
                };
                mDebugDrawer.DrawPolygon(vertices, 4, mGrayColor);
            }
        }

        private void DrawSheepPath(SpriteBatch spriteBatch)
        {
            for (var index = 0; index < mCurrentMap.SheepPath.Count; index++)
            {
                var sheepWaypoint = mCurrentMap.SheepPath[index];
                mDebugDrawer.DrawCircle(sheepWaypoint.ToPhysics(), 0.5f, mWhiteColor);
                if (index > 0)
                {
                    var previousPoint = mCurrentMap.SheepPath[index - 1];
                    mDebugDrawer.DrawSegment(previousPoint.ToPhysics(), sheepWaypoint.ToPhysics(), mGrayColor);
                }

                var text = index.ToString();
                var stringBounds = Globals.Globals.DefaultFont.MeasureString(text);
                spriteBatch.DrawString(Globals.Globals.DefaultFont, text, sheepWaypoint, Microsoft.Xna.Framework.Color.White,
                    0.0f, stringBounds * 0.5f, 1.0f, SpriteEffects.None, 1.0f);
            }
        }

        private void DrawWolfSpawns(SpriteBatch spriteBatch)
        {
            for (var index = 0; index < mCurrentMap.WolfSpawns.Count; index++)
            {
                var wolfSpawn = mCurrentMap.WolfSpawns[index];
                mDebugDrawer.DrawCircle(wolfSpawn.ToPhysics(), 0.5f, mRedColor);
                var text = index.ToString();
                var stringBounds = Globals.Globals.DefaultFont.MeasureString(text);
                spriteBatch.DrawString(Globals.Globals.DefaultFont, text, wolfSpawn, Microsoft.Xna.Framework.Color.Red, 0.0f,
                    stringBounds * 0.5f, 1.0f, SpriteEffects.None, 1.0f);
            }
        }
    }
}