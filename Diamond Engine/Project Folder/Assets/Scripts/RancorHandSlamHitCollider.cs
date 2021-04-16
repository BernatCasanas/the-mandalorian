using DiamondEngine;

class RancorHandSlamHitCollider : DiamondComponent
{
    private int damage = 25;

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        if (triggeredGameObject.CompareTag("Player"))
        {
            PlayerHealth health = triggeredGameObject.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(damage);
            Debug.Log("Damaged From HandSlamHit");
        }
    }
}

