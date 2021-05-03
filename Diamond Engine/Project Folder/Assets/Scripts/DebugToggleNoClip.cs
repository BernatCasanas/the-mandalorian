using System;
using DiamondEngine;

public class DebugToggleNoClip : DiamondComponent
{
    /*public void Awake()
    {
        
    }*/

    public void OnExecuteCheckbox(bool active)
    {
        if (active)
        {
            DebugOptionsHolder.noClip = true;
            Debug.Log("NO CLIP MODE ACTIVE");
            Core.instance.gameObject.GetComponent<PlayerHealth>().ToggleNoClip(true);
        }
        else
        {
            DebugOptionsHolder.noClip = false;
            Debug.Log("NO CLIP DEACTIVATED");
            Core.instance.gameObject.GetComponent<PlayerHealth>().ToggleNoClip(false);
        }
    }
}