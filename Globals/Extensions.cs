using System.Collections.Generic;
using System.Linq;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using ChainTrapper.Basics;
using Microsoft.Xna.Framework;

namespace ChainTrapper.Globals
{
    
    internal struct RayHits
    {
    }
    public static class Extensions
    {
        public static int Query(this World world, Vec2 center, float squareSize, Fixture[] fixtures, int maxCount = 100)
        {
            Vec2 lowerBound = new Vec2(center.X - squareSize, center.Y - squareSize);
            Vec2 upperBound = new Vec2(center.X + squareSize, center.Y + squareSize);
            int count = world.Query(
                new AABB()
                {
                    LowerBound = lowerBound,
                    UpperBound = upperBound
                }, fixtures, maxCount);
            return count;
        }
        public static void SetMagnitude(this ref Vec2 vector, float length)
        {
            vector.Normalize();
            vector *= length;
        }
        
        public static void Limit(this ref Vec2 vector, float maxLength)
        {
            if (vector.LengthSquared() > maxLength * maxLength)
            {
                vector.SetMagnitude(maxLength);
            }
        }

        public static Vec2 RotatedLeft(this Vec2 vector)
        {
            return new Vec2(vector.Y, -vector.X);
        }
        
        public static Vec2 RotatedRight(this Vec2 vector)
        {
            return new Vec2(-vector.Y, vector.X);
        }
        public static Vec2 GetNearestPoint(this Fixture fixture, Vec2 point)
        {
            fixture.Shape.ComputeAABB(out var aabb, XForm.Identity);
            var nearestPoint = new Vec2(
                Math.Clamp(point.X, fixture.Body.GetPosition().X + aabb.LowerBound.X,
                    fixture.Body.GetPosition().X + aabb.UpperBound.X),
                Math.Clamp(point.Y, fixture.Body.GetPosition().Y + aabb.LowerBound.Y,
                    fixture.Body.GetPosition().Y + aabb.UpperBound.Y)
            );
            return nearestPoint;
        }
        public static IEnumerable<Fixture> Raycast(this World world, Vector2 start, Vector2 end)
        {
            Fixture[] hitFixtures;
            int count = world.Raycast(
                new Segment()
                {
                    P1 = start.ToPhysics(),
                    P2 = end.ToPhysics()
                },
                out hitFixtures,
                100,
                true,
                null
            );
            
            return hitFixtures.Take(count);
        }
        public static IEnumerable<Fixture> Raycast(this World world, Vec2 start, Vec2 end)
        {
            Fixture[] hitFixtures;
            int count = world.Raycast(
                new Segment()
                {
                    P1 = start,
                    P2 = end
                },
                out hitFixtures,
                100,
                true,
                null
            );

            return hitFixtures.Take(count);
        }
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
        
        public static Vector2 ToVector2(this Vec2 vector)
        {
            return new Vector2(vector.X, vector.Y);
        }
        
        public static Vec2 ToPhysics(this Vector2 vector)
        {
            return new Vec2(vector.X / Constants.PixelPerMeter, vector.Y / Constants.PixelPerMeter);
        }
    }
}
