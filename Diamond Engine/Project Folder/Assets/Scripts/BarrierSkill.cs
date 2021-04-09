using System;
using DiamondEngine;

public class BarrierSkill : DiamondComponent
{
    public GameObject myMeshPivot = null;

    public float maxTimeAlive = 5.0f;
    float currTimeAlive = 0.0f;

    public float maxHP = 10.0f;
    float currHP = 0.0f;

    public float maxSpawnTimeAnim = 0.5f;
    float currSpawnTimeAnim = 0.0f;

    bool hasSpawned = false;
    public void Awake()
    {
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

        if (currSpawnTimeAnim <= 0.0f)
        {
            currSpawnTimeAnim = 0.0f;
            hasSpawned = true;
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
        if (currTimeAlive <= 0.0f)
        {
            Die();
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