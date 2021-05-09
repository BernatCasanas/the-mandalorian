using System;
using DiamondEngine;

public class TutorialButtons : DiamondComponent
{
	public GameObject dashPS = null;
	public GameObject shootPS = null;
	public GameObject wallPS = null;
	public GameObject pushPS = null;
	public GameObject rifflePS = null;
	public GameObject grenadePS = null;
	public GameObject dashTrapPS = null;

	
	public GameObject dashXbox = null;
	public GameObject shootXbox = null;
	public GameObject wallXbox = null;
	public GameObject pushXbox = null;
	public GameObject riffleXbox = null;
	public GameObject grenadeXbox = null;
	public GameObject dashTrapXbox = null;

	public void Awake()
	{
			ControllerType type = DiamondEngine.Input.GetControllerType();  // We call a function, we get enum / boolean results

            switch (type)
            {
				case ControllerType.SDL_CONTROLLER_TYPE_PS3:
				case ControllerType.SDL_CONTROLLER_TYPE_PS4:
					dashPS.Enable(true);
					shootPS.Enable(true);
					wallPS.Enable(true);
					pushPS.Enable(true);
					rifflePS.Enable(true);
					grenadePS.Enable(true);
					dashTrapPS.Enable(true);
					break;
				case ControllerType.SDL_CONTROLLER_TYPE_XBOX360:
				case ControllerType.SDL_CONTROLLER_TYPE_XBOXONE:
					dashXbox.Enable(true);
					shootXbox.Enable(true);
					wallXbox.Enable(true);
					pushXbox.Enable(true);
					riffleXbox.Enable(true);
					grenadeXbox.Enable(true);
					dashTrapXbox.Enable(true);
					break;
            }
	}

}