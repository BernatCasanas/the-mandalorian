using System;
using DiamondEngine;

public class SpikeTrap : DiamondComponent
{
	public void Awake()
    {
		Animator.Play(this.gameObject, "SpikeTrap_Hide");
	}

	public void OnTriggerEnter()
	{
		Animator.Play(this.gameObject, "SpikeTrap_Activate");
	}

	public void OnTriggerExit()
    {
		Animator.Play(this.gameObject, "SpikeTrap_Hide");
	}

}