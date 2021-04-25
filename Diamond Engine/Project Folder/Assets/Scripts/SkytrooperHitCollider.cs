using System;
using DiamondEngine;

public class SkytrooperHitCollider : DiamondComponent
{
	public GameObject damageCollider = null;
	private bool colliderMoved = false;

	public float destroyTimer = 0.0f;
	public float destroyTime = 6.0f;

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
}