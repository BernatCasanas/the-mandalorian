using System;
using DiamondEngine;

public class Enable : DiamondComponent
{
	public GameObject enable;
	public GameObject disable;
	
	public void OnExecuteButton()
    {
		Debug.Log("Executing!");

		if (enable != null)
		{
			enable.Enable(true);
			Debug.Log(enable.Name + " is enabled!");
		}

		if (disable != null)
		{
			disable.Enable(false);
			Debug.Log(disable.Name + " is disabled!");
		}
    }
}