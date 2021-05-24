using System;
using DiamondEngine;

public class BH_DestructBox : DiamondComponent
{
	public GameObject thisReference;
	public GameObject explosion = null;
	public GameObject wave = null;
	public GameObject mesh = null;


	private bool triggered = false;
	private bool triggeredByPlayer = true;
	public float explosionTime = 2.0f;
	private float timer = 0;
	private ParticleSystem partExp = null;
	private ParticleSystem partWave = null;
	public int explosion_damage = 0;
	bool firstFrame = true;

	public void Awake()
    {
	}

	public void Update()
	{
		if(firstFrame)
        {
			EnemyManager.AddProp(gameObject);
			firstFrame = false;
        }

		if (triggered)
			timer += Time.deltaTime;

		if (timer >= explosionTime)
        {
			InternalCalls.Destroy(thisReference);
		}

	}

	public void OnTriggerEnter(GameObject triggeredGameObject)
	{
		if((triggeredGameObject.CompareTag("Bullet") || triggeredGameObject.CompareTag("ChargeBullet") || triggeredGameObject.CompareTag("StormTrooperBullet")) && triggered == false)
        {
			if (triggeredGameObject.CompareTag("StormTrooperBullet"))
				triggeredByPlayer = false;

			if (explosion != null && wave != null)
			{
				partExp = explosion.GetComponent<ParticleSystem>();
				partWave = wave.GetComponent<ParticleSystem>();

				Audio.PlayAudio(gameObject, "Play_Barrel_Explosion");
				EnemyManager.RemoveProp(gameObject);
			}

			if (partExp != null && !triggered)
				partExp.Play();

			if (partWave != null && !triggered)
				partWave.Play();

			if (mesh != null)
				InternalCalls.Destroy(mesh);
			triggered = true;
			gameObject.DisableCollider();
			gameObject.EnableCollider();
		}
	}

	public void OnDestroy()
    {
		EnemyManager.RemoveProp(gameObject);
	}
}