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
        public event PositionEvent MousePressed;
        public event PositionEvent MouseReleased;
        public event PositionEvent MouseMoved;
        
        private KeyboardState mOldKeyboardState;
        private MouseState mOldMouseState;
        private Vec2 mOldInput;
        private Vector2 mOldMousePosition;

        public void HandleInput()
        {
            Vec2 input = Vec2.Zero;
            var keyState = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            var mousePos = Mouse.GetState().Position.ToVector2();

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

            if (mouseState.LeftButton == ButtonState.Pressed && mOldMouseState.LeftButton == ButtonState.Released)
            {
                MousePressed?.Invoke(mouseState.Position.ToVector2());
            }
            
            if (mouseState.LeftButton == ButtonState.Released && mOldMouseState.LeftButton == ButtonState.Pressed)
            {
                MouseReleased?.Invoke(mouseState.Position.ToVector2());
            }

            if (mousePos != mOldMousePosition)
            {
                MouseMoved?.Invoke(mousePos);
            }
            
            mOldInput = input;
            mOldKeyboardState = keyState;
            mOldMouseState = mouseState;
            mOldMousePosition = mousePos;
        }
    }

    public delegate void EmptyEvent();
    public delegate void DirectionEvent(Vec2 direction);
    public delegate void PositionEvent(Vector2 position);
}