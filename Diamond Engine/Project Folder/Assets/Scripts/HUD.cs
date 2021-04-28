using System;
using DiamondEngine;
using System.Collections.Generic;

//used for when the combo is incremented; easily expanded
class ComboLvlUpEffects
{
    //Constructors
    public ComboLvlUpEffects()
    {
        colorUpdate = new Vector3(1.0f, 1.0f, 1.0f); //default color case
        forceBarPercentageRecovery = 0.0f;
    }
    public ComboLvlUpEffects(Vector3 color, float forceBarRecoveryPercent)
    {
        colorUpdate = color;
        forceBarPercentageRecovery = forceBarRecoveryPercent;
    }
    // Copy constructor.
    public ComboLvlUpEffects(ComboLvlUpEffects previouscomboLvlUp)
    {
        colorUpdate = previouscomboLvlUp.colorUpdate;
        forceBarPercentageRecovery = previouscomboLvlUp.forceBarPercentageRecovery;
    }

    public Vector3 colorUpdate = new Vector3(1.0f, 1.0f, 1.0f); //default color case
    public float forceBarPercentageRecovery = 0.0f;
}

//used for when the combo ends; easily expanded
class ComboResetEffects
{
    //Constructors
    public ComboResetEffects()
    {
        hpToRestore = 0.0f;
    }
    public ComboResetEffects(float hpToRestore)
    {
        this.hpToRestore = hpToRestore;
    }
    // Copy constructor.
    public ComboResetEffects(ComboResetEffects previouscomboReset)
    {
        hpToRestore = previouscomboReset.hpToRestore;
    }

    public float hpToRestore = 0.0f;
}

public class HUD : DiamondComponent
{
    public int hp = 0;
    public int max_hp = 0;
    public int force = 0;
    public int max_force = 0;
    //public int currency = 10000;
    public float primaryWeaponHeat = 0f;
    public float primaryWeaponMaxHeat = 100f;
    public Vector3 primaryWeaponColorStart = new Vector3(0f, 1.2f, 0f);
    public Vector3 primaryWeaponColorEnd = new Vector3(1f, 0f, 0f);
    private bool primaryWeaponOverheat = false;
    public int bullets_secondary_weapon = 0;
    public int max_bullets_secondary_weapon = 0;
    public bool main_weapon = true;
    public GameObject hp_bar = null;
    public GameObject hp_number_gameobject = null;
    public GameObject force_bar = null;
    public GameObject skill_push = null;
    public GameObject weapon_bar = null;
    public GameObject primary_weapon = null;
    public GameObject secondary_weapon = null;
    public GameObject currency_number_gameobject = null;
    public GameObject combo_bar = null;
    public GameObject combo_text = null;
    public GameObject combo_gameobject = null;
    public GameObject max_hp_number = null;
    public GameObject fpsText = null;
    private bool start = true;
    private float pulsation_rate = 0.0f;
    private bool pulsation_forward = true;

    private float lastFrameTime = 0.0f;
    private float fullComboTime = 0.0f;
    private float currComboTime = 0.0f;
    public int comboNumber = 0;
    float comboDecrementMultiplier = 1.0f;
    float lastWeaponDecrementMultiplier = 1.0f;
    public float last_hp = 0;
    private bool hp_descending;
    private bool anakinBoonApplied = false;
    public GameObject shooting_blink = null;
    private float shoot_time = 0.0f;

    bool comboHUDneedsUpdate = false;
    Vector3 comboColor = Vector3.one;

    //stores the level as a key and the reward as a value
    Dictionary<int, ComboLvlUpEffects> lvlUpComboRewards = new Dictionary<int, ComboLvlUpEffects>
    {
        { 0,   new ComboLvlUpEffects(   new Vector3(0.0f,0.8f,1.0f),    0.0f)},
        { 3,   new ComboLvlUpEffects(   new Vector3(0.0f,1.0f,0.0f),    0.05f)},
        { 8,   new ComboLvlUpEffects(   new Vector3(1.0f,1.0f,0.0f),    0.1f)},
        { 15,   new ComboLvlUpEffects(   new Vector3(0.79f,0.28f,0.96f), 0.15f)},
        { 25,   new ComboLvlUpEffects(   new Vector3(1.0f,1.0f,1.0f),    0.2f)},
            //TODO Add lvlUpRewards here
    };
    //stores the level as a key and the reward as a value
    Dictionary<int, ComboResetEffects> endOfComboRewards = new Dictionary<int, ComboResetEffects>
    {
        { 0,   new ComboResetEffects( 0.0f)},
        { 8,   new ComboResetEffects( 0.05f)},
        { 15,   new ComboResetEffects( 0.12f)},
        { 25,   new ComboResetEffects( 0.2f)},
            //TODO Add endOfComboRewards here
    };
    public void Awake()
    {
    }

