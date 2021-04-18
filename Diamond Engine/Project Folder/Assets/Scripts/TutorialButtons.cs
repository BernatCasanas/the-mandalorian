using System;
using DiamondEngine;

public class TutorialButtons : DiamondComponent
{
	public GameObject dashPS = null;
	public GameObject shootPS = null;
	public GameObject groguPS = null;
	
	public GameObject dashXbox = null;
	public GameObject shootXbox = null;
	public GameObject groguXbox = null;

	public void Awake()
	{
        if (Counter.firstRun == true)
        {
			Counter.firstRun = false;

			RewardType type = RewardType.REWARD_BESKAR;	// We call a function, we get enum / boolean results

            if (type == RewardType.REWARD_BESKAR)	// Placeholder for PS controller
            {
				dashPS.Enable(true);
				shootPS.Enable(true);
				groguPS.Enable(true);
            }

			else if (type == RewardType.REWARD_BESKAR)	// Placeholder for Xbox controller
            {
				dashXbox.Enable(true);
				shootXbox.Enable(true);
				groguXbox.Enable(true);
			}

		}
	}

}