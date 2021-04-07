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
    public int currency = 10000;
    public int bullets_main_weapon = 0;
    public int max_bullets_main_weapon = 0;
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
    public GameObject force_wave = null;
    public GameObject force_wave_second = null;
    public GameObject force_wave_third = null;
    public GameObject hp_glitch = null;
    private bool start = true;
    private float pulsation_rate = 0.0f;
    private bool pulsation_forward = true;


    private float fullComboTime = 0.0f;
    private float currComboTime = 0.0f;
    public int comboNumber = 0;
    float comboDecrementMultiplier = 1.0f;
    float lastWeaponDecrementMultiplier = 1.0f;
    public float force_bar_rate = 0.0f;
    public float last_hp = 0;
    private bool hp_descending = false;

    //stores the level as a key and the reward as a value
    Dictionary<int, ComboLvlUpEffects> lvlUpComboRewards = new Dictionary<int, ComboLvlUpEffects>
    {
        { 0,   new ComboLvlUpEffects(   new Vector3(0.0f,0.8f,1.0f),    0.0f)},
        { 10,   new ComboLvlUpEffects(   new Vector3(0.0f,1.0f,0.0f),    0.05f)},
        { 25,   new ComboLvlUpEffects(   new Vector3(1.0f,1.0f,0.0f),    0.1f)},
        { 45,   new ComboLvlUpEffects(   new Vector3(0.79f,0.28f,0.96f), 0.15f)},
        { 77,   new ComboLvlUpEffects(   new Vector3(1.0f,1.0f,1.0f),    0.2f)},
            //TODO Add lvlUpRewards here
    };
    //stores the level as a key and the reward as a value
    Dictionary<int, ComboResetEffects> endOfComboRewards = new Dictionary<int, ComboResetEffects>
    {
        { 0,   new ComboResetEffects( 0.0f)},
        { 25,   new ComboResetEffects( 0.05f)},
        { 45,   new ComboResetEffects( 0.15f)},
        { 77,   new ComboResetEffects( 0.28f)},
            //TODO Add endOfComboRewards here
    };


    public void Update()
    {
        hp_descending = false;
        if (start)
        {
            UpdateHP(PlayerHealth.currHealth, PlayerHealth.currMaxHealth);
            ResetCombo();
            UpdateForce(force, max_force);
            force_bar_rate = -0.15f;
            start = false;
        }
        if (Input.GetKey(DEKeyCode.C) == KeyState.KEY_DOWN)
        {
            currency++;
            UpdateCurrency(currency);
        }
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
            if (force > 0)
            {
                force -= 10;
                if (force == 0)
                {
                    ChangeAlphaSkillPush(false);

                }
                UpdateForce(force, max_force);
            }
        }
        if (Input.GetKey(DEKeyCode.M) == KeyState.KEY_DOWN)
        {
            if (force < max_force)
            {
                if (force == 0)
                {
                    ChangeAlphaSkillPush(true);
                }
                force += 10;
                UpdateForce(force, max_force);

            }
        }
        if (Input.GetKey(DEKeyCode.S) == KeyState.KEY_DOWN)
        {
            if (main_weapon)
            {
                if (bullets_main_weapon > 0)
                {
                    bullets_main_weapon--;
                    UpdateBullets(bullets_main_weapon, max_bullets_main_weapon);
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
                if (bullets_main_weapon < 10)
                {
                    bullets_main_weapon++;
                    UpdateBullets(bullets_main_weapon, max_bullets_main_weapon);

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
        if (Input.GetKey(DEKeyCode.W) == KeyState.KEY_DOWN)
        {
            SwapWeapons();
        }
        if (Input.GetKey(DEKeyCode.B) == KeyState.KEY_DOWN) //test key
        {
            AddToCombo(20, 1.0f);
        }
        if (Input.GetKey(DEKeyCode.N) == KeyState.KEY_DOWN) //test key
        {
            DecreaseComboPercentage(0.3f);
        }
        if (combo_bar != null && comboNumber > 0)
        {
            UpdateCombo();
        }
        last_hp = Mathf.Lerp(last_hp, PlayerHealth.currHealth - 0.5f, 2.5f * Time.deltaTime);
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

        if (force_wave != null)
        {
            force_wave.GetComponent<Material>().SetFloatUniform("t", Time.totalTime);
            force_wave.GetComponent<Material>().SetFloatUniform("rate", force_bar_rate);
        }
        if (force_wave_second != null)
        {
            force_wave_second.GetComponent<Material>().SetFloatUniform("t", Time.totalTime);
            force_wave_second.GetComponent<Material>().SetFloatUniform("rate", force_bar_rate);
        }
        if (force_wave_third != null)
        {
            force_wave_third.GetComponent<Material>().SetFloatUniform("t", Time.totalTime);
            force_wave_third.GetComponent<Material>().SetFloatUniform("rate", force_bar_rate);
        }

        if (force_bar != null)
        {
            force_bar.GetComponent<Material>().SetFloatUniform("t", Time.totalTime);
            force_bar.GetComponent<Material>().SetFloatUniform("rate", force_bar_rate);
        }
        if (hp_bar != null)
        {
            hp_bar.GetComponent<Material>().SetFloatUniform("t", pulsation_rate);

        }
        //hp_glitch.Enable(false);
        if (hp_descending && hp_glitch != null)
        {
            hp_glitch.Enable(true);
        }
    }

    public void AddToCombo(float comboUnitsToAdd, float weaponDecreaseTimeMultiplier)
    {
        if (comboNumber == 0)
            ++comboNumber;

        lastWeaponDecrementMultiplier = weaponDecreaseTimeMultiplier;
        currComboTime += comboUnitsToAdd * (1 / (lastWeaponDecrementMultiplier * comboDecrementMultiplier));

        if (currComboTime > fullComboTime)
        {
            float extraCombo = currComboTime - fullComboTime;
            IncrementCombo();
            AddToCombo(extraCombo, weaponDecreaseTimeMultiplier);

            OnLvlUpComboChange();
        }

        if (comboNumber > 0 && !combo_gameobject.IsEnabled())
        {
            combo_gameobject.Enable(true);
        }

        if (combo_text == null)
            return;

        combo_text.GetComponent<Text>().text = "x" + comboNumber.ToString();

        if (combo_bar == null)
            return;

        combo_bar.GetComponent<Material>().SetIntUniform("combo_number", comboNumber);
        combo_bar.GetComponent<Material>().SetFloatUniform("length_used", Mathf.InvLerp(0, fullComboTime, currComboTime));

    }

    void IncrementCombo()
    {
        //TODO make full combo time not a linear function
        ++comboNumber;
        currComboTime = 0.0f;
        comboDecrementMultiplier += 0.2f;
    }

    bool UpdateCombo()
    {
        float toSubstract = currComboTime - Mathf.Lerp(currComboTime, -0.0f, Time.deltaTime * comboDecrementMultiplier * lastWeaponDecrementMultiplier);

        toSubstract = Math.Max(toSubstract, Time.deltaTime * comboDecrementMultiplier * lastWeaponDecrementMultiplier);
        toSubstract = Math.Min(toSubstract, Time.deltaTime * (1 / comboDecrementMultiplier) * fullComboTime * 0.25f);
        currComboTime -= toSubstract;

        if (currComboTime <= 0.0f)
        {
            currComboTime = 0.0f;
            ResetCombo(true);
            return false;
        }
        Material mat = combo_bar.GetComponent<Material>();
        if (mat != null)
            mat.SetFloatUniform("length_used", Mathf.InvLerp(0, fullComboTime, currComboTime));
        return true;
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


        Material comboMat = combo_bar.GetComponent<Material>();
        if (comboMat != null)
        {
            comboMat.SetIntUniform("combo_number", comboNumber);
            combo_bar.GetComponent<Material>().SetFloatUniform("length_used", Mathf.InvLerp(0, fullComboTime, currComboTime));
        }

        if (combo_text == null || combo_text.GetComponent<Text>()==null)
            return;

        combo_text.GetComponent<Text>().text = "x" + comboNumber.ToString();

        if (combo_gameobject != null)
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

        UpdateForce((int)(force + (max_force * lvlUpComboData.forceBarPercentageRecovery)), max_force); //TODO check this works fine (at the time of creating this line force doesn't work and this part of the method cannot be tested)


        Text t = combo_text.GetComponent<Text>();
        if (t != null)
        {
            t.color_red = lvlUpComboData.colorUpdate.x;
            t.color_green = lvlUpComboData.colorUpdate.y;
            t.color_blue = lvlUpComboData.colorUpdate.z;
        }
        if (combo_bar != null)
        {
            combo_bar.GetComponent<Material>().SetFloatUniform("r", lvlUpComboData.colorUpdate.x);
            combo_bar.GetComponent<Material>().SetFloatUniform("g", lvlUpComboData.colorUpdate.y);
            combo_bar.GetComponent<Material>().SetFloatUniform("b", lvlUpComboData.colorUpdate.z);
        }
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
        //hp_glitch.GetComponent<Material>().SetFloatUniform("length_used", hp_float);
    }

    public void UpdateForce(int new_force, int max_force)
    {
        if (force_bar == null || force_wave == null /*|| force_wave_second == null || force_wave_third == null*/)
            return;
        float force_float = new_force;
        force_float /= max_force;
        //force_float = Math.Max(force_float, 1.0f); CANNOT DO THIS! This returns 1.0 and it's not a wanted value
        force_bar.GetComponent<Material>().SetFloatUniform("length_used", force_float);
        force_wave.GetComponent<Material>().SetFloatUniform("length_used", force_float);
        //force_wave_second.GetComponent<Material>().SetFloatUniform("length_used", force_float);
        //force_wave_third.GetComponent<Material>().SetFloatUniform("length_used", force_float);
    }

    public void ChangeAlphaSkillPush(bool alpha_full)
    {
        if (skill_push == null)
            return;
        if (alpha_full)
            skill_push.GetComponent<Material>().SetFloatUniform("alpha", 1.0f);
        else
            skill_push.GetComponent<Material>().SetFloatUniform("alpha", 0.5f);
    }

    public void SwapWeapons()
    {
        if (primary_weapon != null && secondary_weapon != null)
            primary_weapon.GetComponent<Image2D>().SwapTwoImages(secondary_weapon);
        main_weapon = !main_weapon;
        if (weapon_bar == null)
            return;
        if (main_weapon)
        {
            float bullets_float = bullets_main_weapon;
            bullets_float /= max_bullets_main_weapon;
            weapon_bar.GetComponent<Material>().SetFloatUniform("length_used", bullets_float);
        }
        else
        {
            float bullets_float = bullets_secondary_weapon;
            bullets_float /= max_bullets_secondary_weapon;
            weapon_bar.GetComponent<Material>().SetFloatUniform("length_used", bullets_float);
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

}