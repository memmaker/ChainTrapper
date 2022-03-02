using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChainTrapper.GameStates
{
    public interface IGameState
    {
        void OnBegin();
        void OnEnd(){}
        public void Update(GameTime gameTime);
        public void Draw(SpriteBatch spriteBatch);

    }
}