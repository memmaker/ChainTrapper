using System;
using System.Collections.Generic;
using Box2DX.Dynamics;

namespace ChainTrapper.Basics
{
    public struct GameContext
    {
        public World PhysicsWorld;
        public List<GameObject> AllGameObjects;
        public Queue<Action> QueuedActions;
    }
}
