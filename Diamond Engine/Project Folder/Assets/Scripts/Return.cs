using System;
using DiamondEngine;

public class Return : DiamondComponent
{
	public GameObject controlsWindow = null;
	public GameObject bigBrother = null;
	private Pause aux = null;
	public void OnExecuteButton()
	{
		if (gameObject.Name == "Return")
        {
			bigBrother.Enable(true);
			bigBrother.Enable(true);
			if (bigBrother.Name == "PauseMenu")
			{
				aux = bigBrother.GetComponent<Pause>();
				aux.DisplayBoons();
			}
			controlsWindow.Enable(false);
		}
	}
	public void Update()
	{
	}

}