using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DiamondEngine
{
    class Animator
    {
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public static extern void Play(object gameObject, string animationName,float speed = 1.0f);


        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public static extern void Pause(object gameObject);


        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public static extern void Resume(object gameObject);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public static extern void SetSpeed(object gameObject, float speed);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public static extern string GetCurrentAnimation(object gameObject);


        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public static extern float GetAnimationDuration(object gameObject, string animationName);
    }
}
