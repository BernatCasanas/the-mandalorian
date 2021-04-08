using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DiamondEngine
{
    public class Button : DiamondComponent
    {
        public Button()
        {
            type = ComponentType.BUTTON;
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void ChangeSprites(int pressed, int hovered, int notHovered);

    }


}
