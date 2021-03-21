using System;
using DiamondEngine;

public class StaticVariablesInit : DiamondComponent
{
    public bool InitVariablesOnStart = true;
    bool start = true;
	public void Update()
	{
        if(InitVariablesOnStart && start)
        {
            InitStaticVars();
            start = false;
        }
	}


    public void InitStaticVars()
    {
        PlayerHealth.ResetMaxAndCurrentHPToDefault();
    }
}