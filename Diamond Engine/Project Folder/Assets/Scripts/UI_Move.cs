using System;
using DiamondEngine;

public class UI_Move : DiamondComponent
{
	public float delay = 0.5f;
	public float duration = 1.0f;

	public float initialPosX = 0.0f;
	public float initialPosY = 0.0f;
	public float finalPosX = 0.0f;
	public float finalPosY = 0.0f;

	public float initialScaleX = 0.0f;
	public float initialScaleY = 0.0f;
	public float finalScaleX = 0.0f;
	public float finalScaleY = 0.0f;

	public bool playOnAwake = true;

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
			trans.SetLocalTransform(new Vector3(initialPosX, initialPosY, 1.0f), trans.lRot, new Vector3(initialScaleX, initialScaleY, 1.0f)); 
		}

		if (playOnAwake == false)
			ended = true;
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
			else if (started == true && timer <= duration)
			{
				UpdateValues();
			}

			else if (started == true && timer > duration)
				ended = true;
		}

		else
			firstFramePassed = true;
	}

	public void Activate()
    {
		if (trans != null)
        {
			ended = false;
			started = false;
			timer = 0.0f;

			trans.SetLocalTransform(new Vector3(initialPosX, initialPosY, 1.0f), trans.lRot, new Vector3(initialScaleX, initialScaleY, 1.0f));
		}
    }


	private void UpdateValues()
	{
		float xPosition = Mathf.Lerp(initialPosX, finalPosX, timer / duration);
		float yPosition = Mathf.Lerp(initialPosY, finalPosY, timer / duration);

		float xScale = Mathf.Lerp(initialScaleX, finalScaleX, timer / duration);
		float yScale = Mathf.Lerp(initialScaleY, finalScaleY, timer / duration);

		trans.SetLocalTransform(new Vector3(xPosition, yPosition, 1.0f), trans.lRot, new Vector3(xScale, yScale, 1.0f));
	}

}