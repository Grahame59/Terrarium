namespace GameBox.Utils
{
    public static class PerlinNoise
    {
        public static float[,] Generate(int width = 256, int height = 256, float scale = 20.0f)
        {
            float[,] noiseMap = new float[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float sampleX = x / scale;
                    float sampleY = y / scale;
                    float noiseValue = Mathf.PerlinNoise(sampleX, sampleY);
                    noiseMap[x, y] = noiseValue;
                }
            }
            return noiseMap;
        }
    }

    public static class Mathf
    {
        public static float PerlinNoise(float x, float y)
        {
            // Implement Perlin noise generation here
            // Placeholder: return a random value for now
            Random rand = new Random();
            return (float)rand.NextDouble();
        }
    }
}

