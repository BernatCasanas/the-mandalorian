using System;
using DiamondEngine;

public class MoffGideonSword : DiamondComponent
{
	public GameObject parent = null;

	float throwTimer = 0.0f;

	float throwSpeed = 15f;
	float throwRange = 20.0f;

	Vector3 throwDirection = null;

	public void Awake()
    {

    }

	public void Update()
	{
		Debug.Log(throwTimer.ToString());
		if(throwTimer > 0.0f)
        {
			throwTimer -= Time.deltaTime;
			gameObject.transform.localPosition += throwDirection * throwSpeed * Time.deltaTime;
        }
	}

	public void ThrowSword(Vector3 direction)
    {

		throwDirection = direction.normalized;
		throwTimer = throwRange / throwSpeed;

		float angle = (float)Math.Atan2(direction.x, direction.z);

		if (Math.Abs(angle * Mathf.Rad2Deg) < 1.0f)
			return;

		Quaternion dir = Quaternion.RotateAroundAxis(Vector3.up, angle);

		gameObject.transform.localRotation = dir;
	}

}