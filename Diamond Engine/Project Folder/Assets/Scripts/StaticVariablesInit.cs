using System;
using DiamondEngine;

public class StaticVariablesInit : DiamondComponent
{
    bool start = true;

	public void Update()
	{
        if(start)
        {
            InitStaticVars();
            start = false;
        }
	}


    public static void InitStaticVars()
    {
        PlayerHealth.ResetMaxAndCurrentHPToDefault();
        Counter.ResetCounters();
        EnemyManager.ClearList();
    }
}