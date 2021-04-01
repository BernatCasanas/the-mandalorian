using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DiamondEngine
{
    public class Transform2D : DiamondComponent
    {
        public Transform2D()
        {
            type = ComponentType.TRANSFORM_2D;
        }

        public extern Vector3 pos
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;
        }
        public extern float rot
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;
        }


        public extern Vector3 lPos
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;

            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            set;
        }

        public extern float lRot
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;

            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            set;
        }

        public extern Vector3 size
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;

            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            set;
        }

        //We pass Vec3 because the engine does not support Vec2 for now
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetLocalTransform(Vector3 lPos,float lRot,Vector3 size);

    }
}
