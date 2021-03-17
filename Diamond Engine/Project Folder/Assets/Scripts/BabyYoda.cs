using System;
using DiamondEngine;

public class BabyYoda : DiamondComponent
{
	//Vertical Movement
	public float verticalSpeed = 0.5f;
	public float verticalTimeInterval = 0.8f;
	private float verticalTimer = 0.0f;
	private bool moveDown = true;

	public void Update()
	{
		Move();
	}

	private void Move()
    {
		MoveVertically();
	}

	private void MoveVertically()
    {
        if (moveDown == true)
			gameObject.transform.localPosition -= new Vector3(0.0f, 1.0f, 0.0f) * verticalSpeed * Time.deltaTime;

		else
			gameObject.transform.localPosition += new Vector3(0.0f, 1.0f, 0.0f) * verticalSpeed * Time.deltaTime;

		verticalTimer += Time.deltaTime;

		if (verticalTimer >= verticalTimeInterval)
        {
			moveDown = !moveDown;
			verticalTimer = 0.0f;
		}
	}

}