using System;
using DiamondEngine;

public class BlackFade : DiamondComponent
{
    public enum STATE
    {
        NONE,
        FADE_IN,
        FADE_OUT
    }

    public static Action onFadeInCompleted;
    public static Action onFadeOutCompleted;

    public Material fade;

    private float current_alpha = 0.0f;

    private STATE state;

    public float fadeOutSpeed = 1.0f;
    public float fadeInSpeed = 1.0f;

    private void Awake()
    {
        PlayerHealth.onPlayerDeathEnd += StartFadeIn;
        current_alpha = 1.0f;
        fade = gameObject.GetComponent<Material>();
        fade.SetFloatUniform("fadeValue", current_alpha);
        StartFadeOut();

    }
    public void Update()
    {
        switch (state)
        {
            case STATE.NONE:
                break;
            case STATE.FADE_IN:
                FadeIn();
                break;
            case STATE.FADE_OUT:
                FadeOut();
                break;
        }
    }

    private void FadeIn()
    {
        if (current_alpha < 1)
        {
            current_alpha += Time.deltaTime * fadeInSpeed;
            if (current_alpha > 1)
            {
                current_alpha = 1;
                state = STATE.NONE;
                onFadeInCompleted?.Invoke();
            }
            fade.SetFloatUniform("fadeValue", current_alpha);
        }
    }

    private void FadeOut()
    {
        if (current_alpha > 0)
        {
            current_alpha -= Time.deltaTime * fadeOutSpeed;
            if (current_alpha < 0)
            {
                current_alpha = 0;
                state = STATE.NONE;
                onFadeOutCompleted?.Invoke();
            }
            fade.SetFloatUniform("fadeValue", current_alpha);
        }
    }

    private void StartFadeIn()
    {
        state = STATE.FADE_IN;

    }

    private void StartFadeOut()
    {
        state = STATE.FADE_OUT;
    }

}