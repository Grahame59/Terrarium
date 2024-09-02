/*
using System;
using Error;

namespace Terrarium
{
    public static class Mathf
    {
        private const int GradientMask = 15;
        private const int GradientBound1 = 8;
        private const int GradientBound2 = 4;
        private const int GradientBound3 = 12;
        private const int GradientBound4 = 14;

        private static readonly int[] Permutation;
        private static readonly int[] P;

        static Mathf()
        {
            Permutation = GeneratePermutationArray();

            if (Permutation.Length != 256)
            {
                ErrorLogger.SendError($"Permutation Length = {Permutation.Length}", "PerlinNoise.cs(Terrarium)", "NetworkListener");
                throw new InvalidOperationException("Permutation array must have exactly 256 elements.");
            }

            P = new int[512];
            for (int i = 0; i < 256; i++)
            {
                P[i] = Permutation[i];
                P[i + 256] = Permutation[i];
            }
        }

        private static int[] GeneratePermutationArray()
        {
            var permutation = new int[256];
            var rand = new Random();

            for (int i = 0; i < 256; i++)
            {
                permutation[i] = i;
            }

            for (int i = 255; i > 0; i--)
            {
                int j = rand.Next(i + 1);
                int temp = permutation[i];
                permutation[i] = permutation[j];
                permutation[j] = temp;
            }

            return permutation;
        }

        private static float Fade(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        private static float Lerp(float t, float a, float b)
        {
            return a + t * (b - a);
        }

        private static float Grad(int hash, float x, float y, float z)
        {
            int h = hash & GradientMask;
            float u = h < GradientBound1 ? x : y;
            float v = h < GradientBound2 ? y : (h == GradientBound3 || h == GradientBound4 ? x : z);
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }

        public static float PerlinNoise(float x, float y, float z)
        {
            int X = (int)Math.Floor(x) & 255;
            int Y = (int)Math.Floor(y) & 255;
            int Z = (int)Math.Floor(z) & 255;

            x -= (float)Math.Floor(x);
            y -= (float)Math.Floor(y);
            z -= (float)Math.Floor(z);

            float u = Fade(x);
            float v = Fade(y);
            float w = Fade(z);

            int aaa = P[P[P[X] + Y] + Z];
            int aba = P[P[P[X] + Y + 1] + Z];
            int aab = P[P[P[X] + Y] + Z + 1];
            int abb = P[P[P[X] + Y + 1] + Z + 1];
            int baa = P[P[P[X + 1] + Y] + Z];
            int bba = P[P[P[X + 1] + Y + 1] + Z];
            int bab = P[P[P[X + 1] + Y] + Z + 1];
            int bbb = P[P[P[X + 1] + Y + 1] + Z + 1];

            float x1 = Lerp(u, Grad(aaa, x, y, z), Grad(baa, x - 1, y, z));
            float x2 = Lerp(u, Grad(aba, x, y - 1, z), Grad(bba, x - 1, y - 1, z));
            float y1 = Lerp(v, x1, x2);

            x1 = Lerp(u, Grad(aab, x, y, z - 1), Grad(bab, x - 1, y, z - 1));
            x2 = Lerp(u, Grad(abb, x, y - 1, z - 1), Grad(bbb, x - 1, y - 1, z - 1));
            float y2 = Lerp(v, x1, x2);

            float result = (Lerp(w, y1, y2) + 1) / 2;

            return result;
        }
    }
}
*/