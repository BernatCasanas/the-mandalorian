using System;
using DiamondEngine;

public class LaserTrap : DiamondComponent
{
    public int damage = 5;

    public void OnCollisionEnter(GameObject collidedGameObject)
    {
        if(collidedGameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collidedGameObject.GetComponent<PlayerHealth>();

            if(playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}