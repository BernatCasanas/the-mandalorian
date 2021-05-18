using System;
using DiamondEngine;

public class AreaLTest : DiamondComponent
{
	public void Update()
	{
		AreaLight light = gameObject.GetComponent<AreaLight>();

		if (light == null)
        {
			Debug.Log("Cant find area light");
			return;
        }

		float dst = light.GetFadeDistance();

		dst += 10 * Time.deltaTime;

		light.SetFadeDistance(dst);
	}

}