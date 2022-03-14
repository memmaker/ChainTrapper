using System;
using Box2DX.Common;
using Microsoft.Xna.Framework;
using Math = System.Math;

namespace ChainTrapper.Globals
{
    public static class Helper
    {
        private static Random sRandom = new Random(1234);
        public static Vector2 RandomPosition()
        {
            return new Vector2(sRandom.Next(Globals.ScreenWidth), sRandom.Next(Globals.ScreenHeight));
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
        
        public static int RandomInt(int minValue, int maxValue)
        {
            return sRandom.Next(minValue, maxValue);
        }

        // Rotate the x-coordinate of pixel in a bitmap around the point (a,b) by angle
        public static int RotatePixelX(int x, int y, int a, int b, float angle)
        {
            return (int)((x - a) * Math.Cos(angle) + (b - y) * Math.Sin(angle) + a);
        }
        
        // Rotate the y-coordinate of pixel in a bitmap around the point (a,b) by angle
        public static int RotatePixelY(int x, int y, int a, int b, float angle)
        {
            return (int)((x - a) * Math.Sin(angle) + (y - b) * Math.Cos(angle) + b);
        }

        public static double AngleFromVector(Vec2 vec)
        {
            var angle = Math.Atan2(vec.Y, vec.X);
            return angle;
        }
        
        public static Vec2 RotationToDirectionVector(float rotationInRadians)
        {
            return new Vec2((float) Math.Cos(rotationInRadians), (float) Math.Sin(rotationInRadians));
        }
        public static float RadianAngleBetween2DVectors(Vec2 from, Vec2 to)
        {
            return (float)(Math.Atan2(to.Y, to.X) - Math.Atan2(from.Y, from.X));
        }
        public static bool IsInViewCone(Vec2 sourcePos, Vec2 targetPos, Vec2 lookDirection, float fovInDegrees)
        {
            Vec2 dirToTarget = targetPos - sourcePos;
            Vec2 dirToLook = lookDirection;
            double angleBetween = Math.Abs(RadianAngleBetween2DVectors(dirToTarget, dirToLook));

            if (angleBetween > Math.PI)
                angleBetween = Math.Abs((2 * Math.PI) - angleBetween);
            return angleBetween < (MathHelper.ToRadians(fovInDegrees) / 2);
        }
    }
}