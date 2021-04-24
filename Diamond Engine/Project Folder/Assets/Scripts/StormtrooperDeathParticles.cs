using System;
using DiamondEngine;

public class StormtrooperDeathParticles : DiamondComponent
{
	public GameObject soulsObj;
	public GameObject deadObj;
	public GameObject waveObj;

	private ParticleSystem souls;
	private ParticleSystem dead;
	private ParticleSystem wave;

	public void Awake()
	{
		if (soulsObj != null)
			souls = soulsObj.GetComponent<ParticleSystem>();
		if (deadObj != null)
			dead = deadObj.GetComponent<ParticleSystem>();
		if (waveObj != null)
			wave = waveObj.GetComponent<ParticleSystem>();

		if (souls != null)
			souls.Play();
		if (dead != null)
			dead.Play();
		if (wave != null)
			wave.Play();
	}

	public void Update()
	{
		if (!souls.playing && !dead.playing && !wave.playing)
			InternalCalls.Destroy(gameObject);
	}

}