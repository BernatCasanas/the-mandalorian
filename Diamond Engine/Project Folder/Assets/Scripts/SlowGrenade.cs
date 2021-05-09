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

    public float damage = 5.0f;
    public float procTime = 0.25f;
    private float procTimer = 0f;
    private bool procActivation = false;
    List<GameObject> enemies = new List<GameObject>();

    //public void Awake()
    //   {
    //	Vector3 myForce = gameObject.transform.GetForward() * force;
    //	gameObject.AddForce(myForce);
    //   }
    public void Update()
    {
        if (start == true)
        {
            start = false;
            float modifier = 1;
            if(Core.instance != null)
            if (Core.instance.HasStatus(STATUS_TYPE.SEC_RANGE))
                modifier = 1 + Core.instance.GetStatusData(STATUS_TYPE.SEC_RANGE).severity /100;
            Vector3 myForce = gameObject.transform.GetForward() * forwardForce * modifier;

            myForce.y = upForce;

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
                grenadeDamage *= Core.instance.GetGrenadeDamageMod();

            explosionTimer += Time.deltaTime;
            procTimer += Time.deltaTime;

            if (procTimer > procTime)
            {
                procActivation = true;
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
                        Core.instance.hud.GetComponent<HUD>().AddToCombo(5, 1.45f);
                        script.TakeDamage(grenadeDamage);
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
                            bossScript.TakeDamage(grenadeDamage);
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
                                skelScript.TakeDamage(grenadeDamage);
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
                                wampaScript.TakeDamage(grenadeDamage);
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
            lifeTimer = 0;

        }
        if(Core.instance != null && Core.instance.HasStatus(STATUS_TYPE.SEC_DURATION))
        {
            if (explosionTimer >= explosionTime * (1 + Core.instance.GetStatusData(STATUS_TYPE.SEC_DURATION).severity / 100))
                InternalCalls.Destroy(gameObject);
        }
        else
        {
            if (explosionTimer >= explosionTime)
                InternalCalls.Destroy(gameObject);
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
}