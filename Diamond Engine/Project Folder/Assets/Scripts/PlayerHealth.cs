using System;
using DiamondEngine;

public class PlayerHealth : DiamondComponent
{
    public static int currMaxHealth { get; private set; }
    public static int currHealth { get; private set; }

    //Bo Katan’s resilience: each time you kill an enemy heal +1 HP.
    //0 means this boon is not working else heal the amount stored here
    public static int healWhenKillingAnEnemy { get; private set; }
    public static int skill_chanceToAvoidDamage { get; set; }
    public GameObject character_mesh = null;
    public GameObject damage_screen = null;

    private bool die = false;
    private float damaged = 0.0f;
    private float t = 0.0f;

    public void Awake()
    {
        damage_screen = InternalCalls.FindObjectWithName("DamageScreen");

        if (damage_screen != null)
        {
            damage_screen.GetComponent<Material>().SetFloatUniform("alpha", 1.0f);
        }
        else
        { 
            Debug.Log("Damage Screen not found");

            skill_chanceToAvoidDamage = 0;
        }
    }

    public void Update()
    {
        if (die && !DebugOptionsHolder.godModeActive)
        {
            die = false;
            Die();
        }
        if (damaged > 0.01f)
        {
            damaged = Mathf.Lerp(damaged, 0.0f, 0.1f);
        }
        else
        {
            damaged = 0.0f;
        }
        if (character_mesh != null)
        {
            character_mesh.GetComponent<Material>().SetFloatUniform("damaged", damaged);
        }
        if (damage_screen != null)
        {
            damage_screen.GetComponent<Material>().SetFloatUniform("alpha", 1.0f);
            if (currHealth <= (currMaxHealth / 3))
                damage_screen.GetComponent<Material>().SetFloatUniform("alpha", currHealth / (currMaxHealth / 3));
            t += Time.deltaTime;
            if (t < 1.0f) t = 0.0f;
            damage_screen.GetComponent<Material>().SetFloatUniform("t", t);
        }

        if (Input.GetKey(DEKeyCode.COMMA) == KeyState.KEY_DOWN) Debug.Log(currHealth.ToString());
        if (Input.GetKey(DEKeyCode.M) == KeyState.KEY_DOWN) SetMaxHPValue(50, true);
    }

    //Increments the max Hp by the percentatge given as a parameter (1 = 100% 0 = 0%) It can also be negative to substract HP
    public int IncrementMaxHpPercent(float percent, bool alsoRestoreAllHP = false)
    {
        currMaxHealth += (int)(currMaxHealth * percent);

        if (currMaxHealth < 1)
            currMaxHealth = 1;

        if (alsoRestoreAllHP || currHealth > currMaxHealth)
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

        if (alsoRestoreAllHP || currHealth > currMaxHealth)
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

        if (alsoRestoreAllHP || currHealth > currMaxHealth)
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
        if (DebugOptionsHolder.godModeActive)
            return;

        currHealth += (int)(currHealth * percent);

        if (currHealth > currMaxHealth) currHealth = currMaxHealth;

        if (Core.instance.hud != null)
            Core.instance.hud.GetComponent<HUD>().UpdateHP(currHealth, currMaxHealth);

        if (currHealth <= 0)
        {
            die = true;
        }
    }
    //Increments the current Hp by the percentatge given as a parameter (1 = 100% 0 = 0%) It can also be negative to take percentual damage
    public void HealPercentMax(float percentofMaxHp)
    {
        if (DebugOptionsHolder.godModeActive)
            return;

        currHealth += (int)(currMaxHealth * percentofMaxHp);
        currHealth = Math.Min(currMaxHealth, currHealth);

        if (currHealth > currMaxHealth) currHealth = currMaxHealth;

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
        if (DebugOptionsHolder.godModeActive)
            return;

        currHealth = newCurrentHP;

        if (currHealth > currMaxHealth) currHealth = currMaxHealth;

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
        if (DebugOptionsHolder.godModeActive)
            return currHealth;

        if(Core.instance != null && damage > 0)
        {
            if (Core.instance.IsDashing())
                return currHealth;
        }

        if (ChanceToAvoidDamage(skill_chanceToAvoidDamage))
        {
            Debug.Log("Damage missed!");
            return currHealth; //We have avoided damage with a skill
        }

        if (PlayerResources.CheckBoon(BOONS.BOON_BOSSKSTRENGTH))
        {
            currHealth -= damage - (int)(damage * 0.1f);
        }
        else currHealth -= damage;


        if (currHealth <= 0)
        {
            die = true;
        }
        if (currHealth > currMaxHealth) currHealth = currMaxHealth;

        if (Core.instance.hud != null)
        {
            Core.instance.hud.GetComponent<HUD>().UpdateHP(currHealth, currMaxHealth);
            Core.instance.ReduceComboOnHit(damage);
        }

        if (currHealth < 75 && currHealth >= 50 && MusicSourceLocate.instance != null)
        {
            Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Health", "Injured");
        }
        else if (currHealth < 50 && currHealth > 0 && MusicSourceLocate.instance != null)
        {
            Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Health", "Critical");
        }
        damaged = (damage > 0) ? 1.0f : damaged;
        Debug.Log("Current health: " + currHealth.ToString());
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

        currHealth = currMaxHealth = 100;//TODO set the starting heath here for now
    }

    //chanceToAvoid must be a number between 0 and 100 (%)
    private bool ChanceToAvoidDamage(int chanceToAvoid)
    {
        Random rnd = new Random();
        int randomNum = rnd.Next(1, 101);
        return chanceToAvoid >= randomNum;
    }

    public void SetSkill(string skillName, float value = 0.0f)
    {
        if (skillName == "DAvoidDmg")
        {
            skill_chanceToAvoidDamage = (int)value;
        }
    }


    public void ToggleNoClip(bool val)
    {
        if (val)
        {
            Core.instance.gameObject.GetComponent<BoxCollider>().active = false;
            //TODO: rigidbody gravity
        }
        else
        {
            Core.instance.gameObject.GetComponent<BoxCollider>().active = true;
            //TODO: rigidbody gravity
        }
    }
}