    public void Update()
    {
        hp_descending = false;
        if (start)
        {
            comboColor = Vector3.one;
            UpdateHP(PlayerHealth.currHealth, PlayerHealth.currMaxHealth);
            ResetCombo();
            UpdateForce(BabyYoda.instance.GetCurrentForce(), BabyYoda.GetMaxForce());
            max_hp_number.GetComponent<Text>().text = PlayerHealth.currMaxHealth.ToString();
            last_hp = Mathf.Lerp(last_hp, PlayerHealth.currHealth - 0.5f, 2.5f * Time.deltaTime);

            primaryWeaponHeat = 0f;
            primaryWeaponMaxHeat = 100f;
            primaryWeaponColorStart = new Vector3(1f, 1.2f, 0f);
            primaryWeaponColorEnd = new Vector3(1f, 0f, 0f);

            weapon_bar.GetComponent<Material>().SetVectorUniform("textureColorModStart", primaryWeaponColorStart);
            weapon_bar.GetComponent<Material>().SetVectorUniform("textureColorModEnd", primaryWeaponColorEnd);

            UpdateCombo();

            start = false;
        }
        /*if (Input.GetKey(DEKeyCode.C) == KeyState.KEY_DOWN)
        {
            currency++;
            UpdateCurrency(currency);
        }*/
        if (Input.GetKey(DEKeyCode.H) == KeyState.KEY_DOWN)
        {
            if (hp < max_hp)
            {
                //hp += 5;
                //UpdateHP(hp, max_hp);
                //last_hp = hp;
            }
        }
        if (Input.GetKey(DEKeyCode.D) == KeyState.KEY_DOWN)
        {
            if (hp > 0)
            {
                //if (!(last_hp > hp))
                //last_hp = hp;
                //hp -= 5;
                //UpdateHP(hp, max_hp);
            }
        }
        if (Input.GetKey(DEKeyCode.F) == KeyState.KEY_DOWN)
        {
            if (BabyYoda.instance != null)
            {
                if (BabyYoda.instance.GetCurrentForce() > 0)
                {
                    BabyYoda.instance.SetCurrentForce(BabyYoda.instance.GetCurrentForce() - 10);
                }
            }

        }
        if (Input.GetKey(DEKeyCode.M) == KeyState.KEY_DOWN)
        {
            if (BabyYoda.instance != null)
            {
                if (BabyYoda.instance.GetCurrentForce() < BabyYoda.GetMaxForce())
                {
                    BabyYoda.instance.SetCurrentForce(BabyYoda.instance.GetCurrentForce() + 10);
                }
            }
        }
        if (Input.GetKey(DEKeyCode.S) == KeyState.KEY_DOWN)
        {
            if (main_weapon)
            {
                if (primaryWeaponHeat < 100)
                {
                    AddPrimaryHeatAmount(10);
                }
            }
            else
            {
                if (bullets_secondary_weapon > 0)
                {
                    bullets_secondary_weapon--;
                    UpdateBullets(bullets_secondary_weapon, max_bullets_secondary_weapon);

                }
            }


        }
        if (Input.GetKey(DEKeyCode.R) == KeyState.KEY_DOWN)
        {
            if (main_weapon)
            {
                if (primaryWeaponHeat < 100)
                {
                    ReducePrimaryHeat(10);
                }
            }
            else
            {
                if (bullets_secondary_weapon < 10)
                {
                    bullets_secondary_weapon++;
                    UpdateBullets(bullets_secondary_weapon, max_bullets_secondary_weapon);
                }
            }

        }
        if (Input.GetKey(DEKeyCode.B) == KeyState.KEY_DOWN) //test key
        {
            AddToCombo(20, 1.0f);
        }
        if (Input.GetKey(DEKeyCode.N) == KeyState.KEY_DOWN) //test key
        {
            DecreaseComboPercentage(0.3f);
        }
        if (comboNumber > 0)
        {
            UpdateCombo();
        }
        last_hp = Mathf.Lerp(last_hp, PlayerHealth.currHealth /*- 0.5f*/, 2.5f * Time.deltaTime);
        if (last_hp > PlayerHealth.currHealth)
        {
            if (hp_bar != null)
                hp_bar.GetComponent<Material>().SetFloatUniform("last_hp", last_hp / PlayerHealth.currMaxHealth);
            hp_descending = true;
        }
        if (pulsation_forward)
        {
            pulsation_rate += (Time.deltaTime / 3);
            if (pulsation_rate > 1.0f) pulsation_forward = false;
        }
        else if (!pulsation_forward)
        {
            pulsation_rate -= (Time.deltaTime / 3);
            if (pulsation_rate < 0.6f) pulsation_forward = true;
        }
        if (hp_bar != null)
        {
            hp_bar.GetComponent<Material>().SetFloatUniform("t", pulsation_rate);

        }
        if (shoot_time > 0.0f)
            shoot_time -= Time.deltaTime;
        if (shoot_time < 0.0f)
        {
            ShootSwapImage(false);
            shoot_time = 0.0f;
        }

        if (comboHUDneedsUpdate)
        {
            UpdateComboHUD();
        }

        if (fpsText != null)
        {
            if (DebugOptionsHolder.showFPS)
            {
                if (!fpsText.IsEnabled()) fpsText.Enable(true);
                fpsText.GetComponent<Text>().text = ((int)(1000 * Time.deltaTime)).ToString();
            }
            else if (fpsText.IsEnabled()) fpsText.Enable(false);
        }
    }

