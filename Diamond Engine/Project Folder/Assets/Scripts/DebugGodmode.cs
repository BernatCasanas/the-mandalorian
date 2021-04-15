using System;
using DiamondEngine;

public class DebugGodmode : DiamondComponent
{
    private bool wathever = false;
    public void OnExecuteCheckbox(bool active)
    {
        Debug.Log("LOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOG");
         
        if (active)
            wathever = true;
        else
            wathever = true;
    }

    public void OnExecuteButton()
    {
        Debug.Log("LOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOG");
    }

    public void Update()
    {
        if (wathever == true)
        {
            Debug.Log("Whatever bruh");
            wathever = false;
        }
    }
}