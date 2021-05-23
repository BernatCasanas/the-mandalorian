using System;
using DiamondEngine;

public class DPSDamage : DiamondComponent
{
    public bool onTriggerStay = false;
    public int base_damage = 4;
    public int damage = 0;
    public float ticksPerSecond = 1f;

    public float damageTimer = 1.0f;



    //public Func<int,bool,int> onDealDamage;


    public void Awake()
    {
        damage = base_damage;
        damageTimer = ticksPerSecond;
        //if (Core.instance != null)
        //{
        //    onDealDamage = Core.instance.gameObject.GetComponent<PlayerHealth>().TakeDamage;
        //}
    }

    public void Update()
    {
        if (!onTriggerStay)
        {

            return;
        }

        if (damageTimer < ticksPerSecond)
        {
            damageTimer += Time.deltaTime;
        }
        else
        {
            damageTimer = 0.0f;
            //onDealDamage?.Invoke(damage,false);
            Core.instance.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage, true);
            IncrementDamage();
            Debug.Log("Water Damage");
        }
    }


    public void OnTriggerEnter(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            onTriggerStay = true;
        }
    }

    public void OnTriggerExit(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            onTriggerStay = false;
            damageTimer = 1;
            damage = base_damage;
        }
    }

    private void IncrementDamage()
    {
        if (damage < 10)
        {
            damage += 2;
            if (damage > 10)
            {
                damage = 10;
            }
        }
    }
}