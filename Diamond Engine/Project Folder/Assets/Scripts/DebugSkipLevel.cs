using System;
using DiamondEngine;

public class DebugSkipLevel : DiamondComponent
{
    public void OnExecuteButton()
    {
        Debug.Log("Skip level");
        RoomSwitch.SwitchToLevel2();
    }
}