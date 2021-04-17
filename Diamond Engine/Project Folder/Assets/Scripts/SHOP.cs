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
    ShIt_GREEDOQUICKSHOOTER,
    ShIt_BOSSKSTRENGTH,
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
            //Debug 
            if (Input.GetKey(DEKeyCode.Alpha7) == KeyState.KEY_DOWN)
            {
                PlayerResources.AddRunCoins(500);
                hud.GetComponent<HUD>().UpdateCurrency(PlayerResources.GetRunCoins());
            }
            //
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
            int currency = PlayerResources.GetRunCoins();
            switch (item.itemType)
            {
                case ShopItems.ShIt_CADBANEROCKETBOOTS:
                    if (currency >= (int)item.price_type)
                    {
                        Debug.Log("Bought Cad Bane’s rocket boots");
                        player.GetComponent<Core>().movementSpeed += player.GetComponent<Core>().movementSpeed * 0.1f;
                        currency -= (int)item.price_type;
                        PlayerResources.AddBoon(BOONS.BOON_CADBANEROCKETBOOTS);
                        ret = true;
                    }
                    break;
                case ShopItems.ShIt_WATTOSCOOLANT:
                    if (currency >= (int)item.price_type)
                    {
                        Debug.Log("Bought Watto's Coolant");
                        player.GetComponent<Core>().dashCD -= player.GetComponent<Core>().dashCD * 0.2f;
                        currency -= (int)item.price_type;
                        PlayerResources.AddBoon(BOONS.BOON_WATTOSCOOLANT);
                        ret = true;
                    }
                    break;
                case ShopItems.ShIt_WRECKERRESILIENCE:
                    if (currency >= (int)item.price_type)
                    {
                        Debug.Log("Bought Wrecker’s resilience");
                        player.GetComponent<PlayerHealth>().IncrementMaxHpPercent(0.2f);
                        currency -= (int)item.price_type;
                        PlayerResources.AddBoon(BOONS.BOON_WRECKERRESILIENCE);
                        ret = true;
                    }
                    break;
                case ShopItems.ShIt_IMPERIALREFINEDCOOLANT:
                    if (currency >= (int)item.price_type)
                    {
                        Debug.Log("Bought Imperial’s refined coolant");
                        player.GetComponent<Core>().dashCD += player.GetComponent<Core>().dashCD * 0.5f;
                        player.GetComponent<Core>().movementSpeed += player.GetComponent<Core>().movementSpeed * 0.4f;
                        currency -= (int)item.price_type;
                        PlayerResources.AddBoon(BOONS.BOON_IMPERIALREFINEDCOOLANT);
                        ret = true;
                    }
                    break;
                case ShopItems.ShIt_GREEDOQUICKSHOOTER:
                    if (currency >= (int)item.price_type)
                    {
                        Debug.Log("Bought Greedo’s quick shooting");
                        player.GetComponent<Core>().fireRate -= player.GetComponent<Core>().fireRate * 0.3f;
                        currency -= (int)item.price_type;
                        PlayerResources.AddBoon(BOONS.BOON_GREEDOQUICKSHOOTER);
                        ret = true;
                    }
                    break;
                case ShopItems.ShIt_BOSSKSTRENGTH:
                    if (currency >= (int)item.price_type)
                    {
                        Debug.Log("Bought Bossk’s strength");                      
                        currency -= (int)item.price_type;
                        PlayerResources.AddBoon(BOONS.BOON_BOSSKSTRENGTH);
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
        PlayerResources.SetRunCoins(val);
        hud.GetComponent<HUD>().UpdateCurrency(val);
    }

    public void RandomiseItems()
    {
        bool[] available = new bool[(int)ShopItems.ShIt_MAX];
        for (int i = 0; i < available.Length; i++) available[i] = true;
        var rand = new Random();
        int item;
        bool exit;
        int counter;

        if (item1 != null)
        {
            counter = 0;
            exit = false;
            do
            {
                item = rand.Next(0, (int)ShopItems.ShIt_MAX);
                //Check if item is not already in shop
                if (available[item])
                {
                    //Check if is a boon is not already obtained
                    if (!CheckGotBoon((ShopItems)item))
                    {
                        available[item] = false;
                        SetShopItem(item1.GetComponent<ShopButtons>(), (ShopItems)item);
                        exit = true;
                    }
                }
                counter++;
            } while (exit == false && counter < (int)ShopItems.ShIt_MAX);
            //We run out of possible boons, fill with another unending item
            if (counter >= (int)ShopItems.ShIt_MAX)
            {
                SetShopItem(item1.GetComponent<ShopButtons>(), ShopItems.ShIt_HEALTHREPLENISHMENT);
            }
        }

        if (item2 != null)
        {
            counter = 0;
            exit = false;
            do
            {
                item = rand.Next(0, (int)ShopItems.ShIt_MAX);
                if (available[item])
                {
                    if (!CheckGotBoon((ShopItems)item))
                    {
                        available[item] = false;
                        SetShopItem(item2.GetComponent<ShopButtons>(), (ShopItems)item);
                        exit = true;
                    }
                }
                counter++;
            } while (exit == false && counter < (int)ShopItems.ShIt_MAX);

            if (counter >= (int)ShopItems.ShIt_MAX)
            {
                SetShopItem(item2.GetComponent<ShopButtons>(), ShopItems.ShIt_HEALTHREPLENISHMENT);
            }
        }

        if (item3 != null)
        {
            counter = 0;
            exit = false;
            do
            {
                item = rand.Next(0, (int)ShopItems.ShIt_MAX);
                if (available[item])
                {
                    if (!CheckGotBoon((ShopItems)item))
                    {
                        available[item] = false;
                        SetShopItem(item3.GetComponent<ShopButtons>(), (ShopItems)item);
                        exit = true;
                    }
                }
                counter++;
            } while (exit == false && counter < (int)ShopItems.ShIt_MAX);

            if (counter >= (int)ShopItems.ShIt_MAX)
            {
                SetShopItem(item3.GetComponent<ShopButtons>(), ShopItems.ShIt_HEALTHREPLENISHMENT);
            }
        }

        if (item4 != null)
        {
            counter = 0;
            exit = false;
            do
            {
                item = rand.Next(0, (int)ShopItems.ShIt_MAX);
                if (available[item])
                {
                    if (!CheckGotBoon((ShopItems)item))
                    {
                        available[item] = false;
                        SetShopItem(item4.GetComponent<ShopButtons>(), (ShopItems)item);
                        exit = true;
                    }
                }
                counter++;
            } while (exit == false && counter < (int)ShopItems.ShIt_MAX);
            
            if (counter >= (int)ShopItems.ShIt_MAX)
            {
                SetShopItem(item4.GetComponent<ShopButtons>(), ShopItems.ShIt_HEALTHREPLENISHMENT);
            }
        }
    }

    private bool CheckGotBoon(ShopItems it)
    {
        switch (it)
        {
            case ShopItems.ShIt_CADBANEROCKETBOOTS:
                if (PlayerResources.CheckBoon(BOONS.BOON_CADBANEROCKETBOOTS)) return true;
                else return false;
            case ShopItems.ShIt_WATTOSCOOLANT:
                if (PlayerResources.CheckBoon(BOONS.BOON_WATTOSCOOLANT)) return true;
                else return false;
            case ShopItems.ShIt_WRECKERRESILIENCE:
                if (PlayerResources.CheckBoon(BOONS.BOON_WRECKERRESILIENCE)) return true;
                else return false;
            case ShopItems.ShIt_IMPERIALREFINEDCOOLANT:
                if (PlayerResources.CheckBoon(BOONS.BOON_IMPERIALREFINEDCOOLANT))return true;            
                else return false;
            case ShopItems.ShIt_GREEDOQUICKSHOOTER:
                if (PlayerResources.CheckBoon(BOONS.BOON_GREEDOQUICKSHOOTER)) return true;
                else return false;
            case ShopItems.ShIt_BOSSKSTRENGTH:
                if (PlayerResources.CheckBoon(BOONS.BOON_BOSSKSTRENGTH)) return true;
                else return false;
            default:
                return false;
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
                item.SetItem(type, ShopPrice.SHOP_HEALTH, "Health replenishment", "Heal for 25 % of your max life.");
                break;
            case ShopItems.ShIt_GREEDOQUICKSHOOTER:
                item.SetItem(type, ShopPrice.SHOP_EXPENSIVE, "Greedo’s quick shooter", "+30% fire rate on the primary weapon.");
                break;
            case ShopItems.ShIt_BOSSKSTRENGTH:
                item.SetItem(type, ShopPrice.SHOP_CHEAP, "Bossk’s strength", "-10% damage received.");
                break;
        }
    }

    public void OpenShop()
    {
        shopOpen = true;
        opening = true;
        shopUI.Enable(true);
        textPopUp.Enable(false); 
        player.GetComponent<Core>().lockInputs = true;
    }

    public void CloseShop()
    {
        shopOpen = false;
        shopUI.Enable(false);
        player.GetComponent<Core>().lockInputs = false;
        textPopUp.Enable(true);
    }
}