using Terrarium;

public class TerrainGeneration
{
    public float[] Vertices { get; private set; }

    public void GenerateTerrain(int width, int height)
    {
        // Code for generating terrain vertices using Perlin noise
        Vertices = new float[width * height * 3]; // Assuming 3 coordinates (x, y, z) per vertex
        
        // Example of how you might generate the vertices
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float x = i;
                float z = j;
                float y = Mathf.PerlinNoise(x * 0.1f, z * 0.1f, 0) * 10; // Use Perlin noise for height
                int index = (i * height + j) * 3;
                Vertices[index] = x;
                Vertices[index + 1] = y;
                Vertices[index + 2] = z;
            }
        }
    }
}
