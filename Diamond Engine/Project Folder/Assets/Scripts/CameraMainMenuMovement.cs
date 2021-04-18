using System;
using DiamondEngine;

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

	public void Awake()
    {
		toGoVector = point1.transform.localPosition;
		toRotateQuaternion = point1.transform.localRotation;
	}

	public void Update()
	{
        if (Mathf.Distance(gameObject.transform.localPosition, toGoVector) < 0.5f)
        {
			timer = 0;
            switch(pointCount)
            {
				case (0):
					Debug.Log("Cam Point 2");
					toGoVector = point2.transform.localPosition;
					toRotateQuaternion = point2.transform.globalRotation;
					break;
				case (1):
					Debug.Log("Cam Point 3");
					toGoVector = point3.transform.localPosition;
					toRotateQuaternion = point3.transform.globalRotation;
					break;
				case (2):
					Debug.Log("Cam Point 4");
					toGoVector = point4.transform.localPosition;
					toRotateQuaternion = point4.transform.globalRotation;
					break;
				case (3):
					Debug.Log("Cam Point 5");
					toGoVector = point5.transform.localPosition;
					toRotateQuaternion = point5.transform.globalRotation;
					break;
				case (4):
					Debug.Log("Cam Point 6");
					toGoVector = point6.transform.localPosition;
					toRotateQuaternion = point6.transform.globalRotation;
					break;
				case (5):
					Debug.Log("Cam Point 7");
					toGoVector = point7.transform.localPosition;
					toRotateQuaternion = point7.transform.globalRotation;
					break;
				case (6):
					Debug.Log("Cam Point 1");
					toGoVector = point1.transform.localPosition;
					toRotateQuaternion = point1.transform.globalRotation;
					break;
			}

			speed = Mathf.Distance(gameObject.transform.localPosition, toGoVector) / 10;
			pointCount++;
			if (pointCount >= 7) pointCount = 0;
		}

		if(toGoVector!=null)
        {
			gameObject.transform.localPosition += (toGoVector - gameObject.transform.localPosition).normalized * Time.deltaTime * speed;
			gameObject.transform.localRotation = Quaternion.Slerp(gameObject.transform.localRotation, toRotateQuaternion, 0.25f * Time.deltaTime);
		}
		timer += Time.deltaTime;
	}
}