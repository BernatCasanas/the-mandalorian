using System;
using DiamondEngine;

public class PushSkill : DiamondComponent
{
	public float speed = 60.0f;
	public float maxLifeTime = 5.0f;
	public GameObject pushParticles = null;
	private ParticleSystem push = null;

	private float currentLifeTime = 0.0f;

	public void Awake()
    {
		if (pushParticles != null)
			push = pushParticles.GetComponent<ParticleSystem>();

		if (push != null)
			push.Play();
    }

	public void Update()
	{
		currentLifeTime += Time.deltaTime;

		gameObject.transform.localPosition += gameObject.transform.GetForward() * (speed * Time.deltaTime);

		float modifier = 1;
		if (Core.instance.HasStatus(STATUS_TYPE.GRO_PUSH))
			modifier *= Core.instance.GetStatusData(STATUS_TYPE.GRO_PUSH).severity;

		if (Core.instance.HasStatus(STATUS_TYPE.YODA_FORCE))
			modifier *= Core.instance.GetStatusData(STATUS_TYPE.YODA_FORCE).severity;

		if (currentLifeTime >= maxLifeTime * modifier)
		{
			InternalCalls.Destroy(this.gameObject);
		}



	}

}