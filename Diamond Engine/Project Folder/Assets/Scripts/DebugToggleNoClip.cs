using System;
using DiamondEngine;

public class DebugToggleNoClip : DiamondComponent
{
     public void OnExecuteCheckbox(bool active)
    {
        if (active)
        {
            DebugOptionsHolder.noClip = true;
            Debug.Log("NO CLIP MODE ACTIVE");

        }
        else
        {
            DebugOptionsHolder.noClip = false;
            Debug.Log("NO CLIP DEACTIVATED");
            //Core.instance.gameObject.GetComponent<>();
        }
    }

    public void Awake()
    {
        Navigation navigation = gameObject.GetComponent<Navigation>();
        if (navigation != null)
        {
            navigation.SetUIElementAsActive(DebugOptionsHolder.noClip);
            navigation.Select();
        }
    }


    public void Update()
    {
    }
}