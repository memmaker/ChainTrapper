using Box2DX.Dynamics;
using ChainTrapper.Basics;
using ChainTrapper.Traps;

namespace ChainTrapper.Physics
{
    internal class CollisionFilter : ContactFilter
    {
        public override bool ShouldCollide(Fixture fixtureA, Fixture fixtureB)
        {
            object go1 = null, go2 = null;
            
            if (fixtureA.Body.GetUserData() != null)
            {
                go1 = fixtureA.Body.GetUserData();
            }
            
            if (fixtureB.Body.GetUserData() != null)
            {
                go2 = fixtureB.Body.GetUserData();
            }

            if (go1 is Throwable throw1 && !throw1.IsAbleToCollide)
            {
                return false;
            }
            
            if (go2 is Throwable throw2 && !throw2.IsAbleToCollide)
            {
                return false;
            }

            if ((go1 is Player player1))
            {
                if (player1.IsRolling && go2 is Enemy) return false;
            }
            
            if ((go2 is Player player2))
            {
                if (player2.IsRolling && go1 is Enemy) return false;
            }
            
            if ((go1 is Player && go2 is Bomb) || (go2 is Player && go1 is Bomb))
            {
                return false;
            }

            if (go1 is Wall || go2 is Wall)
                return true;

            return true;
        }
    }
}