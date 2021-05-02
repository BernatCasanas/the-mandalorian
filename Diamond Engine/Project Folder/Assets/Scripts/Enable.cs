using System;
using DiamondEngine;

public class Enable : DiamondComponent
{
	public GameObject enable = null;
	public GameObject disable = null;
	public GameObject disable2 = null;
	public GameObject select = null;

	public void OnExecuteButton()
    {
		Debug.Log("-----------------------Executing!");

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
			Debug.Log("Enable " + enable.Name);
		}

		if (select != null)
		{
			Navigation navComponent = select.GetComponent<Navigation>();

			if (navComponent != null)
			{
				navComponent.Select();
				Debug.Log("Select successful for: " + select.Name);
			}
			else Debug.Log("Select FAILED for: " + select.Name);
		}
    }
}