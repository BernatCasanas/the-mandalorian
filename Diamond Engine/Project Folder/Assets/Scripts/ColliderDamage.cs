using DiamondEngine;

class ColliderDamage : DiamondComponent
{
    public int damage = 0;
    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        if (triggeredGameObject.CompareTag("Player"))
        {
            PlayerHealth health = triggeredGameObject.GetComponent<PlayerHealth>();
            float damageMod = 1;
            if (Core.instance.HasStatus(STATUS_TYPE.ITSA_TRAP))
                damageMod = 1 + Core.instance.GetStatusData(STATUS_TYPE.ITSA_TRAP).severity/100;
            if (health != null)
                health.TakeDamage((int)(damage * damageMod));
        }
    }
}

