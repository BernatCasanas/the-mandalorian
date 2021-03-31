using System;
using DiamondEngine;

public class PushSkill : DiamondComponent
{
	public float speed = 60.0f;
	public float maxLifeTime = 5.0f;

	private float currentLifeTime = 0.0f;

	public void Update()
	{
		currentLifeTime += Time.deltaTime;

		gameObject.transform.localPosition += gameObject.transform.GetForward() * (speed * Time.deltaTime);

		if (currentLifeTime >= maxLifeTime)
		{
			InternalCalls.Destroy(this.gameObject);
		}
	}

}