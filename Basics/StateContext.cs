using ChainTrapper.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ChainTrapper.Basics
{
    public struct StateContext
    {
        public GraphicsDeviceManager GraphicsDeviceManager;
        public SpriteBatch SpriteBatch;
        public ContentManager ContentManager;
        public StateManager StateManager;
    }
}