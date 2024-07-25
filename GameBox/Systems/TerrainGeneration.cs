using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Terrarium.GameBox.Systems
{
    public class TerrainGeneration : IRenderable, IUpdatable
    {
        private RenderWindow _window;
        private VertexArray _terrain;

        public TerrainGeneration(RenderWindow window)
        {
            _window = window;
            _terrain = new VertexArray(PrimitiveType.Points);
            GenerateTerrain();
        }

        public void Update()
        {
            // Update logic if necessary
        }

        public void Render()
        {
            _window.Draw(_terrain);
        }

        private void GenerateTerrain()
        {
            int width = 800;
            int height = 600;
            float scale = 0.01f;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float noiseValue = Mathf.PerlinNoise(x * scale, y * scale);
                    Color color = new Color((byte)(noiseValue * 255), (byte)(noiseValue * 255), (byte)(noiseValue * 255));

                    _terrain.Append(new Vertex(new Vector2f(x, y), color));
                }
            }
        }
    }
}
