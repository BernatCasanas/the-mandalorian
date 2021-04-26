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
    public GameObject greefRig = null;

    public GameObject postCinematicDialogue = null;

    Vector3 toGoPosition = null;
    Vector3 cameraAuxPosition = null;
    Quaternion toRotateQuaternion = null;
    float currentSpeed = 0;
    Quaternion auxCameraRotation = null;

    GameObject[] pointArray = null;
    float[] speedArray = new float[] { 0.5f, 2.0f, 1.0f, 1.0f };    // Adapt values
    int arrayCount = -1;
    bool provisionalBool = false;
   // float greefAnimation = 0.0f; // This should not be a thing when iterating
   // bool playGreefAnimation = false; // This should not be a thing when iterating
   // float yAxisAnimationOffset = 0.5f; // This should not be a thing when iterating

    public void Awake()
    {
        if (Counter.firstRun)
        {
            auxCameraRotation = cameraObject.transform.localRotation;
            pointArray = new GameObject[] { point1, point2, point3, point4, point5, point6, point7, point8 };
            UpdateValues();
            CameraManager.SetCameraPerspective(cameraObject);
            //greefAnimation = Animator.GetAnimationDuration(greefRig, "Greef_Greet");
            //Animator.Play(greefRig, "Greef_Sit");
            // Take player's controls away
        }
        else
        {
            EndCinematic();
        }
    }

    public void Update()
    {
        if (provisionalBool == false)   // We are skipping the helmet part of the cinematic. When we have it, delete this
        {
            provisionalBool = true;
            UpdateValues();
        }

        // We should have a way to skip the cinematic :/
        if (toGoPosition != null)
        {
            cameraAuxPosition += (toGoPosition - cameraAuxPosition).normalized * Time.deltaTime * currentSpeed;
            cameraObject.transform.localRotation = Quaternion.Slerp(cameraObject.transform.localRotation, toRotateQuaternion, 0.25f * Time.deltaTime);
            cameraObject.transform.localPosition = cameraAuxPosition;
        }

        if (Mathf.Distance(cameraAuxPosition, toGoPosition) < 0.5f)
        {
            UpdateValues();
        }

 /*       if (arrayCount == 2)
        {
            greefAnimation -= Time.deltaTime;

            if (greefAnimation <= 0 && playGreefAnimation == false)
            {
                playGreefAnimation = true;
                greefRig.transform.localPosition = new Vector3(greefRig.transform.localPosition.x, greefRig.transform.localPosition.y + yAxisAnimationOffset, greefRig.transform.localPosition.z);
                Animator.Play(greefRig, "Greef_Sit");
            }
        }*/
    }

    public void UpdateValues()
    {
        arrayCount++;

        if (arrayCount >= 4)
        {
            EndCinematic();
            return;
        }

        currentSpeed = speedArray[arrayCount];
        cameraAuxPosition = cameraObject.transform.localPosition = pointArray[arrayCount * 2].transform.localPosition;
        cameraObject.transform.localRotation = pointArray[arrayCount * 2].transform.localRotation;
        toGoPosition = pointArray[(arrayCount * 2) + 1].transform.localPosition;
        toRotateQuaternion = pointArray[(arrayCount * 2) + 1].transform.localRotation;

/*        if (arrayCount == 2)
        {
            greefRig.transform.localPosition = new Vector3(greefRig.transform.localPosition.x, greefRig.transform.localPosition.y - yAxisAnimationOffset, greefRig.transform.localPosition.z);
            Animator.Play(greefRig, "Greef_Greet");
        }*/
    }

    public void EndCinematic()
    {
        gameObject.Enable(false);
        // Re-activate player's control
        if (Counter.firstRun)
        {
            cameraObject.transform.localRotation = auxCameraRotation;
            postCinematicDialogue.Enable(true);
            postCinematicDialogue.GetChild("Button").GetComponent<Navigation>().Select();
            CameraManager.SetCameraOrthographic(cameraObject);
        }
    }
}