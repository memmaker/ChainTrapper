using Box2DX.Dynamics;
using ChainTrapper.Basics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChainTrapper.Traps
{
    public class SpikedHole : GameObject, IGameObjectCollisionListener
    {
        public SpikedHole(World world, Vector2 drawPosition, Texture2D texture) : base(world, drawPosition, texture)
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

        public void OnCollisionBegin(GameContext gameContext, GameObject go)
        {
            // Kill the go
            // Play Death animation
            // Change trap display
            //
            if (!(go is Player))
            {
                go.Destroy();
            }
            //ShouldBeRemoved = true;
        }

        public void OnCollisionEnd(GameContext gameContext, GameObject go)
        {
            
        }
    }
}