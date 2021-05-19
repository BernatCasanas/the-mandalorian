using System;
using DiamondEngine;

public class Explosion : DiamondComponent
{
    public GameObject primaryParticlesGO = null;
    public GameObject secondaryParticlesGO = null;

    ParticleSystem primaryParticles = null;
    ParticleSystem secondaryParticles = null;

    public void Awake()
    {
        if (primaryParticlesGO != null)
        {
            primaryParticles = primaryParticlesGO.GetComponent<ParticleSystem>();

            if (primaryParticles != null)
                primaryParticles.Play();
        }

        if (secondaryParticlesGO != null)
        {
            secondaryParticles = secondaryParticlesGO.GetComponent<ParticleSystem>();

            if (secondaryParticles != null)
                secondaryParticles.Play();
        }
    }

    public void Update()
    {
        if (primaryParticles != null && !primaryParticles.playing)
        {
            if (secondaryParticles != null && !secondaryParticles.playing || secondaryParticles == null)
            {
                InternalCalls.Destroy(gameObject);
            }
        }
    }

}