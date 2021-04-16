using System;
using DiamondEngine;

public class DebugReturn : DiamondComponent
{
    public GameObject window = null;

    public void OnExecuteButton()
    {
        if (window != null)
            InternalCalls.Destroy(window);
    }

}