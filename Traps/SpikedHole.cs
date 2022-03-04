using Box2DX.Dynamics;
using ChainTrapper.Basics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChainTrapper.Traps
{
    public class SpikedHole : GameObject, IVictimCollisionListener
    {
        public SpikedHole(World world, Vector2 position, Texture2D texture) : base(world, position, texture)
        {
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

        public void OnVictimEntered(GameContext gameContext, GameObject victim)
        {
            // Kill the victim
            // Play Death animation
            // Change trap display
            //
            victim.Destroy();
        }
    }
}