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
			ControllerType type = DiamondEngine.Input.GetControllerType();  // We call a function, we get enum / boolean results

            switch (type)
            {
				case ControllerType.SDL_CONTROLLER_TYPE_PS3:
				case ControllerType.SDL_CONTROLLER_TYPE_PS4:
					dashPS.Enable(true);
					shootPS.Enable(true);
					groguPS.Enable(true);
					break;
				case ControllerType.SDL_CONTROLLER_TYPE_XBOX360:
				case ControllerType.SDL_CONTROLLER_TYPE_XBOXONE:
					dashXbox.Enable(true);
					shootXbox.Enable(true);
					groguXbox.Enable(true);
					break;
            }
		}
	}

}