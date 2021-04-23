using System;
using DiamondEngine;

public class CameraController : DiamondComponent
{
    public GameObject target = null;
    //private static bool targetLoaded = false; //TODO: Delete this variable when the bug of not being able to assign the player is solved

    public float x = 0.0f;
    public float y = 0.0f;
    public float z = 0.0f;
    public float smoothSpeed = 0.0f;
    public float zoomDesired = 0.1f;

    //private bool zooming = false;
    public float timer_easing_sec = 0.2f;
    public float timer = 0.0f;
    //private float pointA_zoom;

    public float cornerTopLeftX = -100.0f;
    public float cornerTopLeftY = -100.0f;
    public float cornerBotRightX = 100.0f;
    public float cornerBotRightY = 100.0f;

    public void Update()
    {
        if (/*!targetLoaded && */ Core.instance.gameObject != null)
        {
            target = Core.instance.gameObject;
            //targetLoaded = true;
        }

        Vector3 offset = new Vector3(x, y, z);
        Vector3 desiredPosition = new Vector3(0.0f, 0.0f, 0.0f);

        if (target != null)
            desiredPosition = target.transform.localPosition + offset;

        Vector3 smoothPosition = Vector3.Lerp(gameObject.transform.localPosition, desiredPosition, 1.0f - (float)Math.Pow(smoothSpeed, Time.deltaTime));
        Vector3 finalPos = new Vector3(Mathf.Clamp(smoothPosition.x, cornerTopLeftX, cornerBotRightX), smoothPosition.y, Mathf.Clamp(smoothPosition.z, cornerTopLeftY, cornerBotRightY));
        gameObject.transform.localPosition = finalPos;

        //ZOOM ALGORITHM || WORKING 
        //if (Input.GetKey(DEKeyCode.N) == KeyState.KEY_DOWN)
        //{
        //    zooming = true;
        //    pointA_zoom = CameraManager.GetOrthSize(reference);
        //}
        //if (zooming && timer_easing_sec!=0.0f)
        //{
        //    timer += Time.deltaTime;
        //    float t = 0;

        //    t = timer / timer_easing_sec;

        //    if (timer >= timer_easing_sec)
        //    {
        //        timer = 0;
        //        zooming = false;
        //    }
        //    CameraManager.SetOrthSize(reference, Ease.PointLerp(pointA_zoom, zoomDesired, Ease.OutCubic(t)));
        //}
    }


}
