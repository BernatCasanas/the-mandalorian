using System;
using DiamondEngine;

public class HeavyTrooperParticles : DiamondComponent
{
    public GameObject sweepParticlesObj = null;
    private ParticleSystem sweepParticles = null;

    public enum HEAVYROOPER_PARTICLES : int
    {
        SPEAR,
    }

    public void Awake()
    {
        if (sweepParticlesObj != null)
        {
            sweepParticles = sweepParticlesObj.GetComponent<ParticleSystem>();
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
        }
    }
}
