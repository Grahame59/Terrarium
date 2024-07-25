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
            _p = new int[512];
            for (int i = 0; i < 512; i++)
            {
                _p[i] = _permutation[i % 256];
            }
        }

        private static float Fade(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        private static float Lerp(float t, float a, float b)
        {
            return a + t * (b - a);
        }

        private static float Grad(int hash, float x, float y)
        {
            int h = hash & 15;
            float u = h < 8 ? x : y;
            float v = h < 4 ? y : h == 12 || h == 14 ? x : 0;
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }

        public static float PerlinNoise(float x, float y)
        {
            int X = (int)Math.Floor(x) & 255;
            int Y = (int)Math.Floor(y) & 255;

            x -= (float)Math.Floor(x);
            y -= (float)Math.Floor(y);

            float u = Fade(x);
            float v = Fade(y);

            int aa, ab, ba, bb;
            aa = _p[_p[X] + Y];
            ab = _p[_p[X] + Y + 1];
            ba = _p[_p[X + 1] + Y];
            bb = _p[_p[X + 1] + Y + 1];

            float res = Lerp(v, Lerp(u, Grad(aa, x, y), Grad(ba, x - 1, y)),
                                Lerp(u, Grad(ab, x, y - 1), Grad(bb, x - 1, y - 1)));

            return (res + 1) / 2; // Normalize to [0, 1]
        }
    }
}
