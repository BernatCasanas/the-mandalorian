using System;
using DiamondEngine;

public class PlayerHealth : DiamondComponent
{
    public int startHealth;
    public static int currMaxHealth { get; private set; }
    public static int currHealth { get; private set; }

    //Bo Katan’s resilience: each time you kill an enemy heal +1 HP.
    //0 means this boon is not working else heal the amount stored here
    public static int healWhenKillingAnEnemy { get; private set; }

    static bool firstFrame = true;//Only Once in the game?
    public void Update()
    {
        if (firstFrame)
        {
            firstFrame = false;
            ResetMaxAndCurrentHPToDefault();

        }
    }

    //Increments the max Hp by the percentatge given as a parameter (1 = 100% 0 = 0%) It can also be negative to substract HP
    public int IncrementMaxHpPercent(float percent,bool alsoRestoreAllHP=false)
    {
        currMaxHealth += (int)(currMaxHealth * percent);

        if (currMaxHealth < 1)
            currMaxHealth = 1;

        if(alsoRestoreAllHP)
        {
            currHealth = currMaxHealth;
        }

        return currMaxHealth;
    }

    //Increments the max Hp by the value given as a parameter. It can also be negative to substract HP
    public int IncrementMaxHpValue(int sumMaxHP, bool alsoRestoreAllHP = false)
    {
        currMaxHealth += sumMaxHP;

        if (currMaxHealth < 1)
            currMaxHealth = 1;

        if (alsoRestoreAllHP)
        {
            currHealth = currMaxHealth;
        }

        return currMaxHealth;
    }

    //Sets Max Hp to the value given
    public void SetMaxHPValue(int newMaxHP, bool alsoRestoreAllHP = false)
    {
        currMaxHealth = newMaxHP;

        if (currMaxHealth < 1)
            currMaxHealth = 1;

        if (alsoRestoreAllHP)
        {
            currHealth = currMaxHealth;
        }
    }


    //Adds a value to the current healing when killing an enemy. It can also be negative to substract Healing
    public int IncrementHealingWhenKillingEnemy(int increment)
    {
        healWhenKillingAnEnemy += increment;

        if (healWhenKillingAnEnemy < 0)
            healWhenKillingAnEnemy = 0;

        return healWhenKillingAnEnemy;
    }

    //Sets the value of the current healing when killing an enemy.
    public void SetHealingWhenKillingEnemy(int newHealing)
    {
        healWhenKillingAnEnemy = newHealing;

        if (healWhenKillingAnEnemy < 0)
            healWhenKillingAnEnemy = 0;
    }

    //Increments the current Hp by the percentatge given as a parameter (1 = 100% 0 = 0%) It can also be negative to take percentual damage
    public void HealPercent(float percent)
    {
        currHealth += (int)(currHealth * percent);
    }

    //When current HP drops to 0, Die() Method is called
    public void SetCurrentHP(int newCurrentHP)
    {
        currHealth = newCurrentHP;
        if (currHealth <= 0)
        {
            Die();
        }
    }

    //Also works as a HEAL AMOUNT when taking negative damage ;) When current HP drops to 0, Die() Method is called
    public void TakeDamage(int damage)
    {
        currHealth -= damage;

        if(currHealth<=0)
        {
            Die();
        }
    }

    
    public void Die()
    {
        //TODO die
    }

    public void ResetMaxAndCurrentHPToDefault()
    {
        if (startHealth <= 0)
            startHealth = 1;

        currHealth = currMaxHealth = startHealth;
    }

}