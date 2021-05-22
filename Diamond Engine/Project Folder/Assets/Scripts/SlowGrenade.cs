using System;
using DiamondEngine;
using System.Collections.Generic;

public class SlowGrenade : DiamondComponent
{
    public float forwardForce = 1000;
    public float upForce = 1000;

    bool start = true;
    bool detonate = false;
    public GameObject grenadeActiveObj = null;
    private ParticleSystem grenadeActivePar = null;
    public GameObject granadeAreaObj = null;
    private ParticleSystem granadeArea = null;

    public float lifeTime = 4.0f;
    private float lifeTimer = 0.0f;

    public float explosionTime = 4.0f;
    private float explosionTimer = 0.0f;

    public float areaRadius = 1.85f;
    public float damage = 5.0f;
    public float procTime = 0.25f;
    private float procTimer = 0f;
    private bool procActivation = false;
    List<GameObject> enemies = new List<GameObject>();

    public void Update()
    {
        if (start == true)
        {
            start = false;
            float modifier = 1;
            if (Core.instance != null)
                if (Core.instance.HasStatus(STATUS_TYPE.SEC_RANGE))
                    modifier = 1 + Core.instance.GetStatusData(STATUS_TYPE.SEC_RANGE).severity / 100;
            Vector3 myForce = gameObject.transform.GetForward() * forwardForce * modifier;

            myForce.y = upForce;

            BoxCollider myBoxColl = this.gameObject.GetComponent<BoxCollider>();

            if (myBoxColl != null)
                myBoxColl.active = false;


            gameObject.AddForce(myForce);
        }

        if (!detonate)
        {
            lifeTimer += Time.deltaTime;
        }
        else
        {
            float grenadeDamage = damage;
            if (Core.instance != null)
            {
                grenadeDamage *= Core.instance.GetGrenadeDamageMod();
                if (Core.instance.HasStatus(STATUS_TYPE.BOSSK_AMMO) && enemies.Count > 1)
                    grenadeDamage *= 2;
            }



            explosionTimer += Time.deltaTime;
            float mult = 1;
            if (Core.instance.HasStatus(STATUS_TYPE.BOBBA_STUN_AMMO))
                mult += Core.instance.GetStatusData(STATUS_TYPE.BOBBA_STUN_AMMO).severity / 100;
            procTimer += Time.deltaTime * mult;

            if (procTimer > procTime)
            {
                procActivation = true;
                Audio.StopAudio(gameObject);
                Audio.PlayAudio(gameObject, "Play_Mando_Grenade_Pulse_Wave");
                procTimer = 0f;
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                float slow = 0.33f;
                if (Core.instance != null)
                    if (Core.instance.HasStatus(STATUS_TYPE.SEC_SLOW))
                        slow *= 1 + Core.instance.GetStatusData(STATUS_TYPE.SEC_SLOW).severity;

                Enemy script = enemies[i].GetComponent<StormTrooper>();
                if (script == null)
                {
                    script = enemies[i].GetComponent<DummyStormtrooper>();
                }
                if (script == null)
                {
                    script = enemies[i].GetComponent<Bantha>();
                }
                if (script == null)
                {
                    script = enemies[i].GetComponent<Skytrooper>();
                }
                if (script == null)
                {
                    script = enemies[i].GetComponent<LaserTurret>();
                }

                if (script != null)
                {
                    if (script.healthPoints <= 0)
                    {
                        enemies.Remove(enemies[i]);
                        i--;
                    }
                    else if (procActivation == true)
                    {
                        Debug.Log("Enemy ticked!");
                        Core.instance.hud.GetComponent<HUD>().AddToCombo(5, 1.35f);
                        script.TakeDamage(grenadeDamage * script.damageRecieveMult);
                        script.AddStatus(STATUS_TYPE.SLOWED, STATUS_APPLY_TYPE.BIGGER_TIME, slow, 0.175f);
                    }

                }
                else
                {
                    Rancor bossScript = enemies[i].GetComponent<Rancor>();

                    if (bossScript != null)
                    {
                        if (bossScript.healthPoints <= 0)
                        {
                            enemies.Remove(enemies[i]);
                            i--;
                        }
                        else if (procActivation == true)
                        {
                            Core.instance.hud.GetComponent<HUD>().AddToCombo(5, 1.45f);
                            bossScript.TakeDamage(grenadeDamage * bossScript.damageRecieveMult);
                            bossScript.AddStatus(STATUS_TYPE.SLOWED, STATUS_APPLY_TYPE.BIGGER_TIME, slow, 0.175f);
                        }
                    }
                    else if (bossScript == null)
                    {
                        Skel skelScript = enemies[i].GetComponent<Skel>();
                        Wampa wampaScript = enemies[i].GetComponent<Wampa>();

                        if (skelScript != null)
                        {
                            if (skelScript.healthPoints <= 0)
                            {
                                enemies.Remove(enemies[i]);
                                i--;
                            }
                            else if (procActivation == true)
                            {
                                Core.instance.hud.GetComponent<HUD>().AddToCombo(5, 1.45f);
                                skelScript.TakeDamage(grenadeDamage * skelScript.damageRecieveMult);
                                skelScript.AddStatus(STATUS_TYPE.SLOWED, STATUS_APPLY_TYPE.BIGGER_TIME, slow, 0.175f);
                            }
                        }
                        else if (wampaScript != null)
                        {
                            if (wampaScript.healthPoints <= 0)
                            {
                                enemies.Remove(enemies[i]);
                                i--;
                            }
                            else if (procActivation == true)
                            {
                                Core.instance.hud.GetComponent<HUD>().AddToCombo(5, 1.45f);
                                wampaScript.TakeDamage(grenadeDamage * wampaScript.damageRecieveMult);
                                wampaScript.AddStatus(STATUS_TYPE.SLOWED, STATUS_APPLY_TYPE.BIGGER_TIME, slow, 0.175f);
                            }
                        }

                    }

                }


            }

            procActivation = false;
        }

        if (lifeTimer >= lifeTime & !detonate)
        {
            Detonate();

        }
        if (Core.instance != null && Core.instance.HasStatus(STATUS_TYPE.SEC_DURATION))
        {
            if (explosionTimer >= explosionTime * (1 + Core.instance.GetStatusData(STATUS_TYPE.SEC_DURATION).severity / 100))
                Explode();
        }
        else
        {
            if (explosionTimer >= explosionTime)
                Explode();
        }

    }

