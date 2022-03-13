using ChainTrapper.Basics;

namespace ChainTrapper.Traps
{
    public interface IGameObjectCollisionListener
    {
        void OnCollisionBegin(GameContext gameContext, GameObject go);
        void OnCollisionEnd(GameContext gameContext, GameObject go){}
    }
}