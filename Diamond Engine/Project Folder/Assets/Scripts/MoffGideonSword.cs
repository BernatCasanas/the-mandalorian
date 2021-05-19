using System;
using DiamondEngine;

public class MoffGideonSword : DiamondComponent
{
	public GameObject parent = null;

	float throwTimer = 0.0f;

	float throwSpeed = 15f;

	public float damage = 10f;


	public void Awake()
    {

    }

	public void Update()
	{
		Debug.Log(throwTimer.ToString());
		if(throwTimer > 0.0f)
        {
			throwTimer -= Time.deltaTime;
			gameObject.transform.localPosition += gameObject.transform.GetForward() * throwSpeed * Time.deltaTime;
        }
	}

	public void ThrowSword(Vector3 direction, float range)
    {

		throwTimer = range / throwSpeed;

		float angle = (float)Math.Atan2(direction.x, direction.z);

		if (Math.Abs(angle * Mathf.Rad2Deg) < 1.0f)
			return;

		Quaternion dir = Quaternion.RotateAroundAxis(Vector3.up, angle);

		gameObject.transform.localRotation = dir;
	}

	public void OnCollisionEnter(GameObject collidedGameObject)
    {

		if (collidedGameObject.CompareTag("Column") || collidedGameObject.CompareTag("Wall"))
		{
			throwTimer = 0f;	
		}

		if(collidedGameObject.CompareTag("Player"))
        {
			PlayerHealth playerHealth = collidedGameObject.GetComponent<PlayerHealth>();
			if (playerHealth != null)
			{
				playerHealth.TakeDamage((int)(damage));
			}
		}
			
	}

}