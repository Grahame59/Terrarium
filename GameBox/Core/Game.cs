//using System;
//using GameBox.Systems;
using SFML.Graphics;
using SFML.Window;

namespace Terrarium
{
    public class Game
    {
        private RenderWindow window;
        private TerrainGenerator terrainGenerator;

        public Game()
        {
            window = new RenderWindow(new VideoMode(800, 600), "Terrarium");
            window.Closed += (sender, e) => window.Close();

            terrainGenerator = new TerrainGenerator();
        }

        public void Run()
        {
            while (window.IsOpen)
            {
                window.DispatchEvents();
                
                Update();
                Render();
            }
        }

        private void Update()
        {
            Console.WriteLine("Updating game...");
            terrainGenerator.Update();
        }

        private void Render()
        {
            window.Clear();
            
            Console.WriteLine("Rendering game...");
            terrainGenerator.Render(window);
            
            window.Display();
        }
    }
}
