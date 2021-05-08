using System;
using DiamondEngine;

public class HeavyTrooperParticles : DiamondComponent
{
    public GameObject sweepParticlesObj = null;
    public GameObject dashParticlesObj = null;
    private ParticleSystem sweepParticles = null;
    private ParticleSystem dashParticles = null;

    public enum HEAVYROOPER_PARTICLES : int
    {
        SPEAR,
        DASH
    }

    public void Awake()
    {
        if (sweepParticlesObj != null)
        {
            sweepParticles = sweepParticlesObj.GetComponent<ParticleSystem>();
        }
        if (dashParticlesObj != null)
        {
            dashParticles = dashParticlesObj.GetComponent<ParticleSystem>();
        }
    }

    public void Play(HEAVYROOPER_PARTICLES particleType)
    {
        switch (particleType)
        {
            case HEAVYROOPER_PARTICLES.SPEAR:
                if (sweepParticles != null)
                    sweepParticles.Play();
                else
                    Debug.Log("Sweep particles not found!");
                break;
            case HEAVYROOPER_PARTICLES.DASH:
                if (dashParticles != null)
                    dashParticles.Play();
                else
                    Debug.Log("Dash particles not found!");
                break;
        }
    }

    public void Stop(HEAVYROOPER_PARTICLES particleType)
    {
        switch (particleType)
        {
            case HEAVYROOPER_PARTICLES.SPEAR:
                if (sweepParticles != null)
                    sweepParticles.Stop();
                else
                    Debug.Log("Sweep particles not found!");
                break;
            case HEAVYROOPER_PARTICLES.DASH:
                if (dashParticles != null)
                    dashParticles.Stop();
                else
                    Debug.Log("Dash particles not found!");
                break;
        }
    }
}
