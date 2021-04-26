using System;
using DiamondEngine;

public class AtackBosslv2 : DiamondComponent
{
	public float damage = 20f;

	public void Update()
	{

	}

	public void OnTriggerEnter(GameObject triggeredGameObject)
	{
		if (triggeredGameObject.CompareTag("Player"))
		{
			PlayerHealth health = triggeredGameObject.GetComponent<PlayerHealth>();
			if (health != null)
				health.TakeDamage((int)damage);
		}
	}
}