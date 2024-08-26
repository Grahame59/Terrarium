using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Terrarium.GameBox.Systems;

namespace Terrarium
{
    public class Game
    {
        private RenderWindow _window;
        private TerrainGeneration _terrainGenerator;

        public Game()
        {
            _window = new RenderWindow(new VideoMode(800, 600), "Terrarium");
            _window.SetFramerateLimit(60);
            _window.Closed += (sender, e) => _window.Close();

            _terrainGenerator = new TerrainGeneration(_window);
        }

        public void Run()
        {
            while (_window.IsOpen)
            {
                _window.DispatchEvents();
                Update();
                Render();
            }
        }

        private void Update()
        {
            _terrainGenerator.Update();
        }

        private void Render()
        {
            _window.Clear();
            _terrainGenerator.Render();
            _window.Display();
        }

        public static void Main()
        {
            Game game = new Game();
            game.Run();
        }
    }
}
