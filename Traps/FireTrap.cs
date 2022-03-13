using System;
using Box2DX.Dynamics;
using ChainTrapper.Basics;
using ChainTrapper.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChainTrapper.Traps
{
    public class FireTrap : GameObject, IGameObjectCollisionListener
    {
        private Texture2D mFireTexture;

        public FireTrap(World world, Vector2 drawPosition, Texture2D texture, Texture2D fireTexture) : base(world, drawPosition, texture)
        {
            mFireTexture = fireTexture;
            CreatePhysicsRepresentation(
                4f, 
                1.0f, 
                0.9f, 
                0.9f, 
                0.1f, 
                false, 
                true
            );
        }

        public void OnCollisionBegin(GameContext gameContext, GameObject go)
        {
            if (go is Player || go is BreadCrumb) return;
            ShouldBeRemoved = true;

            gameContext.QueuedActions.Enqueue(() =>
            {
                var fireDist = 1.1f;
                float sqrt2 = (float) Math.Sqrt(2);
                var firePositions = new Vector2[]
                {
                    new Vector2(DrawPosition.X + Constants.PixelPerMeter * fireDist / sqrt2, DrawPosition.Y + Constants.PixelPerMeter * fireDist / sqrt2),
                    new Vector2(DrawPosition.X + Constants.PixelPerMeter * fireDist / sqrt2, DrawPosition.Y - Constants.PixelPerMeter * fireDist / sqrt2),
                    new Vector2(DrawPosition.X - Constants.PixelPerMeter * fireDist / sqrt2, DrawPosition.Y + Constants.PixelPerMeter * fireDist / sqrt2),
                    new Vector2(DrawPosition.X - Constants.PixelPerMeter * fireDist / sqrt2, DrawPosition.Y - Constants.PixelPerMeter * fireDist / sqrt2),
                    
                    new Vector2(DrawPosition.X + Constants.PixelPerMeter * fireDist, DrawPosition.Y),
                    new Vector2(DrawPosition.X, DrawPosition.Y - Constants.PixelPerMeter * fireDist),
                    new Vector2(DrawPosition.X - Constants.PixelPerMeter * fireDist, DrawPosition.Y),
                    new Vector2(DrawPosition.X, DrawPosition.Y + Constants.PixelPerMeter * fireDist),
                };

                foreach (var firePosition in firePositions)
                {
                    var trap = new Fire(mWorld, firePosition, mFireTexture);
                    gameContext.AllGameObjects.Add(trap);
                }
            });
        }
    }
}