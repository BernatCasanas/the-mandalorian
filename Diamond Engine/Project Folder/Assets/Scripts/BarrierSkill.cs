using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using DiamondEngine;

public class BarrierSkill : DiamondComponent
{
    public GameObject myMeshPivot = null;
    public GameObject wall_up = null;
    private ParticleSystem up = null;
    public GameObject wall_effects = null;
    private ParticleSystem effects = null;

    public float maxTimeAlive = 5.0f;
    float currTimeAlive = 0.0f;

    public float maxHP = 10.0f;
    float currHP = 0.0f;

    public float maxSpawnTimeAnim = 0.5f;
    float currSpawnTimeAnim = 0.0f;

    bool hasSpawned = false;
    bool once = true;
    public void Awake()
    {
        if (wall_up!=null)
            up = wall_up.GetComponent<ParticleSystem>();
        if(wall_effects != null)
            effects = wall_effects.GetComponent<ParticleSystem>();
        Reset();
    }

    public void Update()
    {
        if (!hasSpawned)
        {
            Spawning();
        }
        else
        {
            Living();
        }
        currTimeAlive -= Time.deltaTime;
    }

    private void Spawning()
    {
        if (once && up != null)
        {
            up.Play();
            once = false;
        }

        if (currSpawnTimeAnim <= 0.0f)
        {
            currSpawnTimeAnim = 0.0f;
            hasSpawned = true;
            once = true;
        }

        if (myMeshPivot != null)
        {
            Vector3 newScale = new Vector3(myMeshPivot.transform.localScale.x, myMeshPivot.transform.localScale.y, myMeshPivot.transform.localScale.z);
            newScale.y = Mathf.Remap(maxSpawnTimeAnim, 0.0f, 0.0f, 1.5f, currSpawnTimeAnim);
            myMeshPivot.transform.localScale = newScale;
        }
        currSpawnTimeAnim -= Time.deltaTime;
    }
    private void Living()
    {
        if (once && effects != null)
        {
            effects.Play();
            once = false;
        }

        if (currTimeAlive <= 0.0f)
        {
            Die();
            once = true;
        }
    }

    private void Die()
    {
        Reset();
        InternalCalls.Destroy(gameObject);
    }

    private void Reset()
    {
        currHP = maxHP;
        currTimeAlive = maxTimeAlive;
        currSpawnTimeAnim = maxSpawnTimeAnim;
        hasSpawned = false;
    }

    public void OnCollisionEnter(GameObject collidedGameObject)
    {
        if(collidedGameObject.CompareTag("Bullet"))
        {
           BH_Bullet bullet= collidedGameObject.GetComponent<BH_Bullet>();

            if(bullet!=null)
            {
                currHP -= bullet.damage;

                if(currHP<0.0f)
                {
                    Die();
                }
            }

        }
    }
}