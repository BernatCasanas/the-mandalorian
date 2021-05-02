using System;
using DiamondEngine;

public class R1TextController : DiamondComponent
{
	public GameObject dialog = null;
	private float counter = 0;
	private bool showtext = false;
	public void Update()
	{
		counter += Time.deltaTime;
		if (showtext)
		{
			showtext = false;
			dialog.Enable(true);
		}
	}

}