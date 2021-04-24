using System;
using DiamondEngine;

public class SkytrooperHitCollider : DiamondComponent
{
	public GameObject damageCollider = null;
	private bool colliderMoved = false;

	public void Update()
	{
		if (damageCollider != null && colliderMoved)
        {
			InternalCalls.Destroy(damageCollider);
        }
	}

	public void OnTriggerEnter(GameObject triggeredGameObject)
	{
		if (triggeredGameObject.CompareTag("SkytrooperBullet"))
		{
			if(damageCollider != null)
            {
				Debug.Log("Bullet Collision");
				damageCollider.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
				colliderMoved = true;
            }
		}
	}
}