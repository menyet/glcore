using System;

namespace OpenglApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            try
            {
                C x;

                x = null;

                using (Game game = new Game(800, 600, "LearnOpenGL"))
                {
                    //To create a new window, create a class that extends GameWindow, then call Run() on it.
                    //Run takes a double, which is how many frames per second it should strive to reach.
                    //You can leave that out and it'll just update as fast as the hardware will allow it.
                    game.Run(60.0);
                }
            }
            finally { }
            Console.ReadLine();
        }
    }

    public class C
    {
        int _a;
    }
}
