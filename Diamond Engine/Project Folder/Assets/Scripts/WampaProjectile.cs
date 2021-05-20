using System;
using DiamondEngine;

public class WampaProjectile : DiamondComponent
{
	public float speed = 30.0f;
	public float lifeTime = 20.0f;
	public int damage = 10;

	public Vector3 targetPos = new Vector3(0, 0, 0);
	private Vector3 targetDirection = Vector3.zero;

	private bool to_destroy = false;
	private float timer = 1.10f;

	public void Update()
	{
		if (!to_destroy)
		{
			if (targetDirection == Vector3.zero)
				targetDirection = targetPos - gameObject.transform.globalPosition;

			gameObject.transform.localPosition += targetDirection.normalized * speed * Time.deltaTime;

			gameObject.transform.LookAt(targetDirection.normalized);
		}

		lifeTime -= Time.deltaTime;

		if (lifeTime < 0.0f)
			InternalCalls.Destroy(gameObject);
		else if (to_destroy)
        {
			timer -= Time.deltaTime;
			if (timer <= 0.0f)
				InternalCalls.Destroy(gameObject);
        }
	}

	public void OnTriggerEnter(GameObject triggeredGameObject)
	{
		if (triggeredGameObject.CompareTag("Player"))
		{
			PlayerHealth health = triggeredGameObject.GetComponent<PlayerHealth>();
			if (health != null)
				health.TakeDamage(damage);
		}
	}
	public void OnCollisionEnter(GameObject collidedGameObject)
	{
		Audio.PlayAudio(gameObject, "Play_Wampa_Projectile_Impact");
		//gameObject.transform.localPosition += new Vector3(0f, -10f, 0f);
		to_destroy = true;
	}
}