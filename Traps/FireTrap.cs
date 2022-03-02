using System;
using Box2DX.Dynamics;
using ChainTrapper.Basics;
using ChainTrapper.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChainTrapper.Traps
{
    public class FireTrap : GameObject, IVictimCollisionListener
    {
        private Texture2D mFireTexture;

        public FireTrap(World world, Vector2 position, Texture2D texture, Texture2D fireTexture) : base(world, position, texture, 1f, false, true)
        {
            mFireTexture = fireTexture;
        }

        public void OnVictimEntered(Context context, GameObject victim)
        {
            ShouldBeRemoved = true;
            
            context.QueuedActions.Enqueue(() =>
            {
                var firePositions = new Vector2[]
                {
                    new Vector2(Position.X + Constants.PixelPerMeter * 1.5f, Position.Y + Constants.PixelPerMeter * 1.5f),
                    new Vector2(Position.X + Constants.PixelPerMeter * 1.5f, Position.Y - Constants.PixelPerMeter * 1.5f),
                    new Vector2(Position.X - Constants.PixelPerMeter * 1.5f, Position.Y + Constants.PixelPerMeter * 1.5f),
                    new Vector2(Position.X - Constants.PixelPerMeter * 1.5f, Position.Y - Constants.PixelPerMeter * 1.5f),
                
                    new Vector2(Position.X + Constants.PixelPerMeter * 1.5f, Position.Y),
                    new Vector2(Position.X, Position.Y - Constants.PixelPerMeter * 1.5f),
                    new Vector2(Position.X - Constants.PixelPerMeter * 1.5f, Position.Y),
                    new Vector2(Position.X, Position.Y + Constants.PixelPerMeter * 1.5f),
                };

                foreach (var firePosition in firePositions)
                {
                    var trap = new Fire(mWorld, firePosition, mFireTexture);
                    context.AllGameObjects.Add(trap);
                }
            });
        }
    }
}