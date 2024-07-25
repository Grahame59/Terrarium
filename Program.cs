using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Terrarium.GameBox.Systems;

namespace Terrarium
{
    public static class Program
    {
        public static void Main()
        {
            RenderWindow window = new RenderWindow(new VideoMode(800, 600), "Terrarium");
            window.SetFramerateLimit(60);

            TerrainGeneration terrain = new TerrainGeneration(window);

            window.Closed += (sender, e) => window.Close();

            while (window.IsOpen)
            {
                window.DispatchEvents();
                window.Clear();

                terrain.Render();

                window.Display();
            }
        }
    }
}
