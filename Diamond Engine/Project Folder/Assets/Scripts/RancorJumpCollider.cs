using System;
using DiamondEngine;

public class RancorJumpCollider : DiamondComponent
{
    public float colliderSpeed = 1.0f;
    public float lifeTime = 2.0f;

    public bool startState = false;

    public int damage = 10;

    public float deltaTimeMult = 1f;


    public void Update()
    {
        if (lifeTime > 0.0f)
        {
            lifeTime -= Time.deltaTime * deltaTimeMult;

            if (lifeTime <= 0.0f)
                InternalCalls.Destroy(gameObject);
        }

        gameObject.transform.localPosition -= new Vector3(0, colliderSpeed * Time.deltaTime, 0);
    }

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        if (triggeredGameObject.CompareTag("Player"))
        {
            PlayerHealth health = triggeredGameObject.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(damage);
        }
    }

}