    public void OnCollisionEnter(GameObject collidedGameObject)
    {
        if (!collidedGameObject.CompareTag("Player") && !collidedGameObject.CompareTag("StormTrooperBullet"))
        {
            Detonate();
            this.gameObject.SetVelocity(new Vector3(0, 0, 0));

            BoxCollider myBoxColl = this.gameObject.GetComponent<BoxCollider>();

            if (myBoxColl != null)
                myBoxColl.active = true;
        }

    }

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        if (triggeredGameObject != null)
        {
            if (triggeredGameObject.tag == "Enemy")
            {
                enemies.Add(triggeredGameObject);
            }
        }
    }
    public void OnTriggerExit(GameObject triggeredGameObject)
    {
        if (triggeredGameObject != null)
        {
            if (triggeredGameObject.tag == "Enemy")
            {
                //if (!enemies.Remove(triggeredGameObject))
                //	Debug.Log("can't remove");

                foreach (GameObject item in enemies)
                {
                    if (item.GetUid() == triggeredGameObject.GetUid())

                        if (enemies.Remove(item))
                            Debug.Log("removed");

                    break;
                }
            }
        }
    }

    private void Detonate()
    {
        if (detonate == true)
            return;

        if (grenadeActiveObj != null)
        {
            grenadeActivePar = grenadeActiveObj.GetComponent<ParticleSystem>();
            if (grenadeActivePar != null)
                grenadeActivePar.Play();
        }
        if (granadeAreaObj != null)
        {
            granadeArea = granadeAreaObj.GetComponent<ParticleSystem>();
            if (granadeArea != null)
                granadeArea.Play();
        }
        detonate = true;
        Audio.PlayAudio(gameObject, "Play_Mando_Grenade_Set_Up");
        lifeTimer = 0;


        if (EnemyManager.currentEnemies != null)
        {
            for (int i = 0; i < EnemyManager.currentEnemies.Count; i++)
            {
                float distance = EnemyManager.currentEnemies[i].transform.globalPosition.Distance(this.gameObject.transform.globalPosition);

                if (distance <= areaRadius)
                    enemies.Add(EnemyManager.currentEnemies[i]);

            }

        }
    }

    private void Explode()
    {
        InternalCalls.CreatePrefab("Library/Prefabs/2084632366.prefab", gameObject.transform.globalPosition, new Quaternion(0.0f, 0.0f, 0.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f));
        InternalCalls.Destroy(gameObject);
    }
}