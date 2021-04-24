using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DiamondEngine
{
    public class Material : DiamondComponent
    {
        public Material()
        {
            type = ComponentType.MATERIAL;
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetFloatUniform(String name_uniform, float value);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetIntUniform(String name_uniform, int value);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetVectorUniform(String name_uniform, object value);

    }


}
