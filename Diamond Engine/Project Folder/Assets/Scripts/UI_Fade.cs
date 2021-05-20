using System;
using DiamondEngine;

public class UI_Fade : DiamondComponent
{
	public float delay = 0.5f;
	public float duration = 1.0f;

	private float timer = 0.0f;

	private Material material = null;
	private bool firstFramePassed = false;  //first frame when charging a scene has a large dt, so we ignore it
	private bool started = false;
	private bool ended = false;

	public void Awake()
    {
		material = gameObject.GetComponent<Material>();

		if (material == null)
		{
			Debug.Log("Need to add material to gameObject: " + gameObject.Name);
			ended = true;
		}
        else
        {
			material.SetFloatUniform("fadeValue", 0.0f);
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
				float value = Mathf.Lerp(0.0f, 1.0f, timer / duration);
				material.SetFloatUniform("fadeValue", value);
			}

			else if (started == true && timer >= duration)
				ended = true;
		}

		else
			firstFramePassed = true;
	}
}