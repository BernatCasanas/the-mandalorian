using System;
using DiamondEngine;

public class DebugShowTris : DiamondComponent
{
    public void OnExecuteCheckbox(bool active)
    {
        if (active)
        {
            DebugOptionsHolder.showTris = true;
            Debug.Log("SHOW TRIS ACTIVE");
        }
        else
        {
            DebugOptionsHolder.showTris = false;
            Debug.Log("SHOW TRIS DEACTIVATED");
        }
    }
}