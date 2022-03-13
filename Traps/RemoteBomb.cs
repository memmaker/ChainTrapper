using System;
using Box2DX.Dynamics;
using ChainTrapper.Basics;
using ChainTrapper.Traits;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChainTrapper.Traps
{
    public class RemoteBomb : Bomb, ITriggered
    {
        public RemoteBomb(World world, Vector2 drawPosition, Texture2D texture) : base(world, drawPosition, texture)
        {
        }

        public void ActivateTrigger(GameContext gameContext)
        {
            Explode(gameContext);
        }
    }
}
