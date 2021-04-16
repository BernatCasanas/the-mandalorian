using System;
using DiamondEngine;

public class TurretRotation : DiamondComponent
{
	public GameObject turret;
	public float maxRotationDeg = 90;
	public float speedMult = 1.0f;

	private float currentRotation;
	private bool rotatingLeft = true;

	public void Awake()
	{
		currentRotation = maxRotationDeg * 0.5f;
	}

	public void Update()
	{
		if (rotatingLeft)
		{
			currentRotation -= speedMult;
			if (currentRotation <= maxRotationDeg * -0.5f) rotatingLeft = !rotatingLeft;
		}
		else
		{
			currentRotation += speedMult;
			if (currentRotation >= maxRotationDeg * 0.5f) rotatingLeft = !rotatingLeft;
		}

		Rotate(currentRotation * 0.0174533f); //Radian to degree
	}

	private void Rotate(float newAngle)
	{
		turret.transform.localRotation = Quaternion.RotateAroundAxis(new Vector3(0, 1, 0), newAngle);
	}
}