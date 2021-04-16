using System;
using DiamondEngine;

namespace DiamondEngine
{
    public static class DebugOptionsHolder
    {
        public static bool godModeActive = false;
        public static bool showFPS = false;
        public static bool showTris = false;
        public static bool goToNextRoom = false;

        public static void ResetDebugOptions()
        {
            godModeActive = false;
            showFPS = false;
            showTris = false;
            goToNextRoom = false;
        }

    }
}