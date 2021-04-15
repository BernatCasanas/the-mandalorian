using System;
using DiamondEngine;

public class DebugGodmode : DiamondComponent
{
    public GameObject test = null;

    public void OnExecuteCheckbox(bool active)
    {
        Debug.Log("YEEEET");
    }

    public void Update()
    {

    }
}