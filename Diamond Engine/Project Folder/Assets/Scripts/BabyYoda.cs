using System;
using DiamondEngine;

public class BabyYoda : DiamondComponent
{
	//Vertical Movement
	public float verticalSpeed = 0.8f;
	public float verticalTimeInterval = 1.2f;
	private float verticalTimer = 0.0f;
	private bool moveDown = true;

	//Horizontal Movement
	public GameObject followPoint = null;

	public float horizontalSpeed = 5.0f;

	public void Update()
	{
		Move();
	}

	private void Move()
    {
		FollowPoint();
		MoveVertically();
	}


	private void FollowPoint()
    {
        if (followPoint != null)
        {
			gameObject.transform.localPosition = new Vector3(followPoint.transform.globalPosition.x, gameObject.transform.localPosition.y, followPoint.transform.globalPosition.z);
		}
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