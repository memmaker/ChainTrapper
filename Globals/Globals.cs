using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChainTrapper.Globals
{
    public static class Globals
    {
        public static SpriteFont DefaultFont { get; set; }
        public static int ScreenWidth { get; set; }
        public static int ScreenHeight { get; set; }
        public static int CurrentLevelScore { get; set; }
        public static DebugDrawer DebugDrawer { get; set; }

        public static bool DebugEnabled = false;
    }
}