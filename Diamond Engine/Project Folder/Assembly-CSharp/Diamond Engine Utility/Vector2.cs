using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using System.Collections;

namespace DiamondEngine
{

    [StructLayout(LayoutKind.Sequential)]
    public partial class Vector2 //We use class because struct needs to be boxed and unboxed but class doesn't
    {
        //public bool Equals(Vector2 other)
        //{
        //    return (x == other.x && y == other.y);
        //}
        public float x;
        public float y;

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector2 index!");
                }
            }

            set
            {
                switch (index)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector2 index!");
                }
            }
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2(float x, float y) { this.x = x; this.y = y; }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2(Vector3 a) { this.x = a.x; this.y = a.y; }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(float newX, float newY) { x = newX; y = newY; }

        //Mult
        public static Vector2 operator *(Vector2 a, Vector2 b) { return new Vector2(a.x * b.x, a.y * b.y); }
        public static Vector2 operator *(Vector2 a, float d) { return new Vector2(a.x * d, a.y * d); }
        public static Vector2 operator *(float d, Vector2 a) { return new Vector2(a.x * d, a.y * d); }

        //Sum
        public static Vector2 operator +(Vector2 a, Vector2 b) { return new Vector2(a.x + b.x, a.y + b.y); }
        public static Vector2 operator +(Vector2 a, float d) { return new Vector2(a.x + d, a.y + d); }
        public static Vector2 operator +(float d, Vector2 a) { return new Vector2(d + a.x, d + a.y); }
        //Div
        public static Vector2 operator /(Vector2 a, Vector2 b) { return new Vector2(a.x / b.x, a.y / b.y); }
        public static Vector2 operator /(Vector2 a, float d) { return new Vector2(a.x / d, a.y / d); }
        public static Vector2 operator /(float d, Vector2 a) { return new Vector2(d / a.x, d / a.y); }
        //Mod
        public static Vector2 operator %(Vector2 a, Vector2 b) { return new Vector2(a.x % b.x, a.y % b.y); }
        public static Vector2 operator %(Vector2 a, float d) { return new Vector2(a.x % d, a.y % d); }
        public static Vector2 operator %( float d,Vector2 a) { return new Vector2(d % a.x,d % a.y); }
        //Diff
        public static Vector2 operator -(Vector2 a, Vector2 b) { return new Vector2(a.x - b.x, a.y - b.y); }
        public static Vector2 operator -(Vector2 a, float d) { return new Vector2(a.x - d, a.y - d); }
        public static Vector2 operator -(float d, Vector2 a) { return new Vector2(d - a.x, d - a.y); }

        static readonly Vector2 zeroVector = new Vector2(0F, 0F);
        static readonly Vector2 oneVector = new Vector2(1F, 1F);
        static readonly Vector2 rightVector = new Vector2(1F, 0F);
        static readonly Vector2 upVector = new Vector2(0F, 1F);
        static readonly Vector2 positiveInfinityVector = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        static readonly Vector2 negativeInfinityVector = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

        public static Vector2 zero { get { return zeroVector; } }
        public static Vector2 one { get { return oneVector; } }
        public static Vector2 up { get { return upVector; } }
        public static Vector2 right { get { return rightVector; } }
        public static Vector2 positiveInfinity { get { return positiveInfinityVector; } }
        public static Vector2 negativeInfinity { get { return negativeInfinityVector; } }

        public static float Dot(Vector2 a, Vector2 b)
        {
            return ((a.x * b.x) + (a.y * b.y));
        }

        public static float Magnitude(Vector2 vector) { return (float)Math.Sqrt(vector.x * vector.x + vector.y * vector.y); }
        public float magnitude { get { return (float)Math.Sqrt((x * x) + (y * y)); } }

        public static Vector2 Normalize(Vector2 value)
        {
            float mag = Magnitude(value);
            if (mag > float.Epsilon)
                return value / mag;
            else
                return zero;
        }

        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            return (a + (b - a) * t);
        }
        public static Vector2 SlerpVector(Vector2 start, Vector2 end, float percent)
        {
            Vector3 newStart = new Vector3(start);
            Vector3 newEnd = new Vector3(end);
            Vector3 newResult = Vector3.SlerpVector(newStart, newEnd, percent);

            return new Vector2(newResult);
        }

        //public void Normalize()
        //{
        //    float mag = Magnitude(this);
        //    if (mag > float.Epsilon)
        //        this = this / mag;
        //    else
        //        this = zero;
        //}

        public Vector2 normalized { get { return Vector2.Normalize(this); } }


        public override string ToString()
        {
            return (this.x.ToString() + ", " + this.y.ToString());
        }

        public float Distance(Vector2 point)
        {
            return (float)Math.Sqrt(Math.Pow((point.x - this.x), 2) + Math.Pow((point.y - this.y), 2));
        }

        public float DistanceNoSqrt(Vector3 point)
        {
            return (float)(Math.Pow((point.x - this.x), 2) + Math.Pow((point.y - this.y), 2));
        }
    }
}
