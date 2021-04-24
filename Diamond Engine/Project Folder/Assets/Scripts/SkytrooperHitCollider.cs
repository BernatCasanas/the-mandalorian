using System;
using DiamondEngine;

public class SkytrooperHitCollider : DiamondComponent
{
	public GameObject damageCollider = null;
	private bool colliderMoved = false;

	public float destroyTimer = 0.0f;
	public float destroyTime = 5.0f;

	public void Awake()
    {
		Debug.Log("Timer Awake");
		destroyTimer = destroyTime;
    }

	public void Update()
	{
		if(destroyTimer > 0.0f)
        {
			destroyTimer -= Time.deltaTime;

			if (destroyTimer <= 0.0f)
				InternalCalls.Destroy(gameObject);
        }

		if (damageCollider != null && colliderMoved)
        {
			InternalCalls.Destroy(gameObject);
        }
	}

	public void OnTriggerEnter(GameObject triggeredGameObject)
	{
		Debug.Log("Collision with: " + triggeredGameObject.tag);

		if (triggeredGameObject.CompareTag("SK_Bullet"))
		{
			Debug.Log("Bullet Collision");
			if(damageCollider != null)
            {
				Debug.Log("Damage Collider");
				damageCollider.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
				colliderMoved = true;
            }
		}
	}
}