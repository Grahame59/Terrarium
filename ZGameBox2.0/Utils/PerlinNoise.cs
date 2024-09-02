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

        private static readonly int[] Permutation = new int[256];
        private static readonly int[] P = new int[512];

        static Mathf()
        {
            try
            {
                GeneratePermutationArray(); // Initialize Permutation here

                if (Permutation.Length != 256)
                {
                    ErrorLogger.SendError($"Permutation Length = {Permutation.Length}", "PerlinNoise.cs (Terrarium)", "NetworkListener");
                    throw new InvalidOperationException("Permutation array must have exactly 256 elements.");
                }

                for (int i = 0; i < 256; i++)
                {
                    P[i] = Permutation[i];
                    P[i + 256] = Permutation[i];
                }
                
                ErrorLogger.SendDebug("Permutation array initialized and duplicated.", "PerlinNoise.cs (Terrarium)", "NetworkListener");
            }
            catch (Exception ex)
            {
                ErrorLogger.SendError($"Exception during static initialization: {ex.Message}", "PerlinNoise.cs (Terrarium)", "NetworkListener");
            }
        }

        private static void GeneratePermutationArray()
        {
            try
            {
                var rand = new Random();

                for (int i = 0; i < 256; i++)
                {
                    Permutation[i] = i;
                }

                for (int i = 255; i > 0; i--)
                {
                    int j = rand.Next(i + 1);
                    (Permutation[i], Permutation[j]) = (Permutation[j], Permutation[i]);
                }

                ErrorLogger.SendDebug("Permutation array generated successfully.", "PerlinNoise.cs (Terrarium)", "NetworkListener");
            }
            catch (Exception ex)
            {
                ErrorLogger.SendError($"Exception during permutation array generation: {ex.Message}", "PerlinNoise.cs (Terrarium)", "NetworkListener");
                throw;
            }
        }

        private static float Fade(float t)
        {
            try
            {
                float result = t * t * t * (t * (t * 6 - 15) + 10);
                //ErrorLogger.SendDebug($"Fade function result: {result}", "PerlinNoise.cs (Terrarium)", "NetworkListener");
                return result;
            }
            catch (Exception ex)
            {
                ErrorLogger.SendError($"Exception in Fade function: {ex.Message}", "PerlinNoise.cs (Terrarium)", "NetworkListener");
                throw;
            }
        }

        private static float Lerp(float t, float a, float b)
        {
            try
            {
                float result = a + t * (b - a);
                //ErrorLogger.SendDebug($"Lerp function result: {result}", "PerlinNoise.cs (Terrarium)", "NetworkListener");
                return result;
            }
            catch (Exception ex)
            {
                ErrorLogger.SendError($"Exception in Lerp function: {ex.Message}", "PerlinNoise.cs (Terrarium)", "NetworkListener");
                throw;
            }
        }

        private static float Grad(int hash, float x, float y, float z)
        {
            try
            {
                int h = hash & GradientMask;
                float u = h < GradientBound1 ? x : y;
                float v = h < GradientBound2 ? y : (h == GradientBound3 || h == GradientBound4 ? x : z);
                float gradResult = ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
                
                //ErrorLogger.SendDebug($"Grad function result for hash {hash}: {gradResult}", "PerlinNoise.cs (Terrarium)", "NetworkListener");
                return gradResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.SendError($"Exception in Grad function: {ex.Message}", "PerlinNoise.cs (Terrarium)", "NetworkListener");
                throw;
            }
        }

        public static float PerlinNoise(float x, float y, float z)
        {
            try
            {
                int X = (int)Math.Floor(x) & 255;
                int Y = (int)Math.Floor(y) & 255;
                int Z = (int)Math.Floor(z) & 255;

                x -= (float)Math.Floor(x);
                y -= (float)Math.Floor(y);
                z -= (float)Math.Floor(z);

                //ErrorLogger.SendDebug($"PerlinNoise inputs: X={X}, Y={Y}, Z={Z}, x={x}, y={y}, z={z}", "PerlinNoise.cs (Terrarium)", "NetworkListener");

                float u = Fade(x);
                float v = Fade(y);
                float w = Fade(z);

                //ErrorLogger.SendDebug($"Fade values: u={u}, v={v}, w={w}", "PerlinNoise.cs (Terrarium)", "NetworkListener");

                int aaa = P[P[P[X] + Y] + Z];
                int aba = P[P[P[X] + Y + 1] + Z];
                int aab = P[P[P[X] + Y] + Z + 1];
                int abb = P[P[P[X] + Y + 1] + Z + 1];
                int baa = P[P[P[X + 1] + Y] + Z];
                int bba = P[P[P[X + 1] + Y + 1] + Z];
                int bab = P[P[P[X + 1] + Y] + Z + 1];
                int bbb = P[P[P[X + 1] + Y + 1] + Z + 1];

                //ErrorLogger.SendDebug($"Gradient indices: aaa={aaa}, aba={aba}, aab={aab}, abb={abb}, baa={baa}, bba={bba}, bab={bab}, bbb={bbb}", "PerlinNoise.cs (Terrarium)", "NetworkListener");

                float x1 = Lerp(u, Grad(aaa, x, y, z), Grad(baa, x - 1, y, z));
                float x2 = Lerp(u, Grad(aba, x, y - 1, z), Grad(bba, x - 1, y - 1, z));
                float y1 = Lerp(v, x1, x2);

                float x1_ = Lerp(u, Grad(aab, x, y, z - 1), Grad(bab, x - 1, y, z - 1));
                float x2_ = Lerp(u, Grad(abb, x, y - 1, z - 1), Grad(bbb, x - 1, y - 1, z - 1));
                float y2 = Lerp(v, x1_, x2_);

                float result = (Lerp(w, y1, y2) + 1) / 2;

                //ErrorLogger.SendDebug($"PerlinNoise result: {result}", "PerlinNoise.cs (Terrarium)", "NetworkListener");

                return result;
            }
            catch (Exception ex)
            {
                ErrorLogger.SendError($"Exception in PerlinNoise function: {ex.Message}", "PerlinNoise.cs (Terrarium)", "NetworkListener");
                throw;
            }
        }
    }
}