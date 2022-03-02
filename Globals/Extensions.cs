using Box2DX.Common;
using ChainTrapper.Basics;
using Microsoft.Xna.Framework;

namespace ChainTrapper.Globals
{
    public static class Extensions
    {
        public static Vec2 ToVec2(this Vector2 vector)
        {
            return new Vec2(vector.X, vector.Y);
        }
        
        public static Vector3 ToVector3(this Vec2 vector)
        {
            return new Vector3(vector.X, vector.Y, 0);
        }
        
        public static Vector2 ToScreen(this Vec2 vector)
        {
            return new Vector2(vector.X * Constants.PixelPerMeter, vector.Y * Constants.PixelPerMeter);
        }
    }
}
