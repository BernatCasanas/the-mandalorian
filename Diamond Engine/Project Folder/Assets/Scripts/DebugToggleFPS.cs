using System;
using DiamondEngine;

public class DebugToggleFPS : DiamondComponent
{
    public void OnExecuteCheckbox(bool active)
    {
        if (active)
        {
            DebugOptionsHolder.showFPS = true;
            Debug.Log("FPS SHOW ACTIVE");
        }
        else
        {
            DebugOptionsHolder.showFPS = false;
            Debug.Log("FPS SHOW DEACTIVATED");
        }
    }
}