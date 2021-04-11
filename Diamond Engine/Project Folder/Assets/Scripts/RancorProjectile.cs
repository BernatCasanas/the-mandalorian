using System;
using DiamondEngine;

public class RancorProjectile : DiamondComponent
{
	public float speed = 4.0f;
	public float lifeTime = 20.0f;
	public int damage = 10;

	public Vector3 targetPos = new Vector3(0, 0, 0);	//Set from Rancor.cs


	public void Update()
	{
		Vector3 direction = targetPos - gameObject.transform.globalPosition;

		gameObject.transform.localPosition += direction.normalized * speed * Time.deltaTime;

		lifeTime -= Time.deltaTime;

		if (lifeTime < 0.0f)
			InternalCalls.Destroy(gameObject);
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
}