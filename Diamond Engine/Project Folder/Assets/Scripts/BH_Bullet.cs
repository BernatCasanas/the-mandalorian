﻿using System;
using DiamondEngine;


public class BH_Bullet : DiamondComponent
{
    public float speed = 60.0f;
    public float maxLifeTime = 5.0f;

    private float currentLifeTime = 0.0f;

    public float range_squared = 0.0f;
    public Vector3 initial_pos = new Vector3(0.0f, 0.0f, 0.0f);
    public bool use_range_instead_of_time = false;

    private float actual_range_squared = 0.0f;

    public float yVel = 0.0f;
    public float damage = 9.0f;

    private bool started = false;
    private bool triggered = false;
    private float timer = 0;

    private GameObject destroyOBJ = null;
    private GameObject mesh = null;
    private Entity entity = null;

    public ParticleSystem destroyPar = null;

    public void Update()
    {
        if (started == false)
        {
            mesh = this.gameObject.GetChild("bulletmesh");
            destroyOBJ = this.gameObject.GetChild("STimpact Particle");
            started = true;
            initial_pos = gameObject.transform.globalPosition;
            //InternalCalls.CreatePrefab("Library/Prefabs/1564072956.prefab", gameObject.transform.globalPosition, gameObject.transform.globalRotation, gameObject.transform.globalScale);
        }

        if (!use_range_instead_of_time)
            currentLifeTime += Time.deltaTime;
        else
            actual_range_squared = (gameObject.transform.globalPosition -initial_pos).magnitude;

        // gameObject.transform.localPosition += gameObject.transform.GetForward() * (speed * Time.deltaTime);
        if (!triggered)
        {
            if(entity != null && entity.HasStatus(STATUS_TYPE.PRIM_SPEED))
            {
                gameObject.SetVelocity(gameObject.transform.GetForward() * speed * (1 + entity.GetStatusData(STATUS_TYPE.PRIM_SPEED).severity / 100));
            }
            else
            {
                gameObject.SetVelocity(gameObject.transform.GetForward() * speed);
            }
        }
        else
        {
            if (mesh.IsEnabled() == true)
            {
                mesh.Enable(false);
            }
            timer += Time.deltaTime;
            gameObject.SetVelocity(new Vector3(0, 0, 0));

        }
        //yVel -= Time.deltaTime / 15.0f;
        //gameObject.transform.localPosition += (Vector3.up * yVel);

        if (triggered == true)
        {
            if (timer > 0.5 && gameObject != null)
            {
                InternalCalls.Destroy(gameObject);
            }
        }
        else if (((!use_range_instead_of_time && currentLifeTime >= maxLifeTime) || (use_range_instead_of_time && actual_range_squared >= range_squared)) && gameObject != null)
        {
            InternalCalls.Destroy(gameObject);
        }


    }

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {

        if (triggered == false)
        {
            triggered = true;

            if (destroyOBJ != null)
            {
                destroyPar = destroyOBJ.GetComponent<ParticleSystem>();

                if (destroyPar != null)
                    destroyPar.Play();
            }
        }


    }

    public float GetDamage()
    {
        if (Core.instance == null)
            return damage;
        return damage * Core.instance.GetBlasterDamageMod();
    }
    public void SetEntity(Entity myEntity)
    {
        entity = myEntity;
    }
}