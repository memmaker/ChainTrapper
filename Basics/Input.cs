using Box2DX.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ChainTrapper.Basics
{
    public class Input
    {
        public event EmptyEvent NextWeapon;
        public event EmptyEvent Fire;
        public event DirectionEvent Movement;
        
        private KeyboardState mOldKeyboardState;
        //private MouseState mOldMouseState;
        private Vec2 mOldInput;

        public void HandleInput()
        {
            Vec2 input = Vec2.Zero;
            var keyState = Keyboard.GetState();
            //var mouseState = Mouse.GetState();

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
                NextWeapon?.Invoke();
            }

            if (keyState.IsKeyDown(Keys.Space) && mOldKeyboardState.IsKeyUp(Keys.Space))
            {
                Fire?.Invoke();
            }

            if (input != Vec2.Zero || mOldInput != Vec2.Zero)
            {
                input.Normalize();
                Movement?.Invoke(input);
            }

            mOldInput = input;
            mOldKeyboardState = keyState;
            //mOldMouseState = mouseState;
        }
    }

    public delegate void EmptyEvent();
    public delegate void DirectionEvent(Vec2 direction);
}