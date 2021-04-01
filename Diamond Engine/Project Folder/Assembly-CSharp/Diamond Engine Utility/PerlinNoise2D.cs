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

        public float GetNoiseAt(Vector2 coords)
        {
            float value = 0.0f;
            float currentScale = scale;
            float normalizeFactor = 0.0f;

            for (int i = 0; i < octaves; ++i)
            {
                value += GetNoiseSingle(coords) * currentScale;
                normalizeFactor += currentScale;
                coords *= 2.0f;
                currentScale /= lacunarity;
            }


            return value / normalizeFactor;
        }

        float GetNoiseSingle(Vector2 coord)
        {
            //gets the int part of the coords
            Vector2 i = new Vector2((float)Math.Floor(coord.x), (float)Math.Floor(coord.y));

            //gets the decimal part of the coords
            Vector2 f = Fract(coord);

            //multiply by 6.283 (2*pi)
            float twoPi = (float)(Math.PI * 2);
            float tl = Rand(i) * twoPi;//random number between i1 and i2
            float tr = Rand(i + new Vector2(1.0f, 0.0f)) * twoPi;//random number between i1 and i2 + vec2(1,0)
            float bl = Rand(i + new Vector2(0.0f, 1.0f)) * twoPi;//random number between i1 and i2 + vec2(0,1)
            float br = Rand(i + new Vector2(1.0f, 1.0f)) * twoPi;//random number between i1 and i2 + vec2(1,1)


            float tlSin = (float)Math.Sin(tl);
            float tlCos = (float)Math.Cos(tl);
            float trSin = (float)Math.Sin(tr);
            float trCos = (float)Math.Cos(tr);
            float blSin = (float)Math.Sin(bl);
            float blCos = (float)Math.Cos(bl);
            float brSin = (float)Math.Sin(br);
            float brCos = (float)Math.Cos(br);

            Vector2 tlVec = new Vector2(-tlSin, tlCos);
            Vector2 trVec = new Vector2(-trSin, trCos);
            Vector2 blVec = new Vector2(-blSin, blCos);
            Vector2 brVec = new Vector2(-brSin, brCos);

            float tlDot = Vector2.Dot(tlVec, f);
            float trDot = Vector2.Dot(trVec, f - new Vector2(1.0f, 0.0f));
            float blDot = Vector2.Dot(blVec, f - new Vector2(0.0f, 1.0f));
            float brDot = Vector2.Dot(brVec, f - new Vector2(1.0f, 1.0f));


            Vector2 cubic = f * f * (3.0f - 2.0f * f);

            float topMix = Mathf.Lerp(tlDot, trDot, cubic.x);
            float botMix = Mathf.Lerp(blDot, brDot, cubic.x);
            float wholeMix = Mathf.Lerp(topMix, botMix, cubic.y);



            return Mathf.Clamp(wholeMix + 0.5f, 0.0f, 1.0f);

        }

        float Rand(Vector2 coord)
        {
            // prevents randomness decreasing from coordinates too large
            coord = coord % 10000.0f;
            // returns "random" float between 0 and 1
            return Fract((float)Math.Sin(Vector2.Dot(coord, new Vector2(12.9898f, 78.233f))) * 43758.5453f);
        }

        Vector2 Rand2(Vector2 coord)
        {
            // prevents randomness decreasing from coordinates too large
            coord = coord % 10000.0f;
            // returns "random" vec2 with x and y between 0 and 1
            Vector2 res = new Vector2(Vector2.Dot(coord, new Vector2(127.1f, 311.7f)), Vector2.Dot(coord, new Vector2(269.5f, 183.3f)));
            return Fract(new Vector2((float)Math.Sin(res.x), (float)Math.Sin(res.y)) * 43758.5453f);
        }

        float Fract(float f)
        {
            int floorF = (int)Math.Floor(f);
            return f - floorF;
        }
        Vector2 Fract(Vector2 f)
        {
            int floorFX = (int)Math.Floor(f.x);
            int floorFY = (int)Math.Floor(f.y);
            return new Vector2(f.x - floorFX, f.y - floorFY);
        }
    }
}
