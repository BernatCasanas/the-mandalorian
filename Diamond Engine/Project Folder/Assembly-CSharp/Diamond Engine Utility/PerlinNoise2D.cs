using System;

namespace DiamondEngine
{
    class PerlinNoise2D
    {
        Random rand = new Random();
        int octaves = 1;
        float scale = 1.0f;
        float lacunarity = 0.5f;

        public void GenerateNoise(int octaves, float scale, float lacunarity = 0.5f)
        {
            this.octaves = (int)Mathf.Clamp(octaves, 1.0f, 12.0f);
            this.scale = scale;
            this.lacunarity = lacunarity;
            rand = new Random();
        }
        public void GenerateNoise(int seed, int octaves, float scale, float lacunarity = 2.0f)
        {
            this.octaves = (int)Mathf.Clamp(octaves, 1.0f, 12.0f);
            this.scale = scale;
            this.lacunarity = lacunarity;
            rand = new Random(seed);
        }

        public float GetNoiseAt(float x, float y)
        {
            float value = 0.0f;
            float currentScale = scale;
            float normalizeFactor = 0.0f;

            for (int i = 0; i < octaves; ++i)
            {
                value += GetNoiseSingle(x, y) * currentScale;
                normalizeFactor += currentScale;
                x *= 2.0f;
                y *= 2.0f;
                currentScale /= lacunarity;
            }


            return value / normalizeFactor;
        }

        float GetNoiseSingle(float x, float y)
        {
            //gets the int part of the coords
            Vector3 i = new Vector3((float)Math.Floor(x), (float)Math.Floor(y), 0.0f);

            //gets the decimal part of the coords
            Vector3 f = new Vector3(x - i.x, y - i.y);

            //multiply by 6.283 (2*pi)
            float twoPi = (float)(Math.PI * 2);
            float tl = (float)(i.x + rand.NextDouble() * (i.y - i.x)) * twoPi;//random number between i1 and i2
            float tr = (float)(i.x + 1 + rand.NextDouble() * (i.y - i.x + 1)) * twoPi;//random number between i1 and i2 + vec2(1,0)
            float bl = (float)(i.x + rand.NextDouble() * (i.y + 1 - i.x)) * twoPi;//random number between i1 and i2 + vec2(0,1)
            float br = (float)(i.x + 1 + rand.NextDouble() * (i.y + 1 - i.x + 1)) * twoPi;//random number between i1 and i2 + vec2(1,1)


            float tlSin = (float)Math.Sin(tl);
            float tlCos = (float)Math.Cos(tl);
            float trSin = (float)Math.Sin(tr);
            float trCos = (float)Math.Cos(tr);
            float blSin = (float)Math.Sin(bl);
            float blCos = (float)Math.Cos(bl);
            float brSin = (float)Math.Sin(br);
            float brCos = (float)Math.Cos(br);

            Vector3 tlVec = new Vector3(-tlSin, tlCos, 0.0f);
            Vector3 trVec = new Vector3(-trSin, trCos, 0.0f);
            Vector3 blVec = new Vector3(-blSin, blCos, 0.0f);
            Vector3 brVec = new Vector3(-brSin, brCos, 0.0f);

            float tlDot = Vector3.Dot(tlVec, f);
            float trDot = Vector3.Dot(tlVec, f - new Vector3(1.0f, 0.0f, 0.0f));
            float blDot = Vector3.Dot(tlVec, f - new Vector3(0.0f, 1.0f, 0.0f));
            float brDot = Vector3.Dot(tlVec, f - new Vector3(1.0f, 1.0f, 0.0f));


            Vector3 cubic = f * f * (3.0f - 2.0f * f);

            float topMix = Mathf.Lerp(tlDot, trDot, cubic.x);
            float botMix = Mathf.Lerp(blDot, brDot, cubic.x);
            float wholeMix = Mathf.Lerp(topMix, botMix, cubic.y);



            return Mathf.Clamp(wholeMix + 0.5f, 0.0f, 1.0f);

        }
    }
}
