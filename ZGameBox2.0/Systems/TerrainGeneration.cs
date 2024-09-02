using System;
using System.Diagnostics; // For Stopwatch
using SkiaSharp;
using OpenTK.Mathematics;
using Error; // Custom error handling
using Terrarium; // Custom terrain generation

public class TerrainGeneration
{
    public class Mesh
    {
        public Vector3[] vertices { get; set; } = Array.Empty<Vector3>();
        public int[] triangles { get; set; } = Array.Empty<int>();
        public Vector3[] normals { get; set; } = Array.Empty<Vector3>();
    }

    public float[,] GenerateHeightmap(int width, int height, float scale)
    {
        try
        {
            if (width <= 0)
            {
                string message = $"Width must be a positive integer. (Received: {width})";
                ErrorLogger.SendError(message, "TerrainGeneration.cs (Terrarium)", "NetworkListener");
                throw new ArgumentException(message, nameof(width));
            }

            if (height <= 0)
            {
                string message = $"Height must be a positive integer. (Received: {height})";
                ErrorLogger.SendError(message, "TerrainGeneration.cs (Terrarium)", "NetworkListener");
                throw new ArgumentException(message, nameof(height));
            }

            if (scale <= 0)
            {
                string message = $"Scale must be greater than zero. (Received: {scale})";
                ErrorLogger.SendError(message, "TerrainGeneration.cs (Terrarium)", "NetworkListener");
                throw new ArgumentException(message, nameof(scale));
            }

            var heightmap = new float[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float nx = (x / (float)width) - 0.5f;
                    float ny = (y / (float)height) - 0.5f;
                    heightmap[x, y] = Mathf.PerlinNoise(nx * scale, ny * scale, 0);
                }
            }

            // Debug: Save heightmap as an image
            SaveHeightmapAsImage(heightmap, "heightmap.png");

            ErrorLogger.SendDebug($"Generated heightmap with dimensions {width}x{height} and scale {scale}.", "TerrainGeneration.cs (Terrarium)", "NetworkListener");
            return heightmap;
        }
        catch (Exception ex)
        {
            ErrorLogger.SendError($"Exception: {ex.Message}", "TerrainGeneration.cs (Terrarium)", "NetworkListener");
            throw; // Re-throw the exception after logging
        }
    }

    private void SaveHeightmapAsImage(float[,] heightmap, string filePath)
    {
        int width = heightmap.GetLength(0);
        int height = heightmap.GetLength(1);

        using (var bitmap = new SKBitmap(width, height))
        {
            using (var canvas = new SKCanvas(bitmap))
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int colorValue = (int)(heightmap[x, y] * 255);
                        var color = new SKColor((byte)colorValue, (byte)colorValue, (byte)colorValue);
                        bitmap.SetPixel(x, y, color);
                    }
                }
            }

            using (var image = SKImage.FromBitmap(bitmap))
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            {
                using (var stream = System.IO.File.OpenWrite(filePath))
                {
                    data.SaveTo(stream);
                }
            }
        }
    }

    public Mesh GenerateMesh(float[,] heightmap)
    {
        try
        {
            int width = heightmap.GetLength(0);
            int height = heightmap.GetLength(1);

            if (width <= 1 || height <= 1)
            {
                string message = $"Width and height must be greater than 1. (Received: width={width}, height={height})";
                ErrorLogger.SendError(message, "TerrainGeneration.cs (Terrarium)", "NetworkListener");
                throw new ArgumentException(message);
            }

            var vertices = new Vector3[width * height];
            var triangles = new int[(width - 1) * (height - 1) * 6];
            var normals = new Vector3[vertices.Length];

            int triIndex = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    vertices[x + y * width] = new Vector3(x, heightmap[x, y], y);

                    if (x < width - 1 && y < height - 1)
                    {
                        // Two triangles per square
                        triangles[triIndex++] = x + y * width;
                        triangles[triIndex++] = (x + 1) + y * width;
                        triangles[triIndex++] = x + (y + 1) * width;

                        triangles[triIndex++] = (x + 1) + y * width;
                        triangles[triIndex++] = (x + 1) + (y + 1) * width;
                        triangles[triIndex++] = x + (y + 1) * width;
                    }
                }
            }

            // Calculate normals
            for (int i = 0; i < triangles.Length; i += 3)
            {
                var v0 = vertices[triangles[i]];
                var v1 = vertices[triangles[i + 1]];
                var v2 = vertices[triangles[i + 2]];
                var normal = Vector3.Cross(v1 - v0, v2 - v0).Normalized();

                normals[triangles[i]] += normal;
                normals[triangles[i + 1]] += normal;
                normals[triangles[i + 2]] += normal;
            }

            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = normals[i].Normalized();
            }

            var mesh = new Mesh
            {
                vertices = vertices,
                triangles = triangles,
                normals = normals
            };

            ErrorLogger.SendDebug($"Generated mesh with {vertices.Length} vertices and {triangles.Length / 3} triangles.", "TerrainGeneration.cs (Terrarium)", "NetworkListener");
            return mesh;
        }
        catch (Exception ex)
        {
            ErrorLogger.SendError($"Exception: {ex.Message}", "TerrainGeneration.cs (Terrarium)", "NetworkListener");
            throw; // Re-throw the exception after logging
        }
    }

    public void GenerateAndVisualizeTerrain(int width, int height, float scale)
    {
        try
        {
            var heightmap = GenerateHeightmap(width, height, scale);
            var mesh = GenerateMesh(heightmap);

            // Here, you would integrate with your rendering system to display the mesh.
            // For example:
            // RenderMesh(mesh);

            ErrorLogger.SendDebug("Terrain visualization complete.", "TerrainGeneration.cs (Terrarium)", "NetworkListener");
        }
        catch (Exception ex)
        {
            ErrorLogger.SendError($"Exception during terrain generation and visualization: {ex.Message}", "TerrainGeneration.cs (Terrarium)", "NetworkListener");
        }
    }

    public void MeasurePerformance(int width, int height, float scale)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            GenerateAndVisualizeTerrain(width, height, scale);
            stopwatch.Stop();

            ErrorLogger.SendDebug($"Generation time for width={width}, height={height}, scale={scale}: {stopwatch.ElapsedMilliseconds} ms", "TerrainGeneration.cs (Terrarium)", "NetworkListener");
        }
        catch (Exception ex)
        {
            ErrorLogger.SendError($"Exception during performance measurement: {ex.Message}", "TerrainGeneration.cs (Terrarium)", "NetworkListener");
        }
    }
}
