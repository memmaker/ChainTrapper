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
        public event PositionEvent MouseBeginDrag;
        public event EmptyEvent Remove;
        public event EmptyEvent SaveMap;
        
        public event EmptyEvent BeginSheepWalk;
        
        public event EmptyEvent LoadMap;

        public double mDragDelay = 0.1f;
        public double mDragCounter = 0.1f;
        
        private KeyboardState mOldKeyboardState;
        private MouseState mOldMouseState;
        private Vec2 mOldInput;
        private Vector2 mOldMousePosition;

        public void HandleInput(GameTime gameTime)
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
            
            if (keyState.IsKeyDown(Keys.Delete) && mOldKeyboardState.IsKeyUp(Keys.Delete))
            {
                Remove?.Invoke();
            }
            
            if (keyState.IsKeyDown(Keys.K) && mOldKeyboardState.IsKeyUp(Keys.K))
            {
                SaveMap?.Invoke();
            }
            
            if (keyState.IsKeyDown(Keys.B) && mOldKeyboardState.IsKeyUp(Keys.B))
            {
                BeginSheepWalk?.Invoke();
            }
            
            if (keyState.IsKeyDown(Keys.L) && mOldKeyboardState.IsKeyUp(Keys.L))
            {
                LoadMap?.Invoke();
            }

            if (input != Vec2.Zero || mOldInput != Vec2.Zero)
            {
                input.Normalize();
                Movement?.Invoke(input);
            }

            if (mouseState.LeftButton == ButtonState.Pressed && mOldMouseState.LeftButton == ButtonState.Released)
            {
                MousePressed?.Invoke(mousePos);
            }
            
            if (mouseState.LeftButton == ButtonState.Pressed && mOldMouseState.LeftButton == ButtonState.Pressed)
            {
                mDragCounter -= gameTime.ElapsedGameTime.TotalSeconds;
                if (mDragCounter <= 0.0f)
                {
                    MouseBeginDrag?.Invoke(mousePos);
                    mDragCounter = mDragDelay;
                }
            }
            else
            {
                mDragCounter = mDragDelay;
            }
            
            if (mouseState.LeftButton == ButtonState.Released && mOldMouseState.LeftButton == ButtonState.Pressed)
            {
                MouseReleased?.Invoke(mousePos);
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