    public void AddToCombo(float comboUnitsToAdd, float weaponDecreaseTimeMultiplier)
    {
        if (comboNumber == 0)
            ++comboNumber;

        lastWeaponDecrementMultiplier = weaponDecreaseTimeMultiplier;
        float tmpLastWeaponMod = lastWeaponDecrementMultiplier >= 1.0f ? lastWeaponDecrementMultiplier : 1.0f;
        currComboTime += comboUnitsToAdd * (1 / (tmpLastWeaponMod * comboDecrementMultiplier));

        if (currComboTime > fullComboTime)
        {
            float extraCombo = currComboTime - fullComboTime;
            extraCombo = IncrementCombo(extraCombo);
            AddToCombo(extraCombo, weaponDecreaseTimeMultiplier);

            OnLvlUpComboChange();
        }

        if (comboNumber > 0 && combo_gameobject != null && !combo_gameobject.IsEnabled())
        {
            combo_gameobject.Enable(true);
        }

        comboHUDneedsUpdate = true;
    }

    float IncrementCombo(float extraCombo = 0f)
    {
        //TODO make full combo time not a linear function
        ++comboNumber;
        comboDecrementMultiplier += 0.06f;

        if (extraCombo > (fullComboTime * 0.33f))
        {
            currComboTime = 0f;
        }
        else if (extraCombo > 0f)
        {
            float percentageToSum = (extraCombo / fullComboTime);
            percentageToSum = Math.Max(percentageToSum, 0.25f);

            currComboTime = fullComboTime * percentageToSum;
            extraCombo = 0;
        }


        if (!anakinBoonApplied && PlayerResources.CheckBoon(BOONS.BOON_ANAKINKILLSTREAK))
        {
            if (comboNumber >= 50)
            {
                Core.instance.movementSpeed += Core.instance.movementSpeed * 0.2f;
                anakinBoonApplied = true;
            }
        }
        else if (anakinBoonApplied)
        {
            if (comboNumber < 50)
            {
                Core.instance.movementSpeed -= Core.instance.movementSpeed * 0.2f;
                anakinBoonApplied = false;
            }
        }

        return extraCombo;
    }

    public void SubstractToCombo(float amount)
    {
        if (currComboTime <= 0.0f)
        {
            return;
        }

        if (amount < 0f)
            amount = -amount;

        Debug.Log("Combo amount substracted on hit: " + amount.ToString());

        currComboTime -= amount;

        if (currComboTime <= 0.0f)
        {
            currComboTime = 0f;
            ResetCombo(true);
        }

    }

    bool UpdateCombo()
    {
        float toSubstract = currComboTime - Mathf.Lerp(currComboTime, -0.0f, Time.deltaTime * comboDecrementMultiplier * lastWeaponDecrementMultiplier);

        toSubstract = Math.Max(toSubstract, Time.deltaTime * comboDecrementMultiplier * lastWeaponDecrementMultiplier);
        toSubstract = Math.Min(toSubstract, Time.deltaTime * 3f * comboDecrementMultiplier * fullComboTime * 0.085f);
        currComboTime -= toSubstract;

        if (currComboTime <= 0.0f)
        {
            currComboTime = 0.0f;
            ResetCombo(true);
            return false;
        }
        comboHUDneedsUpdate = true;
        return true;
    }

