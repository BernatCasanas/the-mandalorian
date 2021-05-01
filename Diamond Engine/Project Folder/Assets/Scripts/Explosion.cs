using System;
using DiamondEngine;

public class Explosion : DiamondComponent
{
    public GameObject primaryParticlesGO = null;
    public GameObject secondaryParticlesGO = null;

    ParticleSystem primaryParticles = null;
    ParticleSystem secondaryParticles = null;

    public float duration = 0.0f;
    private float timer = 0.0f;

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

        timer = duration;
    }

    public void Update()
    {
        if(timer > 0.0f)
        {
            timer -= Time.deltaTime;

            if(timer <= 0.0f)
            {
                InternalCalls.Destroy(gameObject);
            }
        }

    }

}