using System;
using DiamondEngine;
using System.Collections.Generic;

public enum ShopItems
{
    ShIt_WATTOSCOOLANT = 0,
    ShIt_CADBANEROCKETBOOTS,
    ShIt_IMPERIALREFINEDCOOLANT,
    ShIt_WRECKERRESILIENCE,
    ShIt_HEALTHREPLENISHMENT,
    ShIt_MAX
}

public enum ShopPrice
{
    SHOP_HEALTH = 75,
    SHOP_CHEAP = 150,
    SHOP_AVERAGE = 230,
    SHOP_EXPENSIVE = 310
}

public class SHOP : DiamondComponent
{
    public GameObject player;
    public GameObject shopUI;
    public GameObject hud;
    public GameObject textPopUp;
    public GameObject item1;
    public GameObject item2;
    public GameObject item3;
    public GameObject item4;
    public float interactionRange = 2.0f;
    public bool autoGenerateItems = true;
    public bool opening;
    //public bool firstClick = true;

    float playerSpeed;
    bool shopOpen = false;

    public void Awake()
    {
        if(autoGenerateItems) RandomiseItems();
    }

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

    public bool Buy(ShopButtons item)
    {
        bool ret = false;
        //int cost = -1;
        if (shopOpen)
        {
            int currency = hud.GetComponent<HUD>().currency;
            switch (item.itemType)
            {
                case ShopItems.ShIt_CADBANEROCKETBOOTS:
                    if (currency >= (int)item.price_type)
                    {
                        Debug.Log("Bought Cad Bane’s rocket boots");
                        playerSpeed += playerSpeed * 0.1f;
                        currency -= (int)item.price_type;
                        ret = true;
                    }
                    break;
                case ShopItems.ShIt_WATTOSCOOLANT:
                    if (currency >= (int)item.price_type)
                    {
                        Debug.Log("Bought Watto's Coolant");
                        player.GetComponent<Core>().dashCD -= player.GetComponent<Core>().dashCD * 0.2f;
                        currency -= (int)item.price_type;
                        ret = true;
                    }
                    break;
                case ShopItems.ShIt_WRECKERRESILIENCE:
                    if (currency >= (int)item.price_type)
                    {
                        Debug.Log("Bought Wrecker’s resilience");
                        player.GetComponent<PlayerHealth>().IncrementMaxHpPercent(0.2f);
                        currency -= (int)item.price_type;
                        ret = true;
                    }
                    break;
                case ShopItems.ShIt_IMPERIALREFINEDCOOLANT:
                    if (currency >= (int)item.price_type)
                    {
                        Debug.Log("Bought Imperial’s refined coolant");
                        player.GetComponent<Core>().dashCD += player.GetComponent<Core>().dashCD * 0.5f;
                        playerSpeed += playerSpeed * 0.4f;
                        currency -= (int)item.price_type;
                        ret = true;
                    }
                    break;
                case ShopItems.ShIt_HEALTHREPLENISHMENT:
                    if (currency >= (int)item.price_type)
                    {
                        Debug.Log("Bought Health replenishment");
                        player.GetComponent<PlayerHealth>().HealPercentMax(0.25f);
                        currency -= (int)item.price_type;
                        ret = true;
                    }
                    break;
            }
            UpdateCurrency(currency);
        }
        return ret;
    }

    private void UpdateCurrency(int val)
    {
        hud.GetComponent<HUD>().currency = val;
        hud.GetComponent<HUD>().UpdateCurrency(val);
    }

    public void RandomiseItems()
    {
        bool[] available = new bool[(int)ShopItems.ShIt_MAX];
        for (int i = 0; i < available.Length; i++) available[i] = true;
        var rand = new Random();
        int item;
        bool exit;

        if (item1 != null)
        {
            exit = false;
            do
            {
                item = rand.Next(0, (int)ShopItems.ShIt_MAX);
                if (available[item])
                {
                    available[item] = false;
                    SetShopItem(item1.GetComponent<ShopButtons>(), (ShopItems)item);

                    exit = true;
                }
            } while (exit == false);
        }

        if (item2 != null)
        {
            exit = false;
            do
            {
                item = rand.Next(0, (int)ShopItems.ShIt_MAX);
                if (available[item])
                {
                    available[item] = false;
                    SetShopItem(item2.GetComponent<ShopButtons>(), (ShopItems)item);

                    exit = true;
                }
            } while (exit == false);
        }

        if (item3 != null)
        {
            exit = false;
            do
            {
                item = rand.Next(0, (int)ShopItems.ShIt_MAX);
                if (available[item])
                {
                    available[item] = false;
                    SetShopItem(item3.GetComponent<ShopButtons>(), (ShopItems)item);

                    exit = true;
                }
            } while (exit == false);
        }

        if (item4 != null)
        {
            exit = false;
            do
            {
                item = rand.Next(0, (int)ShopItems.ShIt_MAX);
                if (available[item])
                {
                    available[item] = false;
                    SetShopItem(item4.GetComponent<ShopButtons>(), (ShopItems)item);

                    exit = true;
                }
            } while (exit == false);
        }
    }

    private void SetShopItem(ShopButtons item,ShopItems type)
    {
        switch (type)
        {
            case ShopItems.ShIt_CADBANEROCKETBOOTS:
                item.SetItem(type, ShopPrice.SHOP_CHEAP, "Cad Bane’s rocket boots", "+10% movement speed.");
                break;
            case ShopItems.ShIt_WATTOSCOOLANT:
                item.SetItem(type, ShopPrice.SHOP_CHEAP, "Watto’s coolant", "Dash cooldown -20%.");
                break;
            case ShopItems.ShIt_WRECKERRESILIENCE:
                item.SetItem(type, ShopPrice.SHOP_EXPENSIVE, "Wrecker’s resilience", "+20% max HP.");
                break;
            case ShopItems.ShIt_IMPERIALREFINEDCOOLANT:
                item.SetItem(type, ShopPrice.SHOP_CHEAP, "Imperial refined coolant", "Movement +40%, +50% dash cooldown.");
                break;
            case ShopItems.ShIt_HEALTHREPLENISHMENT:
                item.SetItem(type, ShopPrice.SHOP_HEALTH, "Health replenishment", "Heal for 25 % of your max life");
                break;
        }
    }

    public void OpenShop()
    {
        shopOpen = true;
        opening = true;
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