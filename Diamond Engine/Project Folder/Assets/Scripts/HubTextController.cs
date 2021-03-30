using System;
using DiamondEngine;

public class HubTextController : DiamondComponent
{
	public GameObject BoKatanDialogue = null;
	public GameObject anotherDialog = null;
	private bool showtext = true;
	public void Update()
	{
        if (showtext)//TODO: Be near the NPC && press input A
        {
			BoKatanDialogue.Enable(true);
			showtext = false;
        }
	}
}