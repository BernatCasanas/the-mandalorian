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
	public float time_easing = 0.2f;
	public float timer;
    private float pointA_zoom;

	public void Update()
	{
        Vector3 offset = new Vector3(x, y, z);
        Vector3 desiredPosition = target.localPosition + offset;
        Vector3 smoothPosition = Vector3.Lerp(reference.localPosition, desiredPosition, smoothSpeed);
        reference.localPosition = smoothPosition;
        //if (Input.GetKey(DEKeyCode.N) == KeyState.KEY_DOWN)
        //{
        //    zooming = true;
        //    pointA_zoom = CameraManager.GetOrthSize(reference);
        //}
        //if (zooming)
        //{
            timer = timer + 1;
            float t = 0;

            if (time_easing !=0.0f)
                t = timer / time_easing;

            Debug.Log(time_easing.ToString());
            if (timer >= time_easing)
            {
                timer = 0;
                zooming = false;
            }
            CameraManager.SetOrthSize(reference, PointLerp(pointA_zoom, zoomDesired, Ease(t)));
        //}
    }

	public float Ease(float t)
	{
		return (float)(1 - Math.Pow(1 - t, 3));
    }
	public float PointLerp(float p1, float p2, float t)
    {
		return p1 + (p2 - p1) * t;
    }
}