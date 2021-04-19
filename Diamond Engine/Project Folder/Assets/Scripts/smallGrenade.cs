using System;
using DiamondEngine;

public class smallGrenade : DiamondComponent
{
    public GameObject thisReference = null; //This is needed until i make all this be part of a component base class

    public float detonationTime = 2.0f;
    public float damage = 5.0f;
    private float detonateTimer = 0.0f;

    private bool exploding = false;
    public float explosionTime = 0.5f;
    private float explosionTimer = 0.0f;

    public void OnCollisionEnter(GameObject collidedGameObject)
    {
        if (collidedGameObject.CompareTag("Enemy"))
        {
            Explode();
        }
    }

    public void Update()
    {
        if (detonateTimer > detonationTime && exploding == false)
        {
            Explode();
        }

        if (explosionTimer > explosionTime && exploding == true)
        {
            Audio.StopAudio(gameObject);
            InternalCalls.Destroy(thisReference);
        }

        UpdateTimers();
    }


    private void Explode()
    {
        Audio.StopAudio(gameObject);
        Audio.PlayAudio(gameObject, "Play_Mando_Grenade_Explosion_2");
        exploding = true;
    }
    private void UpdateTimers()
    {
        detonateTimer += Time.deltaTime;

        if (exploding == true)
        {
            explosionTimer += Time.deltaTime;
        }
    }

    public void InitMiniGrenades( float explosionTimeOffset, int damage = 0)
    {
        explosionTime += explosionTimeOffset;

        if (damage != 0)
        {
            this.damage = damage;   
        }

    }

}