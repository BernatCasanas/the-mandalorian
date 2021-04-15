using System;
using DiamondEngine;
using System.Collections.Generic;

public enum ShopItems
{
    ShIt_WATTOSCOOLANT = 0,
    ShIt_CADBANEROCKETBOOTS,
    ShIt_IMPERIALREFINEDCOOLANT,
    ShIt_WRECKERRESILIENCE,
    ShIt_HEALTHREPLENISHMENT
}

public class SHOP : DiamondComponent
{
    public GameObject player;
    public GameObject shopUI;
    public GameObject hud;
    public GameObject textPopUp;
    public float interactionRange = 2.0f;

    //public bool firstClick = true;

    float playerSpeed;
    bool shopOpen = false;
    
    public void Update()
    {
        if (shopOpen)
        {
            if (Input.GetGamepadButton(DEControllerButton.B) == KeyState.KEY_DOWN)
            {
                CloseShop();
            }

            if (Input.GetKey(DEKeyCode.Alpha7) == KeyState.KEY_DOWN)
            {
                UpdateCurrency(1000);
            }
        }
        else 
        {
            if (InRange(player.transform.globalPosition, interactionRange))
            {
                if (!textPopUp.IsEnabled()) textPopUp.Enable(true);

                if (Input.GetGamepadButton(DEControllerButton.A) == KeyState.KEY_DOWN)
                {
                    OpenShop();
                }
            }
            else if (textPopUp.IsEnabled())
            {
                textPopUp.Enable(false);
            }
        }        
    }

    public bool InRange(Vector3 point, float givenRange)
    {
        return Mathf.Distance(gameObject.transform.globalPosition, point) < givenRange;
    }

    public void Buy(int item)
    {
        //int cost = -1;
        if (shopOpen)
        {
            int currency = hud.GetComponent<HUD>().currency;
            switch (item)
            {
                case 0:
                    if (currency >= 150)
                    {
                        Debug.Log("Bought Cad Bane’s rocket boots");
                        playerSpeed += playerSpeed * 0.1f;
                        currency -= 150;
                    }
                    break;
                case 1:
                    if (currency >= 230)
                    {
                        Debug.Log("Bought Watto's Coolant");
                        player.GetComponent<Core>().dashCD -= player.GetComponent<Core>().dashCD * 0.2f;
                        currency -= 230;
                    }
                    break;
                case 2:
                    if (currency >= 310)
                    {
                        Debug.Log("Bought Wrecker’s resilience");
                        player.GetComponent<PlayerHealth>().IncrementMaxHpPercent(0.2f);
                        currency -= 310;
                    }
                    break;
                case 3:
                    if (currency >= 75)
                    {
                        Debug.Log("Bought Health replenishment");
                        player.GetComponent<PlayerHealth>().HealPercent(0.25f);
                        currency -= 75;
                    }
                    break;
            }
            UpdateCurrency(currency);
        }
        /*if (cost == -1) Debug.Log("Not enough money");
        else
        {
            
        }*/
    }

    private void UpdateCurrency(int val)
    {
        hud.GetComponent<HUD>().currency = val;
        hud.GetComponent<HUD>().UpdateCurrency(val);
    }

    public void OpenShop()
    {
        shopOpen = true;
        shopUI.Enable(true);
        textPopUp.Enable(false); 
        //Save current player speed and set it to 0 toa void movement while shop opened
        playerSpeed = player.GetComponent<Core>().movementSpeed;
        player.GetComponent<Core>().movementSpeed = 0;
        //Time.PauseGame();
    }

    public void CloseShop()
    {
        shopOpen = false;
        shopUI.Enable(false);
        //Time.ResumeGame();
        //Get player's speed back
        player.GetComponent<Core>().movementSpeed = playerSpeed;
        textPopUp.Enable(true);
    }
}