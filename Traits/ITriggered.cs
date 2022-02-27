using System;
using ChainTrapper.Basics;

namespace ChainTrapper.Traits
{
    public interface ITriggered
    {
        public void ActivateTrigger(Context context);
    }
}
