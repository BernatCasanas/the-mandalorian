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

	public float horizontalSpeed = 4f;

	public void Update()
	{
		Move();
	}

	private void Move()
    {
		FollowPoint();
		MoveVertically();
		LookAtMando();
	}


	private void FollowPoint()
    {
        if (followPoint != null)
        {
			float x = Mathf.Lerp(gameObject.transform.localPosition.x, followPoint.transform.globalPosition.x, horizontalSpeed * Time.deltaTime);
			float z = Mathf.Lerp(gameObject.transform.localPosition.z, followPoint.transform.globalPosition.z, horizontalSpeed * Time.deltaTime);
			gameObject.transform.localPosition = new Vector3(x, gameObject.transform.localPosition.y, z);
		}
    }


	private void MoveVertically()
    {
		if (moveDown == true)
		{
			gameObject.transform.localPosition -= new Vector3(0.0f, 1.0f, 0.0f) * verticalSpeed * Time.deltaTime;
		}

		else
			gameObject.transform.localPosition += new Vector3(0.0f, 1.0f, 0.0f) * verticalSpeed * Time.deltaTime;

		verticalTimer += Time.deltaTime;

		if (verticalTimer >= verticalTimeInterval)
        {
			moveDown = !moveDown;
			verticalTimer = 0.0f;
		}
	}


	private void LookAtMando()
    {
		Vector3 worldForward = new Vector3(0, 0, 1);

		Vector3 vec = Core.instance.gameObject.transform.localPosition + Core.instance.gameObject.transform.GetForward() * 2 - gameObject.transform.globalPosition;
		vec = vec.normalized;

		float dotProduct = vec.x * worldForward.x + vec.z * worldForward.z;
		float determinant = vec.x * worldForward.z - vec.z * worldForward.x;

		float angle = (float)Math.Atan2(determinant, dotProduct);

		gameObject.transform.localRotation = Quaternion.RotateAroundAxis(new Vector3(0, 1, 0), angle);
	}
}