    void UpdateComboHUD()
    {
        if (combo_text != null && combo_text.IsEnabled())
        {
            Text t = combo_text.GetComponent<Text>();
            if (t != null)
            {
                t.text = "x" + comboNumber.ToString();
                t.color_red = comboColor.x;
                t.color_green = comboColor.y;
                t.color_blue = comboColor.z;
            }
        }


        if (combo_bar != null && combo_bar.IsEnabled())
        {
            Material m = combo_bar.GetComponent<Material>();
            if (m != null)
            {
                m.SetIntUniform("combo_number", comboNumber);
                m.SetFloatUniform("length_used", Mathf.InvLerp(0, fullComboTime, currComboTime));

                m.SetFloatUniform("r", comboColor.x);
                m.SetFloatUniform("g", comboColor.y);
                m.SetFloatUniform("b", comboColor.z);
            }
        }

        comboHUDneedsUpdate = false;
    }

    public void ResetCombo(bool applyRewards = false)
    {
        if (applyRewards)
        {

            ComboResetEffects endOfComboData = new ComboResetEffects();
            bool lastEffect = false;
            int key = 0;
            foreach (KeyValuePair<int, ComboResetEffects> reward in endOfComboRewards)
            {
                lastEffect = false;
                key = reward.Key;
                if (comboNumber >= key)
                {
                    endOfComboData = reward.Value;
                    lastEffect = true;
                }
            }

            float hpPercentageToIncrease = 0.0f;
            if (lastEffect)//special formula for when the effect applied is the last one in the dictionary
            {
                int comboLvlToOffset = 10; //every X lvls, offset will increase by 1
                int comboLvlOffset = (comboNumber - key) / comboLvlToOffset;
                hpPercentageToIncrease = endOfComboData.hpToRestore + (0.05f * comboLvlOffset);
            }
            else //normal reward case
            {
                hpPercentageToIncrease = endOfComboData.hpToRestore;
            }


            if (Core.instance != null && Core.instance.gameObject != null && Core.instance.gameObject.GetComponent<PlayerHealth>() != null)
            {
                Core.instance.gameObject.GetComponent<PlayerHealth>().HealPercentMax(hpPercentageToIncrease);
            }

        }

        comboNumber = 0;
        comboDecrementMultiplier = 1.0f;
        lastWeaponDecrementMultiplier = 1.0f;
        fullComboTime = 100.0f;
        currComboTime = 0.0f;

        OnLvlUpComboChange();


        comboHUDneedsUpdate = true;

        if (combo_gameobject != null && combo_gameobject.IsEnabled())
            combo_gameobject.Enable(false);
    }

    //Updates the combo color + gives rewards
    void OnLvlUpComboChange()
    {
        ComboLvlUpEffects lvlUpComboData = new ComboLvlUpEffects();
        foreach (KeyValuePair<int, ComboLvlUpEffects> lvlUpChange in lvlUpComboRewards)
        {
            int key = lvlUpChange.Key;
            if (comboNumber >= key)
            {
                lvlUpComboData = lvlUpChange.Value;
            }
        }

        if (BabyYoda.instance != null)
            UpdateForce((int)(BabyYoda.instance.GetCurrentForce() + (BabyYoda.GetMaxForce() * lvlUpComboData.forceBarPercentageRecovery)), BabyYoda.GetMaxForce()); //TODO check this works fine (at the time of creating this line force doesn't work and this part of the method cannot be tested)

        comboColor.x = lvlUpComboData.colorUpdate.x;
        comboColor.y = lvlUpComboData.colorUpdate.y;
        comboColor.z = lvlUpComboData.colorUpdate.z;

        comboHUDneedsUpdate = true;
    }

    //percentage between 0 and 1 (0%-100%) decreases the time left for the current combo to deplete
    public void DecreaseComboPercentage(float percentageOfTotal)
    {
        currComboTime -= fullComboTime * percentageOfTotal;

        if (currComboTime < 0.0f)
            currComboTime = 0.0f;
    }

