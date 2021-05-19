using System;
using DiamondEngine;

public class UI_Move : DiamondComponent
{
	public float delay = 0.5f;
	public float duration = 1.0f;

	public Vector2 initialPos;
	public Vector2 finalPos;

	public Vector2 initialScale;
	public Vector2 finalScale;

	private float timer = 0.0f;

	private Transform2D trans = null;
	private bool firstFramePassed = false;  //first frame when charging a scene has a large dt, so we ignore it
	private bool started = false;
	private bool ended = false;

	public void Awake()
	{
		trans = gameObject.GetComponent<Transform2D>();

		if (trans == null)
		{
			Debug.Log("Need to add transform2D to gameObject: " + gameObject.Name);
			ended = true;
		}
		else
		{
			trans.SetLocalTransform(new Vector3(initialPos.x, initialPos.y, 1.0f), trans.lRot, new Vector3(initialScale.x, initialScale.y, 1.0f)); 
		}
	}

	public void Update()
	{
		if (firstFramePassed == true && ended == false)
		{

			timer += Time.deltaTime;

			if (started == false && timer >= delay)
			{
				started = true;
				timer = 0.0f;
			}
			else if (started == true && timer < duration)
			{
				float xValue = Mathf.Lerp(0.0f, 1.0f, timer / duration); //Working on the lerp
				//material.SetFloatUniform("fadeValue", value);
			}

			else if (started == true && timer >= duration)
				ended = true;
		}

		else
			firstFramePassed = true;
	}

}