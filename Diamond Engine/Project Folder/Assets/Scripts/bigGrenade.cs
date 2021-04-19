using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using DiamondEngine;

public class bigGrenade : DiamondComponent
{
    public GameObject thisReference = null; //This is needed until i make all this be part of a component base class

    private float timeToGetToTarget = 1f;
    private float setUpTimer = 0f;
    private float speed = 25.0f;

    private float damage = 0.0f;

    public float detonationTime = 1.5f;
    private float dftExplosionTimer = 0.0f;

    private bool grenadeSetUp = false;
    private bool enemiesNearby = false;

    public float detonationTimeOnDetect = 0.16f;
    private float proximityTimer = 0f;

    private Vector3 targetPos = new Vector3(0, 0, 0);
    private Vector3 targetDirection = new Vector3(0, 0, 0);


    public void Update()
    {
        Move();

        if (dftExplosionTimer > detonationTime || proximityTimer > detonationTimeOnDetect)
        {
            InternalCalls.Destroy(thisReference);
            Audio.StopAudio(gameObject);
            Audio.PlayAudio(gameObject, "Play_Mando_Grenade_Explosion_1_Charge");

            SpawnMiniGrenades();

        }

        UpdateTimers();

    }

    private void Move()
    {
        if (grenadeSetUp == true)
            return;

        gameObject.transform.localPosition += targetDirection * (speed * Time.deltaTime);

        if (setUpTimer >= timeToGetToTarget)
        {
            grenadeSetUp = true;
        }

    }

    private void UpdateTimers()
    {
        if (grenadeSetUp == true)
        {
            dftExplosionTimer += Time.deltaTime;
        }
        else
        {
            setUpTimer += Time.deltaTime;
        }

        if (enemiesNearby == true)
        {
            proximityTimer += Time.deltaTime;
        }

    }

    private void SpawnMiniGrenades()
    {
        Vector3 scale = new Vector3(0.125f, 0.125f, 0.125f);

        Vector3 position = new Vector3(0.0f, 0.0f, 0.0f); ;
        position.y = 0.25f;

        float scatteringMult = 1.9f;

        position.x = 0.45f;
        position.z = 0f;
        InstanciateMiniGrenade(position * scatteringMult, scale);

        position.x = 0f;
        position.z = -0.45f;
        InstanciateMiniGrenade(position * scatteringMult, scale);

        position.x = -0.45f;
        position.z = 0f;
        InstanciateMiniGrenade(position * scatteringMult, scale);

        position.x = 0f;
        position.z = 0.45f;
        InstanciateMiniGrenade(position * scatteringMult, scale);

        position.x = 0f;
        position.z = 0.9f;
        InstanciateMiniGrenade(position * scatteringMult, scale);

        position.x = 0.675f;
        position.z = 0.675f;
        InstanciateMiniGrenade(position * scatteringMult, scale);

        position.x = 0.9f;
        position.z = 0f;
        InstanciateMiniGrenade(position * scatteringMult, scale);

        position.x = 0.675f;
        position.z = -0.675f;
        InstanciateMiniGrenade(position * scatteringMult, scale);

        position.x = 0f;
        position.z = -0.9f;
        InstanciateMiniGrenade(position * scatteringMult, scale);

        position.x = -0.675f;
        position.z = -0.675f;
        InstanciateMiniGrenade(position * scatteringMult, scale);

        position.x = -0.9f;
        position.z = 0f;
        InstanciateMiniGrenade(position * scatteringMult, scale);

        position.x = -0.675f;
        position.z = 0.675f;
        InstanciateMiniGrenade(position * scatteringMult, scale);
    }

    private void InstanciateMiniGrenade(Vector3 positionOffset, Vector3 scale)
    {
        smallGrenade smalLGre = InternalCalls.CreatePrefab("Library/Prefabs/1968664915.prefab", thisReference.transform.globalPosition + positionOffset, thisReference.transform.globalRotation, scale).GetComponent<smallGrenade>();

        if (smalLGre != null)
        {
            var rand = new Random();
            smalLGre.InitMiniGrenades((float)rand.NextDouble());
        }

    }

    public void OnCollisionEnter(GameObject collidedGameObject)
    {
        if (collidedGameObject.CompareTag("Enemy") && grenadeSetUp == true)
        {
            enemiesNearby = true;
        }

    }


    public float GetDamage()
    {
        return damage;
    }

    public void InitGrenade(Vector3 position, float timeToGetToPos, float damage = 0f)
    {
        targetPos = position;
        timeToGetToTarget = timeToGetToPos;

        targetDirection = targetPos - gameObject.transform.globalPosition;
        speed = gameObject.transform.globalPosition.Distance(targetPos) / timeToGetToPos;

        if (damage != 0f)
            this.damage = damage;

        Audio.PlayAudio(gameObject, "Play_Mando_Grenade_1_Charge");

    }


}