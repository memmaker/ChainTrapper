using ChainTrapper.Basics;

namespace ChainTrapper.Traps
{
    public interface IVictimCollisionListener
    {
        void OnVictimEntered(Context context, GameObject victim);
    }
}