using System;
using DiamondEngine;

public class DebugNextRoom : DiamondComponent
{

    public void OnExecuteButton()
    {
        Debug.Log("Go to next room");
        DebugOptionsHolder.goToNextRoom = true;
    }

}