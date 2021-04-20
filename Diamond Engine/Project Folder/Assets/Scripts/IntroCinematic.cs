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
    
    Vector3 toGoPosition = null;
    Vector3 cameraAuxPosition = null;
    Quaternion auxCameraTransform;
    Quaternion toRotateQuaternion = null;
    float currentSpeed = 0;

    GameObject[] pointArray;
    float[] speedArray = new float[] { 4.0f, 1.0f, 1.0f, 1.0f };    // Adapt values
    int arrayCount = -1;

    public void Awake()
    {
        /*auxCameraTransform = cameraObject.transform.localRotation;
        pointArray = new GameObject[] { point1, point2, point3, point4, point5, point6, point7, point8 };
        UpdateValues();
        // Take player's controls away*/
    }

    public void Update()
    {
        /*if (toGoPosition != null)
        {
            Debug.Log("Camera position " + cameraAuxPosition);
            Debug.Log("Go vector position " + toGoPosition);
            Debug.Log("Array count " + arrayCount);
            cameraAuxPosition += (toGoPosition - cameraAuxPosition).normalized * Time.deltaTime * currentSpeed;
            cameraObject.transform.localRotation = Quaternion.Slerp(cameraObject.transform.localRotation, toRotateQuaternion, 0.25f * Time.deltaTime);
            cameraObject.transform.localPosition = cameraAuxPosition;
        }

        Debug.Log("Distance is " + Mathf.Distance(cameraObject.transform.localPosition, toGoPosition).ToString());
        if (Mathf.Distance(cameraAuxPosition, toGoPosition) < 0.5f)
        {
            UpdateValues();
        }*/
    }

    public void UpdateValues()
    {
        arrayCount++;

        if (arrayCount >= 4)
        {
            gameObject.Enable(false);
            cameraObject.transform.localRotation = auxCameraTransform;
            // Re-activate player's control
            return;
        }

        currentSpeed = speedArray[arrayCount];
        cameraAuxPosition = cameraObject.transform.localPosition = pointArray[arrayCount * 2].transform.localPosition;
        toGoPosition = pointArray[(arrayCount * 2) + 1].transform.localPosition;
        toRotateQuaternion = pointArray[(arrayCount * 2) + 1].transform.localRotation;
    }
}