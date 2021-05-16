using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DiamondEngine
{
    public class Checkbox : DiamondComponent
    {
        public Checkbox()
        {
            type = ComponentType.CHECKBOX;
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void ChangeActive(bool checkboxActive);

    }


}
