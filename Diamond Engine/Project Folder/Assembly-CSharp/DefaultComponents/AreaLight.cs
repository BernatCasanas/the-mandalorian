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
        public extern void ActivateLight();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void DeactivateLight();
    }


}
