using System;
using DiamondEngine;

public class HeavyTrooperSpear : DiamondComponent
{
    public int damage = 0;

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {

        if(triggeredGameObject.CompareTag("Player"))
        {
            Debug.Log("Player Hit with damage: " + damage);
            PlayerHealth playerHealth = Core.instance.gameObject.GetComponent<PlayerHealth>();

            if(playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }

}