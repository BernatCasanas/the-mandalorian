using System;
using DiamondEngine;

public class AreaLightTest : DiamondComponent
{
	private float timer = 0.0f;
	public void Update()
	{
		timer += Time.deltaTime;

        if (timer > 2.0f)
        {
			//gameObject.GetComponent<AreaLight>().ActivateLight();
        }
	}

}