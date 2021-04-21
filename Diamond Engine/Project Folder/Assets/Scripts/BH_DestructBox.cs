using System;
using DiamondEngine;

public class BH_DestructBox : DiamondComponent
{
	public GameObject thisReference;
	public GameObject explosion = null;
	public GameObject wave = null;
	public GameObject mesh = null;


	private bool triggered = false;
	public float explosionTime = 2.0f;
	private float timer = 0;
	private ParticleSystem partExp = null;
	private ParticleSystem partWave = null;
	private BoxCollider boxColl = null;
	private SphereCollider sphereColl = null;

	public void Start()
    {
		boxColl = gameObject.GetComponent<BoxCollider>();
		sphereColl = gameObject.GetComponent<SphereCollider>();
    }

	public void Update()
	{
		if (triggered)
			timer += Time.deltaTime;

		if (timer >= explosionTime)
        {
			InternalCalls.Destroy(thisReference);
		}

	}

	public void OnTriggerEnter(GameObject triggeredGameObject)
	{
		if(triggeredGameObject.CompareTag("Bullet"))
        {
			if (explosion != null && wave != null)
			{
				partExp = explosion.GetComponent<ParticleSystem>();
				partWave = wave.GetComponent<ParticleSystem>();
			}

			if (partExp != null && !triggered)
				partExp.Play();

			if (partWave != null && !triggered)
				partWave.Play();

			if (mesh != null)
				InternalCalls.Destroy(mesh);
			sphereColl.active = true;
			triggered = true;
			boxColl.active = false;
		}
		
	}

}