using System;
using DiamondEngine;

public class DebugToggleFPS : DiamondComponent
{
    public void OnExecuteCheckbox(bool active)
    {
        if (active)
        {
            DebugOptionsHolder.showFPS = true;
            Debug.Log("FPS SHOW ACTIVE");
        }
        else
        {
            DebugOptionsHolder.showFPS = false;
            Debug.Log("FPS SHOW DEACTIVATED");
        }
    }

    public void Awake()
    {
        Navigation navigation = gameObject.GetComponent<Navigation>();
        if (navigation != null)
        {
            navigation.SetUIElementAsActive(DebugOptionsHolder.showFPS);
            navigation.Select();
        }
    }


    public void Update()
    {

    }
}