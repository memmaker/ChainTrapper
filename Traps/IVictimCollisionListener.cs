using ChainTrapper.Basics;

namespace ChainTrapper.Traps
{
    public interface IVictimCollisionListener
    {
        void OnVictimEntered(GameContext gameContext, GameObject victim);
        void OnVictimLeft(GameContext gameContext, GameObject victim){}
    }
}