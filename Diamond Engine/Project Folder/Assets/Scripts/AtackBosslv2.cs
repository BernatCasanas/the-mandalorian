using System;
using DiamondEngine;

public class AtackBosslv2 : DiamondComponent
{
    public float damage = 0.0f;
    public GameObject camera = null;

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        if (triggeredGameObject.CompareTag("Player") && this.active)
        {
            PlayerHealth health = triggeredGameObject.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage((int)damage);
                gameObject.GetComponent<AtackBosslv2>().active = false;

                if (camera != null)
                {
                    Shake3D shake = camera.GetComponent<Shake3D>();
                    if (shake != null)
                    {
                        shake.StartShaking(0.8f, 0.1f);
                    }
                }
            }
        }
    }
}