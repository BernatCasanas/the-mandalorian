using System;
using DiamondEngine;

public class HUBSave : DiamondComponent
{
    public void OnExecuteButton()
    {
        DiamondPrefs.SaveData();
    }

}