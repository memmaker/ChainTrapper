using Box2DX.Collision;
using Box2DX.Dynamics;
using ChainTrapper.Basics;
using ChainTrapper.Traps;

namespace ChainTrapper.Physics
{
    public class CollisionHandler : ContactListener
    {
        private GameContext mGameContext;

        public CollisionHandler(GameContext gameContext)
        {
            mGameContext = gameContext;
        }
        
        public void BeginContact(Contact contact)
        {
            object go1 = null, go2 = null;
            
            if (contact.FixtureA.Body.GetUserData() != null)
            {
                go1 = contact.FixtureA.Body.GetUserData();
            }
            
            if (contact.FixtureB.Body.GetUserData() != null)
            {
                go2 = contact.FixtureB.Body.GetUserData();
            }

            if (go1 is GameObject burn1 && go2 is GameObject burn2)
            {
                if (burn1.IsBurning)
                {
                    burn2.IsBurning = true;
                }
                
                if (burn2.IsBurning)
                {
                    burn1.IsBurning = true;
                }
            }
            
            if (go1 is IVictimCollisionListener listener1 && (go2 is Wolf || go2 is Sheep))
            {
                listener1.OnVictimEntered(mGameContext, (GameObject)go2);
            }
            
            if (go2 is IVictimCollisionListener listener2 && (go1 is Wolf || go1 is Sheep))
            {
                listener2.OnVictimEntered(mGameContext, (GameObject)go1);
            }

            if (go1 is Projectile && !(go2 is Player))
            {
                var go = (GameObject)go1;
                go.Destroy();
            }

            if (go2 is Projectile && !(go1 is Player))
            {
                var go = (GameObject)go2;
                go.Destroy();
            }
        }
        
        public void EndContact(Contact contact)
        {
            
        }

        public void PreSolve(Contact contact, Manifold oldManifold)
        {
            
        }

        public void PostSolve(Contact contact, ContactImpulse impulse)
        {
            
        }
        
        /*
        private void HitUpperEdge(GameObject gameObject, Contact contact)
        {
            var rotation = gameObject.Rotation;
            Color[] pixelData = new Color[Constants.PixelPerMeter * Constants.PixelPerMeter];
            mArrowTexture.GetData(pixelData);
            contact.GetWorldManifold(out var worldManifold);
            var firstContact = worldManifold.Points[0] * Constants.PixelPerMeter;
            Debug("Contact", firstContact.X + "/" + firstContact.Y);
            Color[] originalEdgeData = new Color[Constants.PixelPerMeter * Constants.PixelPerMeter];
            mUpperEdgeTexture.GetData(0, new Rectangle((int) (firstContact.X - 16), 0, Constants.PixelPerMeter, Constants.PixelPerMeter), originalEdgeData,0, Constants.PixelPerMeter * Constants.PixelPerMeter);
            for (int x = 0; x < Constants.PixelPerMeter; x++)
            {
                for (int y = 0; y < Constants.PixelPerMeter; y++)
                {
                    int index = y * Constants.PixelPerMeter + x;
                    var sourcePixel = pixelData[index];
                    if (sourcePixel == Color.Transparent)
                        continue;
                    int destX = Helper.RotatePixelX(x, y, Constants.PixelPerMeter/2, Constants.PixelPerMeter/2, rotation);
                    int destY = Helper.RotatePixelY(x, y, Constants.PixelPerMeter/2, Constants.PixelPerMeter/2, rotation);

                    if (destX > -1 && destX < Constants.PixelPerMeter && destY > -1 && destY < Constants.PixelPerMeter)
                    {
                        int rotIndex = destY * Constants.PixelPerMeter + destX;
                        originalEdgeData[rotIndex] = sourcePixel;
                    }
                }
            }
            mUpperEdgeTexture.SetData(0, new Rectangle((int) (firstContact.X - 16), 0, Constants.PixelPerMeter, Constants.PixelPerMeter), originalEdgeData, 0, originalEdgeData.Length);
        }
        */
    }
}