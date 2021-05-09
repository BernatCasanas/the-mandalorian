using System;
using DiamondEngine;

public class MoffGideonSword : DiamondComponent
{
	GameObject hand = null;

	float throwTimer = 0.0f;

	float throwTime = 0.0f;
	float throwSpeed = 4.5f;
	float throwRange = 5.0f;

	Vector3 throwDirection = null;

	public void Awake()
    {
		throwTime = throwRange / throwSpeed;
    }

	public void Update()
	{
		if(throwTimer > 0.0f)
        {
			gameObject.transform.localPosition += throwDirection * throwSpeed * Time.deltaTime;
        }
	}

	public void ThrowSword(Vector3 direction)
    {
		throwDirection = direction.normalized;
		throwTimer = throwTime;
    }

}