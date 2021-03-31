using System;
using DiamondEngine;

public class FinalSwitch : DiamondComponent
{
	//public void OnTriggerEnter(GameObject triggeredGameObject)
	//{
	//	SceneManager.LoadScene(821370213);

	//}
	public void Update()
	{
		if (Input.GetKey(DEKeyCode.I) == KeyState.KEY_DOWN)
		{
			SceneManager.LoadScene(821370213);
		}
	}
}