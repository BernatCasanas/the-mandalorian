using System;
using DiamondEngine;

public class MuzleFire : DiamondComponent
{
	private bool started = false;

	public void Update()
	{
        if (started == false)
        {
			started = true;
			gameObject.GetComponent<ParticleSystem>().Play();
        }
	}

}