using System;
using DiamondEngine;

public class SpikeTrap : DiamondComponent
{
    bool triggered = false;

    private float timer = 0.0f;
    private float activateAnimationTime = 0.0f;
    private float hideAnimationTime = 0.0f;

    public void Awake()
    {
        Animator.Play(this.gameObject, "SpikeTrap_Hide");
        activateAnimationTime = Animator.GetAnimationDuration(gameObject, "SpikeTrap_Activate");
        hideAnimationTime = Animator.GetAnimationDuration(gameObject, "SpikeTrap_Hide");
    }

    public void Update()
    {
        if (timer > 0.0f)
        {
            timer -= Time.deltaTime;

            if (timer <= 0.0f && triggered)
            {
                Animator.Play(this.gameObject, "SpikeTrap_Hide");
                triggered = false;
            }
        }
    }

    public void OnTriggerEnter()
    {
        if (!triggered)
        {
            Animator.Play(this.gameObject, "SpikeTrap_Activate");
            timer = activateAnimationTime;
            triggered = true;
        }
    }
}