    public void UpdateHP(int new_hp, int max_hp)
    {
        if (hp_number_gameobject != null)
            hp_number_gameobject.GetComponent<Text>().text = new_hp.ToString();
        float hp_float = new_hp;
        hp_float /= max_hp;
        if (!(last_hp > new_hp))
            last_hp = new_hp;
        if (hp_bar == null)
            return;
        hp_bar.GetComponent<Material>().SetFloatUniform("length_used", hp_float);
        hp_bar.GetComponent<Material>().SetFloatUniform("last_hp", last_hp / max_hp);
        max_hp_number.GetComponent<Text>().text = PlayerHealth.currMaxHealth.ToString();
    }

    public void UpdateForce(int new_force, int max_force)
    {
        if (force_bar == null)
            return;

        float force_float = new_force;
        force_float /= max_force;
        //force_float = Math.Max(force_float, 1.0f); CANNOT DO THIS! This returns 1.0 and it's not a wanted value
        force_bar.GetComponent<Material>().SetFloatUniform("length_used", force_float);

        if (new_force >= BabyYoda.GetPushCost())
            ChangeAlphaSkillPush(1.0f);
        else
            ChangeAlphaSkillPush(0.0f);
    }

    public void ChangeAlphaSkillPush(float alpha)
    {
        if (skill_push == null)
            return;

        //Debug.Log("UUUUUUUUUUUUUUUUUUUU: " + alpha.ToString());

        skill_push.GetComponent<Material>().SetFloatUniform("alpha", alpha);
    }

    public void ShootSwapImage(bool shoot)
    {
        if (shooting_blink != null)
        {
            if (shoot)
            {
                shooting_blink.Enable(true);
                shoot_time = 0.3f;
                return;
            }
            else
            {
                shooting_blink.Enable(false);
                return;
            }
        }
    }

    public void UpdateBullets(int num_bullets, int max_bullets)
    {
        if (weapon_bar == null)
            return;

        float bullets_float = num_bullets;
        bullets_float /= max_bullets;
        weapon_bar.GetComponent<Material>().SetFloatUniform("length_used", bullets_float);
    }

    public void UpdateCurrency(int total_currency)
    {
        if (currency_number_gameobject == null)
            return;
        currency_number_gameobject.GetComponent<Text>().text = total_currency.ToString();
    }

    #region PRIMARY_HEAT

    public void AddPrimaryHeatShot()
    {
        float currentHeat = primaryWeaponHeat > 0 ? primaryWeaponHeat : 0.01f;

        float newValue = (float)(Math.Log(currentHeat * 5) - Math.Log(0.01f)) / (3 / 2.6f);

        Debug.Log("Added Heat: " + newValue);

        AddPrimaryHeatAmount(newValue);
    }

    public void AddPrimaryHeatAmount(float amount)
    {
        primaryWeaponHeat += amount;

        if (primaryWeaponHeat >= primaryWeaponMaxHeat)
        {
            SetPrimaryOVerheat(true);
            primaryWeaponHeat = primaryWeaponMaxHeat;
        }

        UpdateHeat();
    }

    public void ReducePrimaryHeat(float amount)
    {
        if (amount < 0f)
            amount = -amount;

        primaryWeaponHeat -= amount;

        if (primaryWeaponHeat <= 0f)
        {
            SetPrimaryOVerheat(false);
            primaryWeaponHeat = 0f;
        }

        UpdateHeat();
    }

    public void SetMaxPrimaryHeat(float maxAmount)
    {
        primaryWeaponMaxHeat = maxAmount;
        UpdateHeat();
    }

    public void UpdateHeat()
    {
        if (weapon_bar == null)
        {
            Debug.Log("Weapon heat bar is null!!");
            return;
        }

        float newHeatValue = primaryWeaponHeat / primaryWeaponMaxHeat;

        weapon_bar.GetComponent<Material>().SetFloatUniform("length_used", newHeatValue);

        if (primaryWeaponOverheat == false)
            weapon_bar.GetComponent<Material>().SetFloatUniform("colorLerpLength", newHeatValue);

    }

    private void SetPrimaryOVerheat(bool newState)
    {
        if (newState == true)
        {
            primaryWeaponOverheat = true;
            weapon_bar.GetComponent<Material>().SetFloatUniform("colorLerpLength", 1f);
        }
        else
        {
            primaryWeaponOverheat = false;
            weapon_bar.GetComponent<Material>().SetFloatUniform("colorLerpLength", 0f);
        }

    }

    public float GetPrimaryHeat()
    {
        return primaryWeaponHeat;
    }

    public float GetPrimaryMaxHeat()
    {
        return primaryWeaponMaxHeat;
    }

    public bool IsPrimaryOverheated()
    {
        return primaryWeaponOverheat;
    }

    #endregion

}