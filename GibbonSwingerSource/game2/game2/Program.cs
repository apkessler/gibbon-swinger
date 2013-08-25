using System;

namespace game2
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GibbonSwinger game = new GibbonSwinger())
            {
                game.Run();
            }
        }
    }
#endif
}

