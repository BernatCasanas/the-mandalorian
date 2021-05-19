using System;
using DiamondEngine;

public class AtackBosslv2 : DiamondComponent
{
    public float damage = 0.0f;

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        if (triggeredGameObject.CompareTag("Player") && this.active)
        {
            PlayerHealth health = triggeredGameObject.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage((int)damage);
                gameObject.GetComponent<AtackBosslv2>().active = false;
            }
        }
    }
}