using System;
using DiamondEngine;

public class Enable : DiamondComponent
{
	public GameObject gameObject1;
	public GameObject gameObject2;
	
	public void OnExecuteButton()
    {
		if (gameObject1 != null) gameObject1.Enable(!gameObject1.IsEnabled());
		if (gameObject2 != null) gameObject2.Enable(!gameObject2.IsEnabled());
    }
}