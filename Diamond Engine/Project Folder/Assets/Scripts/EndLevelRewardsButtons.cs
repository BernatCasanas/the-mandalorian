using System;
using DiamondEngine;

public class EndLevelRewardsButtons : DiamondComponent  // Probably, the saddest script ever. But. We need it. Shit happens
{
    public bool pressed = false;
	public void OnExecuteButton()
    {
        pressed = true;
    }

}