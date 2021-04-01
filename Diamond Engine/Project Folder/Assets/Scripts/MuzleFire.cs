using System;
using DiamondEngine;

public class MuzleFire : DiamondComponent
{
	private bool started = false;
	public ParticleSystem partSys;

	public void Update()
	{
        if (started == false)
        {
			started = true;
			partSys = gameObject.GetComponent<ParticleSystem>();

			if(partSys != null)
			partSys.Play();
        }

        //if (partSys != null && partSys.playing == false)
        //    InternalCalls.Destroy(gameObject);
    }

}