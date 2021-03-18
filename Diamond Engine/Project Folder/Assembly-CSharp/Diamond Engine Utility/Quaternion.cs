﻿using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace DiamondEngine
{

    [StructLayout(LayoutKind.Sequential)]
    public partial class Quaternion /*: IEquatable<Vector3>*/
    {

        public float x;
        public float y;
        public float z;
        public float w;

        public float this[int index] //Unity does this, is not the fastest way but maybe it's the way to fix the transform bug?
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;
                    default:
                        throw new IndexOutOfRangeException("Invalid Quaternion index!");
                }
            }

            set
            {
                switch (index)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    case 2: z = value; break;
                    case 3: w = value; break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Quaternion index!");
                }
            }
        }

        public static Quaternion operator *(Quaternion a, float d) { return new Quaternion(a.w*d, a.x * d, a.y * d, a.z * d); }
        public static Quaternion operator +(Quaternion a, Quaternion b) { return new Quaternion(a.w + b.w, a.x + b.x, a.y + b.y, a.z + b.z); }

        public Quaternion(float x, float y, float z, float w) 
        { 
            this.x = x; this.y = y; this.z = z; this.w = w; 
        }

        //yaw(Z), pitch(Y), roll(X)
        public Quaternion(float yaw, float pitch, float roll)
        {
            float cy = (float)Math.Cos(yaw * 0.5);
            float sy = (float)Math.Sin(yaw * 0.5);
            float cp = (float)Math.Cos(pitch * 0.5);
            float sp = (float)Math.Sin(pitch * 0.5);
            float cr = (float)Math.Cos(roll * 0.5);
            float sr = (float)Math.Sin(roll * 0.5);

            this.w = cr * cp * cy + sr * sp * sy;
            this.x = sr * cp * cy - cr * sp * sy;
            this.y = cr * sp * cy + sr * cp * sy;
            this.z = cr * cp * sy - sr * sp * cy;
        }

        static readonly Quaternion identityQuaternion = new Quaternion(0F, 0F, 0F, 1F);
        public static Quaternion identity
        {
            get
            {
                return identityQuaternion;
            }
        }

        public void Set(float newX, float newY, float newZ, float newW)
        {
            x = newX; y = newY; z = newZ; w = newW;
        }

        //Rotate an angle(radiants) aroun an axis
        public static Quaternion RotateAroundAxis(Vector3 axis, float angle)
        {
            Quaternion ret = Quaternion.identity;

            float factor = (float)Math.Sin(angle / 2.0);

            ret.x = axis.x * factor;
            ret.y = axis.y * factor;
            ret.z = axis.z * factor;

            ret.w = (float)Math.Cos(angle / 2.0);

            //InternalCalls.CSLog(ret);
            //ret = ret.normalized;

            return ret;
        }

        public static Quaternion Slerp(Quaternion q1, Quaternion q2, float t)
        {
            float omega = (float)Math.Acos(Dot(q1, q2));
            Quaternion p1 = q1 * (float)(Math.Sin((1 - t) * omega) / Math.Sin(omega));
            Quaternion p2 = q2 * (float)(Math.Sin(t * omega) / Math.Sin(omega));
            return p1 + p2;
        }

        public static float Dot(Quaternion a, Quaternion b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        public static Quaternion Normalize(Quaternion q)
        {
            float mag = (float)Math.Sqrt(Dot(q, q));

            if (mag < float.Epsilon)
                return Quaternion.identity;

            return new Quaternion(q.x / mag, q.y / mag, q.z / mag, q.w / mag);
        }

        public void Normalize()
        {
            Quaternion norm = Normalize(this);
            x = norm.x;
            y = norm.y;
            z = norm.z;
            w = norm.w;
        }

        public Quaternion normalized
        {
            get 
            {
                return Normalize(this); 
            }
        }

        public static Quaternion operator *(Quaternion q1, Quaternion q2)
        {
            return new Quaternion(
                q1.w * q2.x + q1.x * q2.w + q1.y * q2.z - q1.z * q2.y,
                q1.w * q2.y + q1.y * q2.w + q1.z * q2.x - q1.x * q2.z,
                q1.w * q2.z + q1.z * q2.w + q1.x * q2.y - q1.y * q2.x,
                q1.w * q2.w - q1.x * q2.x - q1.y * q2.y - q1.z * q2.z);
        }


        public override string ToString()
        {
            return x.ToString() + ", " + y.ToString() + ", " + z.ToString() + ", " + w.ToString();
        }

    }
}
