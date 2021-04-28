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
    ShIt_MASTERYODAASSITANCE,
    ShIt_GREEFPAYCHECK,
    ShIt_BOUNTYHUNTERSKILLS,
    ShIt_ANAKINKILLSTREAK,
    ShIt_MAX
}

public enum ShopPrice
{
    SHOP_FREE = 0,
    SHOP_HEALTH = 75,
    SHOP_CHEAP = 150,
    SHOP_AVERAGE = 230,
    SHOP_EXPENSIVE = 310
}

public class SHOP : DiamondComponent
{
    public GameObject shopUI;
    public GameObject hud;
    public GameObject textPopUp;
    public GameObject item1;
    public GameObject item2;
    public GameObject item3;
    public GameObject item4;
    public GameObject currency = null;
    public float interactionRange = 2.0f;
    public bool autoGenerateItems = true;
    public bool opening;
    public GameObject defaultButton = null;

    bool shopOpen = false;

    public void Awake()
    {
        DebugOptionsHolder.goToNextLevel = false;
        shopOpen = false;
        opening = false;
        if (autoGenerateItems) 
            RandomiseItems();
        //Audio.PlayAudio(gameObject, "Play_Post_Boss_Room_1_Ambience");
    }

    public void Update()
    {
        if (shopOpen)
        {
            if (Input.GetGamepadButton(DEControllerButton.B) == KeyState.KEY_DOWN)
            {
                CloseShop();
                if (currency != null)
                    currency.SetParent(hud);
            }           
        }
        else 
        {
            if (InRange(Core.instance.gameObject.transform.globalPosition, interactionRange))
            {
                if (!textPopUp.IsEnabled()) textPopUp.Enable(true);

                if (Input.GetGamepadButton(DEControllerButton.A) == KeyState.KEY_DOWN)
                {
                    OpenShop();
                    currency.SetParent(shopUI.parent);
                    shopUI.SetParent(shopUI.parent);
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
                        Core.instance.gameObject.GetComponent<Core>().movementSpeed += Core.instance.gameObject.GetComponent<Core>().movementSpeed * 0.1f;
                        currency -= (int)item.price_type;
                        PlayerResources.AddBoon(BOONS.BOON_CADBANEROCKETBOOTS);
                        ret = true;
                    }
                    break;
                case ShopItems.ShIt_WATTOSCOOLANT:
                    if (currency >= (int)item.price_type)
                    {
                        Debug.Log("Bought Watto's Coolant");
                        Core.instance.gameObject.GetComponent<Core>().dashCD -= Core.instance.gameObject.GetComponent<Core>().dashCD * 0.2f;
                        currency -= (int)item.price_type;
                        PlayerResources.AddBoon(BOONS.BOON_WATTOSCOOLANT);
                        ret = true;
                    }
                    break;
                case ShopItems.ShIt_WRECKERRESILIENCE:
                    if (currency >= (int)item.price_type)
                    {
                        Debug.Log("Bought Wrecker’s resilience");
                        Core.instance.gameObject.GetComponent<PlayerHealth>().IncrementMaxHpPercent(0.2f);
                        currency -= (int)item.price_type;
                        PlayerResources.AddBoon(BOONS.BOON_WRECKERRESILIENCE);
                        ret = true;
                    }
                    break;
                case ShopItems.ShIt_IMPERIALREFINEDCOOLANT:
                    if (currency >= (int)item.price_type)
                    {
                        Debug.Log("Bought Imperial’s refined coolant");
                        Core.instance.gameObject.GetComponent<Core>().dashCD += Core.instance.gameObject.GetComponent<Core>().dashCD * 0.5f;
                        Core.instance.gameObject.GetComponent<Core>().movementSpeed += Core.instance.gameObject.GetComponent<Core>().movementSpeed * 0.4f;
                        currency -= (int)item.price_type;
                        PlayerResources.AddBoon(BOONS.BOON_IMPERIALREFINEDCOOLANT);
                        ret = true;
                    }
                    break;
                case ShopItems.ShIt_GREEDOQUICKSHOOTER:
                    if (currency >= (int)item.price_type)
                    {
                        Debug.Log("Bought Greedo’s quick shooting");
                        Core.instance.gameObject.GetComponent<Core>().baseFireRate -= Core.instance.gameObject.GetComponent<Core>().baseFireRate * 0.3f;
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
                case ShopItems.ShIt_MASTERYODAASSITANCE:
                    if (currency >= (int)item.price_type)
                    {
                        Debug.Log("Bought Master’s yoda assistance");
                        currency -= (int)item.price_type;
                        PlayerResources.AddBoon(BOONS.BOON_MASTERYODAASSITANCE);
                        ret = true;
                    }
                    break;
                case ShopItems.ShIt_GREEFPAYCHECK:
                    if (currency >= (int)item.price_type)
                    {
                        Debug.Log("Bought Greef’s paycheck");
                        currency += 50;
                        PlayerResources.AddBoon(BOONS.BOON_GREEFPAYCHECK);
                        ret = true;
                    }
                    break;
                case ShopItems.ShIt_BOUNTYHUNTERSKILLS:
                    if (currency >= (int)item.price_type)
                    {
                        Debug.Log("Bought Bounty’s hunter skills");
                        currency -= (int)item.price_type;
                        PlayerResources.AddBoon(BOONS.BOON_BOUNTYHUNTERSKILLS);
                        ret = true;
                    }
                    break;
                case ShopItems.ShIt_ANAKINKILLSTREAK:
                    if (currency >= (int)item.price_type)
                    {
                        Debug.Log("Bought Anakin’s kill streak");
                        currency -= (int)item.price_type;
                        PlayerResources.AddBoon(BOONS.BOON_ANAKINKILLSTREAK);
                        ret = true;
                    }
                    break;
                case ShopItems.ShIt_HEALTHREPLENISHMENT:
                    if (currency >= (int)item.price_type)
                    {
                        Debug.Log("Bought Health replenishment");
                        Core.instance.gameObject.GetComponent<PlayerHealth>().HealPercentMax(0.25f);
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
        List<int> available = new List<int>();
        for (int i = 0; i < (int)ShopItems.ShIt_MAX;i++)
        {
            if (!CheckGotBoon((ShopItems)i)) available.Add(i);
        }

        var rand = new Random();
        int item;


        if (item1 != null)
        {
            if (available.Count > 0)
            {
                item = rand.Next(0, available.Count);
                SetShopItem(item1.GetComponent<ShopButtons>(), (ShopItems)item);
                available.RemoveAt(item);
            }
            else
            {
                SetShopItem(item1.GetComponent<ShopButtons>(), ShopItems.ShIt_HEALTHREPLENISHMENT);
            }
        }

        if (item2 != null)
        {
            if (available.Count > 0)
            {
                item = rand.Next(0, available.Count);
                SetShopItem(item2.GetComponent<ShopButtons>(), (ShopItems)item);
                available.RemoveAt(item);
            }
            else
            {
                SetShopItem(item2.GetComponent<ShopButtons>(), ShopItems.ShIt_HEALTHREPLENISHMENT);
            }
        }

        if (item3 != null)
        {
            if (available.Count > 0)
            {
                item = rand.Next(0, available.Count);
                SetShopItem(item3.GetComponent<ShopButtons>(), (ShopItems)item);
                available.RemoveAt(item);
            }
            else
            {
                SetShopItem(item3.GetComponent<ShopButtons>(), ShopItems.ShIt_HEALTHREPLENISHMENT);
            }
        }

        if (item4 != null)
        {
            if (available.Count > 0)
            {
                item = rand.Next(0, available.Count);
                SetShopItem(item4.GetComponent<ShopButtons>(), (ShopItems)item);
                available.RemoveAt(item);
            }
            else
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
            case ShopItems.ShIt_MASTERYODAASSITANCE:
                if (PlayerResources.CheckBoon(BOONS.BOON_MASTERYODAASSITANCE)) return true;
                else return false;
            case ShopItems.ShIt_GREEFPAYCHECK:
                if (PlayerResources.CheckBoon(BOONS.BOON_GREEFPAYCHECK)) return true;
                else return false;
            case ShopItems.ShIt_BOUNTYHUNTERSKILLS:
                if (PlayerResources.CheckBoon(BOONS.BOON_BOUNTYHUNTERSKILLS)) return true;
                else return false;
            case ShopItems.ShIt_ANAKINKILLSTREAK:
                if (PlayerResources.CheckBoon(BOONS.BOON_ANAKINKILLSTREAK)) return true;
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
                item.SetItem(type, ShopPrice.SHOP_AVERAGE, "Bossk’s strength", "-10% damage received.");
                break;
            case ShopItems.ShIt_MASTERYODAASSITANCE:
                item.SetItem(type, ShopPrice.SHOP_EXPENSIVE, "Master Yoda’s assistance", "Every kill counts +3 to combo.");
                break;
            case ShopItems.ShIt_GREEFPAYCHECK:
                item.SetItem(type, ShopPrice.SHOP_FREE, "Greef’s paycheck", "+50 coins.");
                break;
            case ShopItems.ShIt_BOUNTYHUNTERSKILLS:
                item.SetItem(type, ShopPrice.SHOP_AVERAGE, "Bounty Hunter skills", "+2 gold when room is cleared.");
                break;
            case ShopItems.ShIt_ANAKINKILLSTREAK:
                item.SetItem(type, ShopPrice.SHOP_AVERAGE, "Anakin’s kill streak", "+20% movement speed if combo is 50 or more.");
                break;
        }
    }

    public void OpenShop()
    {
        shopOpen = true;
        opening = true;
        shopUI.Enable(true);
        textPopUp.Enable(false);
        Core.instance.lockInputs = true;
        if (defaultButton != null)
            defaultButton.GetComponent<Navigation>().Select();
    }

    public void CloseShop()
    {
        shopOpen = false;
        shopUI.Enable(false);
        Core.instance.lockInputs = false;
        textPopUp.Enable(true);
    }
}