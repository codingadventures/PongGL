using System;

namespace PongGL
{
    public class Program 
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new Game())
            {
                game.Run(60.0);
            }
        }
    }
}