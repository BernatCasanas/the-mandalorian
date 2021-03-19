using System;
using DiamondEngine;

public class FinalSwitch : DiamondComponent
{
	public void OnTriggerEnter(GameObject triggeredGameObject)
	{
		SceneManager.LoadScene(1076838722);

	}
	public void Update()
	{
		if (Input.GetKey(DEKeyCode.I) == KeyState.KEY_DOWN)
		{

			SceneManager.LoadScene(1076838722);
		}


	}
}