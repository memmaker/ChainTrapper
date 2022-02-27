using System;

namespace ChainTrapper
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new ChainTrapperGame())
                game.Run();
        }
    }
}
