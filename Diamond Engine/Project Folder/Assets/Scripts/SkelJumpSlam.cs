using System;
using DiamondEngine;

public class SkelJumpSlam : DiamondComponent
{
	public float damageJumpSlam = 20f;

	public void Update()
	{

	}

	public void OnTriggerEnter(GameObject triggeredGameObject)
	{
		if (triggeredGameObject.CompareTag("Player"))
		{
			PlayerHealth health = triggeredGameObject.GetComponent<PlayerHealth>();
			if (health != null)
				health.TakeDamage((int)damageJumpSlam);
		}
	}
}