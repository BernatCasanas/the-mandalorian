using System;
using DiamondEngine;

public class HUBAudio : DiamondComponent
{
	private bool start = false;
	private void Start()
    {
		Audio.PlayAudio(gameObject, "Play_Cantine_Ambience");
		start = true;
    }
	public void Update()
	{
        if (!start)
        {
			Start();
        }
	}

}