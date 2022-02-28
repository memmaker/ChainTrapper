using System;
using System.Collections.Generic;

namespace ChainTrapper.Basics
{
    public struct Context
    {
        public List<GameObject> AllGameObjects;
        public Queue<Action> QueuedActions;
    }
}
