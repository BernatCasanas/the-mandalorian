using System;
using DiamondEngine;

public class TextController : DiamondComponent
{
	private int next = 0;
	public void Update()
	{
		Debug.Log("uwu");

		if (Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_REPEAT)
		{
			next++;
			Debug.Log("Hey");
		}
	}

}