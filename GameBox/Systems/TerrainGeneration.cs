/*
using System;
using OpenTK.Mathematics;
using Error;
using Terrarium;

public class TerrainGeneration
{
    public float[] Vertices { get; private set; }
    public uint[] Indices { get; private set; }
    public int Columns { get; set; }
    public int Rows { get; set; }

    public void GenerateTerrain(int width, int height, float scale = 1f, float heightMultiplier = 1f)
    {
        int vertexCount = (width + 1) * (height + 1);
        Vertices = new float[vertexCount * 3]; // x, y, z for each vertex
        Indices = new uint[width * height * 6]; // 2 triangles per quad

        int currentVertexIndex = 0;
        int currentIndex = 0;

        for (int z = 0; z <= height; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                // Calculate positions
                float posX = x * scale;
                float posZ = z * scale;

                // Get height from Perlin noise
                float y = Mathf.PerlinNoise(x * 0.1f, z * 0.1f, 0) * heightMultiplier;

                // Assign positions to Vertices array
                Vertices[currentVertexIndex] = posX;
                Vertices[currentVertexIndex + 1] = y;
                Vertices[currentVertexIndex + 2] = posZ;

                currentVertexIndex += 3;

                // Skip creating indices for the last row and column
                if (x < width && z < height)
                {
                    int topLeft = z * (width + 1) + x;
                    int topRight = topLeft + 1;
                    int bottomLeft = (z + 1) * (width + 1) + x;
                    int bottomRight = bottomLeft + 1;

                    // Triangle 1
                    Indices[currentIndex] = (uint)topLeft;
                    Indices[currentIndex + 1] = (uint)bottomLeft;
                    Indices[currentIndex + 2] = (uint)topRight;

                    // Triangle 2
                    Indices[currentIndex + 3] = (uint)topRight;
                    Indices[currentIndex + 4] = (uint)bottomLeft;
                    Indices[currentIndex + 5] = (uint)bottomRight;

                    currentIndex += 6;
                }
            }
        }
        
        // Log status
        LogTerrainGenerationStatus();
    }

    private void LogTerrainGenerationStatus()
    {
        ErrorLogger.SendError("Terrain generation completed successfully.", "TerrainGeneration.cs (Terrarium)", "NetworkListener");
        ErrorLogger.SendError($"Vertices Length: {Vertices.Length}, Indices Length: {Indices.Length}", "TerrainGeneration.cs (Terrarium)", "NetworkListener");

        // Log a few vertex positions for sanity check
        int logCount = Math.Min(Vertices.Length / 3, 10); // Limit log count
        for (int i = 0; i < logCount * 3; i += 3)
        {
            Console.WriteLine($"Vertex {i / 3}: ({Vertices[i]}, {Vertices[i + 1]}, {Vertices[i + 2]})");
        }
    }
}

*/