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
        // Put CameraController component is Active(false)? Or leave it like that in the inspector, then activate it from here when cinematic is done

        toGoVector = point1.transform.localPosition;
        toRotateQuaternion = point1.transform.localRotation;

        pointArray = new GameObject[] { point1, point2, point3, point4, point5, point6, point7, point8 };
        speedArray = new float[] { 1.0f, 1.0f, 1.0f, 1.0f}; // Clean values
    }

    public void Update()
    {
        if (Mathf.Distance(gameObject.transform.localPosition, toGoVector) < 0.5f)
        {
            arrayCount++;
            if (arrayCount >= 4)
            {
                // I could probably recieve the camera disabled from the editor, then disable this gameObject, enable real camera, pim pam pum
            }

            currentSpeed = speedArray[arrayCount];
            toGoVector = pointArray[(arrayCount + 1) * 2].transform.localPosition;
            toRotateQuaternion = pointArray[(arrayCount + 1) * 2].transform.globalRotation;
        }

        if (toGoVector != null)
        {
            gameObject.transform.localPosition += (toGoVector - gameObject.transform.localPosition).normalized * Time.deltaTime * currentSpeed;
            gameObject.transform.localRotation = Quaternion.Slerp(gameObject.transform.localRotation, toRotateQuaternion, 0.25f * Time.deltaTime);
        }
    }

}