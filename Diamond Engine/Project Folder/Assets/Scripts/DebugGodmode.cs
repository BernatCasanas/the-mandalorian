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
        if(DebugOptionsHolder.godModeActive)
        {
            Navigation nav = gameObject.GetComponent<Navigation>();
            if (nav != null)
            {
                nav.SetUIElementAsActive(false);
                nav.SetUIElementAsActive(true);
            }
        }
    }
    public void Update()
    {

    }
}