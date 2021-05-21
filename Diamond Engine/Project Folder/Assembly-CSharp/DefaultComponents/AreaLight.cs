using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DiamondEngine
{
    public class AreaLight : DiamondComponent
    {
        public AreaLight()
        {
            type = ComponentType.AREA_LIGHT;
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern float GetIntensity();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetIntensity(float lIntensity);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern float GetFadeDistance();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetFadeDistance(float fDistance);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern float GetMaxDistance();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetMaxDistance(float mDistance);
    }


}
