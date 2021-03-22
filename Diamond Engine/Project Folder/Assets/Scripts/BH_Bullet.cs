using System;
using DiamondEngine;


public class BH_Bullet : DiamondComponent
{
    public float speed = 60.0f;
    public float maxLifeTime = 5.0f;

    public float currentLifeTime = 0.0f;

    public float yVel = 0.0f;
    public float damage = 5.0f;

    private bool started = false;

    public void Update()
    {
        if (started == false)
        {
            started = true;
            InternalCalls.CreatePrefab("Library/Prefabs/150008798.prefab", gameObject.transform.globalPosition, gameObject.transform.globalRotation, gameObject.transform.globalScale);
        }

        currentLifeTime += Time.deltaTime;

        gameObject.transform.localPosition += gameObject.transform.GetForward() * (speed * Time.deltaTime);

        //yVel -= Time.deltaTime / 15.0f;
        //gameObject.transform.localPosition += (Vector3.up * yVel);

        if (currentLifeTime >= maxLifeTime)
        {
            InternalCalls.Destroy(this.gameObject);
        }
    }

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        InternalCalls.Destroy(gameObject);
    }
}