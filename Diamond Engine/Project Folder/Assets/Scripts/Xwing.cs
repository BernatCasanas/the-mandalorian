using System;
using DiamondEngine;

public class Xwing : DiamondComponent
{
	public GameObject sparks = null;
	private ParticleSystem partSpark = null;

	public float timeBetweenSparks = 2.0f;
	public float stopTime = 2.0f;
	private float timer = 0;

	public void Awake()
	{
		if (sparks != null)
		{
			partSpark = sparks.GetComponent<ParticleSystem>();
		}
	}

	public void Update()
	{
		timer += Time.deltaTime;
		if (timer >= stopTime)
		{
			if (partSpark != null)
				partSpark.Stop();
		}
		if (timer >= timeBetweenSparks)
		{
			if (partSpark != null )
				partSpark.Play();
			timer = 0;
		}
	}

}