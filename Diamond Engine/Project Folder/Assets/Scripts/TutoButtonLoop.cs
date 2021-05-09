using System;
using DiamondEngine;

public class TutoButtonLoop : DiamondComponent
{
	public int releasedButton = 0;
	public int pressedButton = 0;
	public int radiantButton = 0;
	private int buttonToChange = 0;

	Material buttonMaterial = null;
	private float maxTime = 0.5f;
	private float currentTime = 0.0f;
	int loopState = 0;

	public void Update()
	{
		//buttonMaterial=this.gameObject.GetComponent<Material>();

		currentTime += Time.deltaTime;
		if (currentTime >= maxTime)
		{
			switch (loopState)
			{
				case 0:
					buttonToChange = pressedButton; loopState = 1;
					break;
				case 1:
					buttonToChange = radiantButton; loopState = 2;
					break;
				case 2:
					buttonToChange = releasedButton; loopState = 0;
					break;
			}
			this.gameObject.AssignLibraryTextureToMaterial(buttonToChange, "diffuseTexture");
			currentTime = 0.0f;
		}
	}

}