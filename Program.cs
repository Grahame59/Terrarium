using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Terrarium
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create the main window
            RenderWindow window = new RenderWindow(new VideoMode(800, 600), "SFML Window");

            // Set the frame rate limit to 60 FPS
            window.SetFramerateLimit(60);

            // Main game loop
            while (window.IsOpen)
            {
                // Process events
                window.DispatchEvents();

                // Clear the window
                window.Clear(Color.Black);

                // Draw your game entities here

                // Display everything drawn to the window
                window.Display();
            }
        }
    }
}
