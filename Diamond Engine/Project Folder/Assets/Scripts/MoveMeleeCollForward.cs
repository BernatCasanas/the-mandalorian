using System;
using DiamondEngine;

public class MoveMeleeCollForward : DiamondComponent
{
	public float colliderSpeed = 1.0f;

	public void Update()
	{
		gameObject.transform.localPosition = gameObject.transform.localPosition + gameObject.transform.GetForward().normalized * colliderSpeed * Time.deltaTime;
	}

}