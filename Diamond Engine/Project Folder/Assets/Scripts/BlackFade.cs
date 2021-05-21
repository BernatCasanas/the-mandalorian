using System;
using DiamondEngine;

public class BlackFade : DiamondComponent
{
	public Material fade;

	private float alpha = 0.0f;
	private float current_alpha = 0.0f;
	private void Awake()
    {
		current_alpha = 1.0f;
		fade.SetFloatUniform("fadeValue", current_alpha);
		
    }
	public void Update()
	{
		FadeOut();
	}

	public void FadeIn()
    {
		if (current_alpha < 1)
		{
			current_alpha += Time.deltaTime;
			fade.SetFloatUniform("fadeValue", current_alpha);
		}
	}

	public void FadeOut()
    {
		if(current_alpha > 0)
        {
			current_alpha -= Time.deltaTime;
			fade.SetFloatUniform("fadeValue", current_alpha);
        }
    }

}