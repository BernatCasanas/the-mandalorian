using System;
using DiamondEngine;

public class BlackFade : DiamondComponent
{
    public Material fade;

    private float current_alpha = 0.0f;
    private void Awake()
    {
        fade = gameObject.GetComponent<Material>();
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
            if (current_alpha > 1)
                current_alpha = 1;
            fade.SetFloatUniform("fadeValue", current_alpha);
        }
    }

    public void FadeOut()
    {
        if (current_alpha > 0)
        {
            current_alpha -= Time.deltaTime;
            if (current_alpha < 0)
                current_alpha = 0;
            fade.SetFloatUniform("fadeValue", current_alpha);
        }
    }

}