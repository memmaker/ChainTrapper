using System;
using System.Collections.Generic;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using ChainTrapper.Basics;
using ChainTrapper.GameStates;
using ChainTrapper.Globals;
using ChainTrapper.Physics;
using ChainTrapper.Traits;
using ChainTrapper.Traps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using IDrawable = ChainTrapper.Traits.IDrawable;

namespace ChainTrapper
{

    public class ChainTrapperGame : Game
    {
        private StateContext mContext = new StateContext();
        private StateManager StateManager => mContext.StateManager;
        private GraphicsDeviceManager Graphics => mContext.GraphicsDeviceManager;
        private SpriteBatch SpriteBatch => mContext.SpriteBatch; 

        public ChainTrapperGame()
        {
            mContext.GraphicsDeviceManager = new GraphicsDeviceManager(this);
            if (GraphicsDevice == null)
            {
                Graphics.ApplyChanges();
            }
            Graphics.PreferredBackBufferWidth = 1440 / 2;
            Graphics.PreferredBackBufferHeight = 900 / 2;
            
            // This should set the resolution to the desktop resolution
            //Graphics.PreferredBackBufferWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            //Graphics.PreferredBackBufferHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            
            //Graphics.IsFullScreen = true;
            Graphics.ApplyChanges();
            
            Globals.Globals.ScreenWidth = Graphics.PreferredBackBufferWidth;
            Globals.Globals.ScreenHeight = Graphics.PreferredBackBufferHeight;
            
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1.0d / 60.0d);
            
            mContext.StateManager = new StateManager();
            mContext.SpriteBatch = new SpriteBatch(GraphicsDevice);
            mContext.ContentManager = Content;
            
            Globals.Globals.DefaultFont = Content.Load<SpriteFont>("Fonts/Default");
            
            //StateManager.Push(new MainGameState(mContext));
            StateManager.Push(new MapEditorState(mContext));
        }

        protected override void Update(GameTime gameTime)
        {
            if (StateManager.IsEmpty)
                Exit();
            
            StateManager.Update(gameTime);
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            StateManager.Draw(SpriteBatch);
            base.Draw(gameTime);
        }
    }
}
