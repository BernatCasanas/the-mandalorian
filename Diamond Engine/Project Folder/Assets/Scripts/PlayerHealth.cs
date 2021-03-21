using System;
using DiamondEngine;

public class PlayerHealth : DiamondComponent
{
    public static int currMaxHealth { get; private set; }
    public static int currHealth { get; private set; }

    //Bo Katan’s resilience: each time you kill an enemy heal +1 HP.
    //0 means this boon is not working else heal the amount stored here
    public static int healWhenKillingAnEnemy { get; private set; }

   
    private bool die = false;

    public void Update()
    {
        if (die)
        {
            die = false;
            Die();
        }
    }

    //Increments the max Hp by the percentatge given as a parameter (1 = 100% 0 = 0%) It can also be negative to substract HP
    public int IncrementMaxHpPercent(float percent, bool alsoRestoreAllHP = false)
    {
        currMaxHealth += (int)(currMaxHealth * percent);

        if (currMaxHealth < 1)
            currMaxHealth = 1;

        if (alsoRestoreAllHP)
        {
            currHealth = currMaxHealth;
        }

        if (Core.instance.hud != null)
            Core.instance.hud.GetComponent<HUD>().UpdateHP(currHealth, currMaxHealth);
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

        if (Core.instance.hud != null)
            Core.instance.hud.GetComponent<HUD>().UpdateHP(currHealth, currMaxHealth);
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
        if (Core.instance.hud != null)
            Core.instance.hud.GetComponent<HUD>().UpdateHP(currHealth, currMaxHealth);
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

        if (Core.instance.hud != null)
            Core.instance.hud.GetComponent<HUD>().UpdateHP(currHealth, currMaxHealth);

        if (currHealth <= 0)
        {
            die = true;
        }
    }

    //When current HP drops to 0, Die() Method is called
    public void SetCurrentHP(int newCurrentHP)
    {
        currHealth = newCurrentHP;

        if (Core.instance.hud != null)
            Core.instance.hud.GetComponent<HUD>().UpdateHP(currHealth, currMaxHealth);

        if (currHealth <= 0)
        {
            die = true;
        }
    }

    //Also works as a HEAL AMOUNT when taking negative damage ;) When current HP drops to 0, Die() Method is called
    public int TakeDamage(int damage)
    {
        currHealth -= damage;

        if (Core.instance.hud != null)
            Core.instance.hud.GetComponent<HUD>().UpdateHP(currHealth, currMaxHealth);

        if (currHealth <= 0)
        {
            die = true;
        }

        if (currHealth < 75 && currHealth >= 50 && MusicSourceLocate.instance != null)
        {
            Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Health", "Injured");
        }
        else if (currHealth < 50 && currHealth > 0 && MusicSourceLocate.instance != null)
        {
            Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Health", "Critical");
        }


        Debug.Log("Current health: " + currHealth);
        return currHealth;
    }


    public void Die()
    {
        ResetMaxAndCurrentHPToDefault();
        //TODO die
        Audio.PlayAudio(gameObject, "Play_Mando_Death");
        // Set as defeat:
        Counter.gameResult = Counter.GameResult.DEFEAT;
        // When the player has died load the scene:
        SceneManager.LoadScene(821370213);


    }

    public static void ResetMaxAndCurrentHPToDefault()
    {

        healWhenKillingAnEnemy = 0;

        currHealth = currMaxHealth = 50;//TODO set the starting heath here for now

    }

}