using System;
using DiamondEngine;

public class CameraController : DiamondComponent
{
	public GameObject reference = null;
	public GameObject target;

	public float x, y, z;
	public float smoothSpeed = 0.0f;
	public float zoomDesired = 0.1f;

	private bool zooming = false;
	public float timer_easing_sec = 0.2f;
	public float timer;
    private float pointA_zoom;

	public void Update()
	{
        Vector3 offset = new Vector3(x, y, z);
        Vector3 desiredPosition = target.localPosition + offset;
        Vector3 smoothPosition = Vector3.Lerp(reference.localPosition, desiredPosition, smoothSpeed);
        reference.localPosition = smoothPosition;
        if (Input.GetKey(DEKeyCode.N) == KeyState.KEY_DOWN)
        {
            zooming = true;
            pointA_zoom = CameraManager.GetOrthSize(reference);
        }
        if (zooming && timer_easing_sec!=0.0f)
        {
            timer += Time.deltaTime;
            float t = 0;

            t = timer / timer_easing_sec;

            if (timer >= timer_easing_sec)
            {
                timer = 0;
                zooming = false;
            }
            CameraManager.SetOrthSize(reference, Ease.PointLerp(pointA_zoom, zoomDesired, Ease.OutCubic(t)));
        }
    }


}