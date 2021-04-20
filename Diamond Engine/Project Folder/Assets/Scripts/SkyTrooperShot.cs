using System;
using DiamondEngine;

public class SkyTrooperShot : DiamondComponent
{
	ParticleSystem particles = null;
	Vector2 expectedVelocity = null;

	public float travelTime = 2.0f;

	Vector3 targetPosition = null;
	float time = 0.0f;
	float deleteTimer = 0.0f;

	public void Update()
	{


		if(deleteTimer > 0.0f)
        {
			deleteTimer -= Time.deltaTime;

			if(deleteTimer <= 0.0f)
            {
				InternalCalls.Destroy(gameObject);
			}
        }
	}

	public void SetTarget(Vector3 target)
    {
		targetPosition = target;

		Vector3 direction = targetPosition - gameObject.transform.globalPosition;


    }

	public void OnCollisionEnter(GameObject collidedGameObject)
	{
		if(particles != null)
			particles.Play();

		deleteTimer = 5.0f;
	}
}