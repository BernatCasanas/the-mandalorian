using System;
using DiamondEngine;

public class R1TextController : DiamondComponent
{
	public GameObject dialog = null;
	private float counter = 0;
	private bool showtext = true;
	public void Update()
	{
		counter += Time.deltaTime;
		if (Counter.roomEnemies <= 0 && counter > 5 && showtext)
		{
			showtext = false;
			dialog.Enable(true);
	
		}
	}

}