using System;
using Box2DX.Common;
using Microsoft.Xna.Framework;

namespace ChainTrapper
{
    public static class Extensions
    {
        public static Vec2 ToVec2(this Vector2 vector)
        {
            return new Vec2(vector.X, vector.Y);
        }
    }
    public static class Helper
    {
        private static Random sRandom = new Random();
        public static Vector2 RandomPosition()
        {
            return new Vector2(sRandom.Next(ChainTrapperGame.ScreenWidth), sRandom.Next(ChainTrapperGame.ScreenHeight));
        }

        public static Vector2 RandomDirection()
        {
            float x = (float)((sRandom.NextDouble() * 2.0f) - 1);
            float y = (float)((sRandom.NextDouble() * 2.0f) - 1);
            var result =  new Vector2(x, y);
            result.Normalize();
            return result;
        }

        public static float RandomSpeed()
        {
            return (float)(sRandom.NextDouble() * 6.0f);
        }

        public static bool Percent(int percent)
        {
            double asDouble = percent / 100.0d;
            return sRandom.NextDouble() <= asDouble;
        }

        public static int RandomInt(int maxValue)
        {
            return sRandom.Next(maxValue);
        }
    }
}
