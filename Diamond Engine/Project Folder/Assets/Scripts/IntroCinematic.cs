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
    public GameObject point9 = null;
    public GameObject point10 = null;
    public GameObject point11 = null;
    public GameObject point12 = null;
    public GameObject point13 = null;
    public GameObject point14 = null;
    public GameObject point15 = null;
    public GameObject point16 = null;
    public GameObject greefRig = null;

    public GameObject postCinematicDialogue = null;

    Vector3 toGoPosition = null;
    Vector3 cameraAuxPosition = null;
    Quaternion toRotateQuaternion = null;
    float currentSpeed = 0;
    float currentTimer = 0.0f;
    float currentTimeLimit = 0.0f;
    Quaternion auxCameraRotation = null;

    GameObject[] pointArray = null;
    float[] speedArray = new float[] { 0.5f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f };    // Adapt values; wouldn't it be easier to just calculate the speed based on how long we need that scene to be?
    float[] timerArray = null;    // Why am I using timer approach? Basically, the triggers of each camera switch are animation ends, positions reached and timers. Everything is convertable to time, but the other two can't be converted universally
    int arrayCount = -1;

    public void Awake()
    {
/*        if (Counter.firstRun)
        {
            auxCameraRotation = cameraObject.transform.localRotation;
            CameraManager.SetCameraPerspective(cameraObject);

            if (InitializeTimers())
            {
                pointArray = new GameObject[] { point1, point2, point3, point4, point5, point6, point7, point8 };
                UpdateValues();
                Animator.Play(greefRig, "Greef_Sit"); // This probably isn't needed with fixed animations
                // Take player's controls away
            }
        }
        else
        {
            EndCinematic();
        }*/
    }

    public void Update()
    {
        // We should have a way to skip the cinematic :/
/*        if (toGoPosition != null)
        {
            cameraAuxPosition += (toGoPosition - cameraAuxPosition).normalized * Time.deltaTime * currentSpeed;
            cameraObject.transform.localRotation = Quaternion.Slerp(cameraObject.transform.localRotation, toRotateQuaternion, 0.25f * Time.deltaTime);
            cameraObject.transform.localPosition = cameraAuxPosition;
        }

        currentTimer += Time.deltaTime;
        if (currentTimer > timerArray[arrayCount])
        {
            ManageCamera();
            UpdateValues();
        }*/
    }

    public void UpdateValues()
    {
        arrayCount++;

        if (arrayCount >= 8)
        {
            EndCinematic();
            return;
        }

        currentTimer = 0;
        currentTimeLimit = timerArray[arrayCount];
        currentSpeed = speedArray[arrayCount];
        cameraAuxPosition = cameraObject.transform.localPosition = pointArray[arrayCount * 2].transform.localPosition;
        cameraObject.transform.localRotation = pointArray[arrayCount * 2].transform.localRotation;
        toGoPosition = pointArray[(arrayCount * 2) + 1].transform.localPosition;
        toRotateQuaternion = pointArray[(arrayCount * 2) + 1].transform.localRotation;
    }

    public void ManageCamera()
    {
        switch (arrayCount)
        {
            case 2:
                Animator.Play(greefRig, "Greef_Head");
                break;

            case 3:
                Animator.Play(greefRig, "Greef_Greet");
                // Mando goes back to his usual model
                break;

            case 4:
                Animator.Play(greefRig, "Greef_Sit");
                break;

            default:
                break;
        }
    }

    public void EndCinematic()
    {
        gameObject.Enable(false);

        if (Counter.firstRun)
        {
            cameraObject.transform.localRotation = auxCameraRotation;
            CameraManager.SetCameraOrthographic(cameraObject);
            // Re-activate player's control
            postCinematicDialogue.Enable(true);
            postCinematicDialogue.GetChild("Button").GetComponent<Navigation>().Select();
        }
    }

    public bool InitializeTimers()
    {
        if (greefRig != null)
        {
            float spaceScene = Mathf.Distance(point1.transform.globalPosition, point2.transform.globalPosition) / speedArray[0];
            float revolverZoomOut = Mathf.Distance(point1.transform.globalPosition, point2.transform.globalPosition) / speedArray[1];
            float revolverStatic = 0.36f;
            float greefTurningZoom = Animator.GetAnimationDuration(greefRig, "Greef_Head");
            float greefGreeting = Animator.GetAnimationDuration(greefRig, "Greef_Head");
            float tableZoomOut = Mathf.Distance(point11.transform.globalPosition, point12.transform.globalPosition) / speedArray[5];
            float tableStatic = 0.50f;
            float mandoZoomOut = Mathf.Distance(point15.transform.globalPosition, point16.transform.globalPosition) / speedArray[7];

            timerArray = new float[] { spaceScene, revolverZoomOut, revolverStatic, greefTurningZoom, greefGreeting, tableZoomOut, tableStatic, mandoZoomOut };
        }
        else
        {
            EndCinematic();
            return false;
        }
        return true;
    }
}