using System;
using DiamondEngine;

public class DebugBossNoDmg : DiamondComponent
{
    public void OnExecuteCheckbox(bool active)
    {
        if (active)
        {
            DebugOptionsHolder.bossDmg = true;
            Debug.Log("BOSS NO DAMAGE ACTIVE");
        }
        else
        {
            DebugOptionsHolder.bossDmg = false;
            Debug.Log("BOSS NO DAMAGE DEACTIVATED");
        }
    }

}