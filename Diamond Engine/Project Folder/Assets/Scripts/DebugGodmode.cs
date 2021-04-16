using System;
using DiamondEngine;

public class DebugGodmode : DiamondComponent
{

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

    public void Awake()
    { 
        Navigation navigation = gameObject.GetComponent<Navigation>();
        if (navigation != null)
        {
            navigation.SetUIElementAsActive(DebugOptionsHolder.godModeActive);
            navigation.Select();
        }
    }


    public void Update()
    {
    }
}