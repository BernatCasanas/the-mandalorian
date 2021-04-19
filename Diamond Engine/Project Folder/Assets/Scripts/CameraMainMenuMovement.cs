using System;
using DiamondEngine;
using System.Collections.Generic;

public class CameraMainMenuMovement : DiamondComponent
{
	public GameObject point1 = null;
	public GameObject point2 = null;
	public GameObject point3 = null;
	public GameObject point4 = null;
	public GameObject point5 = null;
	public GameObject point6 = null;
	public GameObject point7 = null;

	public float speed = 2;
	public float timer = 2;
	public int pointCount = 0;

	Vector3 toGoVector = null;
	Quaternion toRotateQuaternion = null;
	private GameObject[] pointArray;

	public void Awake()
    {
		toGoVector = point1.transform.localPosition;
		toRotateQuaternion = point1.transform.localRotation;

		pointArray = new GameObject[] { point1, point2, point3, point4, point5, point6, point7 };
	}

	public void Update()
	{
        if (Mathf.Distance(gameObject.transform.localPosition, toGoVector) < 0.5f)
        {
			timer = 0;

			toGoVector = pointArray[pointCount].transform.localPosition;
			toRotateQuaternion = pointArray[pointCount].transform.globalRotation;

			speed = Mathf.Distance(gameObject.transform.localPosition, toGoVector) / 10;
			pointCount++;

			if (pointCount >= 7) 
				pointCount = 0;
		}

		if(toGoVector!=null)
        {
			gameObject.transform.localPosition += (toGoVector - gameObject.transform.localPosition).normalized * Time.deltaTime * speed;
			gameObject.transform.localRotation = Quaternion.Slerp(gameObject.transform.localRotation, toRotateQuaternion, 0.25f * Time.deltaTime);
		}
		timer += Time.deltaTime;
	}
}