using System;
using DiamondEngine;

public class BH_DestructBox : DiamondComponent
{
	public GameObject thisReference;

	private bool triggered = false;
	public float explosionTime = 2.0f;
	private float timer = 0;
	public ParticleSystem partSys;

	public void Update()
	{
		if (triggered)
			timer += Time.deltaTime;

		if (timer >= explosionTime)
			InternalCalls.Destroy(thisReference);

	}

	public void OnTriggerEnter(GameObject triggeredGameObject)
	{
		partSys = gameObject.GetComponent<ParticleSystem>();

		if (partSys != null && !triggered)
			partSys.Play();

		triggered = true;
	}

}