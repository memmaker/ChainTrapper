using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChainTrapper.GameStates
{
    public class StateManager
    {
        private readonly Stack<IGameState> mStack;

        public StateManager()
        {
            mStack = new Stack<IGameState>();
        }
        public bool IsEmpty => mStack.Count == 0;
        public void Update(GameTime gameTime)
        {
            if (mStack.Count == 0)
                return;
            mStack.Peek().Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (mStack.Count == 0)
                return;
            mStack.Peek().Draw(spriteBatch);
        }

        public void Push(IGameState gameState)
        {
            mStack.Push(gameState);
            gameState.OnBegin();
        }

        public void Pop()
        {
            mStack.Pop().OnEnd();
        }
    }
}