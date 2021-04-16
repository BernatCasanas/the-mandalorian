using System;
using DiamondEngine;

public class DebugGodmode : DiamondComponent
{
    public GameObject test = null;

    public void OnExecuteCheckbox(bool active)
    {
        if(active)
        {
            DebugOptionsHolder.godModeActive = true;
            Debug.Log("GOD MODE ACTIVE");
        }
        else
        {
            DebugOptionsHolder.godModeActive = false;
            Debug.Log("GOD MODE DEACTIVATED");
        }
    }

    public void Update()
    {

    }
}