using System;
using DiamondEngine;

public class RancorProjectile : DiamondComponent
{
	public float speed = 4.0f;
	public float lifeTime = 20.0f;
	public int damage = 10;

	public Vector3 targetPos = new Vector3(0, 0, 0);    //Set from Rancor.cs
	private Vector3 targetDirection = Vector3.zero;


	public void Update()
	{
		if (targetDirection == Vector3.zero)
			targetDirection = targetPos - gameObject.transform.globalPosition;
		
		gameObject.transform.localPosition += targetDirection.normalized * speed * Time.deltaTime;

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
			InternalCalls.Destroy(gameObject);
		}
    }
}