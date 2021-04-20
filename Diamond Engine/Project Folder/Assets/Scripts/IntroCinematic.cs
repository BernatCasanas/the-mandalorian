using System;
using DiamondEngine;

public class IntroCinematic : DiamondComponent
{
    public GameObject cameraObject = null;
    public GameObject point1 = null;
    public GameObject point2 = null;
    public GameObject point3 = null;
    public GameObject point4 = null;
    public GameObject point5 = null;
    public GameObject point6 = null;
    public GameObject point7 = null;
    public GameObject point8 = null;
    
    Vector3 toGoVector = null;
    Quaternion toRotateQuaternion = null;
    float currentSpeed = 0;

    GameObject[] pointArray;
    float[] speedArray;
    int arrayCount = -1;

    public void Awake()
    {
        cameraObject.transform.localPosition = point3.transform.localPosition;  // This should be point1

        toGoVector = point1.transform.localPosition;
        toRotateQuaternion = point1.transform.localRotation;

        pointArray = new GameObject[] { point1, point2, point3, point4, point5, point6, point7, point8 };
        speedArray = new float[] { 1.0f, 1.0f, 1.0f, 1.0f}; // Clean values
    }

    public void Update()
    {
        if (Mathf.Distance(cameraObject.transform.localPosition, toGoVector) < 0.5f)
        {
            arrayCount++;
            if (arrayCount >= 4)
            {
                gameObject.Enable(false);
            }

            currentSpeed = speedArray[arrayCount];
            toGoVector = pointArray[(arrayCount + 1) * 2].transform.localPosition;
            toRotateQuaternion = pointArray[(arrayCount + 1) * 2].transform.globalRotation;
        }

        if (toGoVector != null)
        {
            cameraObject.transform.localPosition += (toGoVector - cameraObject.transform.localPosition).normalized * Time.deltaTime * currentSpeed;
            cameraObject.transform.localRotation = Quaternion.Slerp(cameraObject.transform.localRotation, toRotateQuaternion, 0.25f * Time.deltaTime);
        }
    }

}