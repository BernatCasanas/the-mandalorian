using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DiamondEngine
{
    public class Navigation : DiamondComponent
    {
        public Navigation()
        {
            type = ComponentType.NAVIGATION;
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetLeftNavButton(GameObject button_mapped);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetRightNavButton(GameObject button_mapped);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetUpNavButton(GameObject button_mapped);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetDownNavButton(GameObject button_mapped);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void Select();

        public extern bool is_selected
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;
        }

    }


}
