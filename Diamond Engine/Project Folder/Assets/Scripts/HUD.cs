using System;
using DiamondEngine;
using System.Collections.Generic;

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
    private bool start = true;



    private float fullComboTime = 0.0f;
    private float currComboTime = 0.0f;
    public int comboNumber = 0;
    public float force_bar_rate = 0.0f;
    public float last_hp = 0;

    //stores the level as a key and the color as a value
    Dictionary<int, Vector3> comboLvlColors = new Dictionary<int, Vector3>
    {
        { 0, new Vector3(0.0f,0.8f,1.0f)},
        { 10, new Vector3(0.0f,1.0f,0.0f)},
        { 25, new Vector3(1.0f,1.0f,0.0f)},
        { 45, new Vector3(0.79f,0.28f,0.96f)},
        { 77, new Vector3(1.0f,1.0f,1.0f)},
            //TODO Add colors here
    };


    public void Update()
    {
        if (start)
        {
            UpdateHP(PlayerHealth.currHealth, PlayerHealth.currMaxHealth);
            last_hp = (float)PlayerHealth.currHealth;
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
                hp += 1;
                UpdateHP(hp, max_hp);
                last_hp = hp;
            }
        }
        if (Input.GetKey(DEKeyCode.D) == KeyState.KEY_DOWN)
        {
            if (hp > 0)
            {
                last_hp = hp;
                hp -= 1;
                UpdateHP(hp, max_hp);
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
            IncrementCombo(1, 1.0f);
        }
        if (Input.GetKey(DEKeyCode.N) == KeyState.KEY_DOWN) //test key
        {
            DecreaseComboPercentage(0.5f);
        }
        if (combo_bar != null && comboNumber > 0)
        {
            UpdateCombo();
        }
        if (last_hp > hp)
        {
            hp_bar.GetComponent<Material>().SetFloatUniform("last_hp", last_hp/max_hp);
            last_hp -= 0.025f;
        }
        force_bar.GetComponent<Material>().SetFloatUniform("t", Time.totalTime);
        force_wave.GetComponent<Material>().SetFloatUniform("t", Time.totalTime);
        force_bar.GetComponent<Material>().SetFloatUniform("rate", force_bar_rate);
        force_wave.GetComponent<Material>().SetFloatUniform("rate", force_bar_rate);
    }

    public void IncrementCombo(int levelsToAdd, float weaponTimeMultiplier)
    {
        comboNumber += levelsToAdd;
        //TODO make full combo time not a linear function
        currComboTime = fullComboTime = (100 - comboNumber) * weaponTimeMultiplier;

        if (comboNumber == 1)
        {
            combo_gameobject.Enable(true);
        }

        if (combo_bar != null)
        {
            combo_bar.GetComponent<Material>().SetIntUniform("combo_number", comboNumber);
            combo_bar.GetComponent<Material>().SetFloatUniform("length_used", Mathf.InvLerp(0, fullComboTime, currComboTime));
        }
        if (combo_text == null)
            return;
        combo_text.GetComponent<Text>().text = "x" + comboNumber.ToString();

        ChangeComboColor();
    }

    public bool UpdateCombo()
    {
        currComboTime -= Time.deltaTime * 100;
        if (currComboTime <= 0.0f)
        {
            currComboTime = 0.0f;
            ResetCombo();
            return false;
        }
        Material mat = combo_bar.GetComponent<Material>();
        if (mat != null)
            mat.SetFloatUniform("length_used", Mathf.InvLerp(0, fullComboTime, currComboTime));
        return true;
    }

    public void ResetCombo()
    {
        comboNumber = 0;
        Material comboMat = combo_bar.GetComponent<Material>();

        if (comboMat != null)
            comboMat.SetIntUniform("combo_number", comboNumber);

        if (combo_gameobject != null)
            combo_gameobject.Enable(false);
    }

    void ChangeComboColor()
    {
        Vector3 newColor = new Vector3(0.0f, 0.8f, 1.0f); //default color in case the dictionary is empty

        foreach (KeyValuePair<int, Vector3> lvlColor in comboLvlColors)
        {
            int key = lvlColor.Key;
            Vector3 value = new Vector3(lvlColor.Value.x, lvlColor.Value.y, lvlColor.Value.z);
            if (comboNumber >= key)
            {
                newColor = value;
            }
        }

        Text t = combo_text.GetComponent<Text>();
        if (t != null)
        {
            t.color_red = newColor.x;
            t.color_green = newColor.y;
            t.color_blue = newColor.z;
        }
    }

    //percentage between 0 and 1 (0%-100%) decreases the time left for the current combo to deplete
    public void DecreaseComboPercentage(float percentageOfTotal)
    {
        currComboTime -= fullComboTime * percentageOfTotal;

        if (currComboTime > fullComboTime)
            currComboTime = fullComboTime;
        else if (currComboTime < 0.0f)
            currComboTime = 0.0f;
    }

    public void UpdateHP(int new_hp, int max_hp)
    {
        if (hp_number_gameobject != null)
            hp_number_gameobject.GetComponent<Text>().text = new_hp.ToString();
        if (hp_bar == null)
            return;
        float hp_float = new_hp;
        hp_float /= max_hp;
        hp_bar.GetComponent<Material>().SetFloatUniform("length_used", hp_float);
        hp_bar.GetComponent<Material>().SetFloatUniform("last_hp", last_hp / max_hp);
    }

    public void UpdateForce(int new_force, int max_force)
    {
        if (force_bar == null)
            return;
        float force_float = new_force;
        force_float /= max_force;
        force_bar.GetComponent<Material>().SetFloatUniform("length_used", force_float);
        force_wave.GetComponent<Material>().SetFloatUniform("length_used", force_float);
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