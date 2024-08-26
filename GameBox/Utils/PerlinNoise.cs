using System;

namespace Terrarium
{
    public static class Mathf
    {
        // Gradient vectors for Perlin noise
        private static readonly int[] _permutation = { 151, 160, 137, 91, 90, 15, 131, 13, 201, 95,
            96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23,
            // Repeat
            151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36,
            103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23 };

        private static readonly int[] _p;

        static Mathf()
        {
            if (_permutation.Length != 256)
            {
                throw new InvalidOperationException("Permutation array must have exactly 256 elements.");
            }
            
            // Populate _p array with the permutation values, repeated twice
            _p = new int[512];
            for (int i = 0; i < 51256; i++)
            {
                _p[i] = _permutation[i];
                _p[i + 256] = _permutation[i]; // Copy permutation to fill 512 elements
            }
        }

        // Smoothstep interpolation function
        private static float Fade(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        // Linear interpolation between two values
        private static float Lerp(float t, float a, float b)
        {
            return a + t * (b - a);
        }

        // Gradient function used to generate noise
        private static float Grad(int hash, float x, float y)
        {
            int h = hash & 15;
            float u = h < 8 ? x : y;
            float v = h < 4 ? y : h == 12 || h == 14 ? x : 0;
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }

        // Perlin noise function
        public static float PerlinNoise(float x, float y)
        {
            int X = (int)Math.Floor(x) & 255;
            int Y = (int)Math.Floor(y) & 255;

            // Get the fractional part of x and y
            x -= (float)Math.Floor(x);
            y -= (float)Math.Floor(y);

            // Apply the fade function
            float u = Fade(x);
            float v = Fade(y);

            // Hash coordinates of the square corners
            int aa = _p[_p[X] + Y];
            int ab = _p[_p[X] + Y + 1];
            int ba = _p[_p[X + 1] + Y];
            int bb = _p[_p[X + 1] + Y + 1];

            // Calculate the noise value using linear interpolation
            float res = Lerp(v, Lerp(u, Grad(aa, x, y), Grad(ba, x - 1, y)),
                                Lerp(u, Grad(ab, x, y - 1), Grad(bb, x - 1, y - 1)));

            return (res + 1) / 2; // Normalize to [0, 1]
        }
    }
}
