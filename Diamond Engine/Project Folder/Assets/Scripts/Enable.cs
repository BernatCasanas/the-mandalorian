using System;
using DiamondEngine;

public class Enable : DiamondComponent
{
	public GameObject enable;
	public GameObject disable;
	public GameObject disable2;

	public void OnExecuteButton()
    {
		Debug.Log("Executing!");

		if (disable != null)
		{
			disable.EnableNav(false);
			Debug.Log("Disable " + disable.Name);
		}
		if (disable2 != null)
		{
			disable2.EnableNav(false);
			Debug.Log("Disable " + disable2.Name);
		}
		if (enable != null)
		{
			enable.EnableNav(true);
		}
    }
}