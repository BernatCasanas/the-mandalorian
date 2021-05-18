using System;
using DiamondEngine;
using System.Collections.Generic;

public enum ShopItems
{
    SHOP_ITEM_BOON,
    SHOP_ITEM_HEALTHREPLENISHMENT,
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

    //Buttons
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
            }

            if (Input.GetKey(DEKeyCode.M) == KeyState.KEY_DOWN)
            {
                PlayerResources.AddRunCoins(100);
                hud.GetComponent<HUD>().UpdateCurrency(PlayerResources.GetRunCoins());
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

            if (currency >= (int)item.price_type)
            {
                if (item.itemType == ShopItems.SHOP_ITEM_BOON)
                {
                    //Add and apply boon
                }
                else if (item.itemType == ShopItems.SHOP_ITEM_HEALTHREPLENISHMENT)
                {
                    Core.instance.gameObject.GetComponent<PlayerHealth>().HealPercentMax(0.25f);
                }

                currency -= (int)item.price_type;
                ret = true;
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
        List<BOONS> available = new List<BOONS>();
        for (BOONS boon = 0; boon < BOONS.BOON_MAX; ++boon)  //Number of boons + health replenishments
        {
            if (!PlayerResources.CheckBoon(boon))
                available.Add(boon);
        }

        Random rand = new Random();
        int item;

        if (item1 != null)
        {
            if (available.Count > 0)
            {
                item = rand.Next(0, available.Count);
                SetShopItem(item1.GetComponent<ShopButtons>(), ShopItems.SHOP_ITEM_BOON, (BOONS)item);
                available.RemoveAt(item);
            }
            else
            {
                SetShopItem(item1.GetComponent<ShopButtons>(), ShopItems.SHOP_ITEM_HEALTHREPLENISHMENT, BOONS.BOON_MAX);
            }
        }

        if (item2 != null)
        {
            if (available.Count > 0)
            {
                item = rand.Next(0, available.Count);
                SetShopItem(item2.GetComponent<ShopButtons>(), ShopItems.SHOP_ITEM_BOON, (BOONS)item);
                available.RemoveAt(item);
            }
            else
            {
                SetShopItem(item2.GetComponent<ShopButtons>(), ShopItems.SHOP_ITEM_HEALTHREPLENISHMENT, BOONS.BOON_MAX);
            }
        }

        if (item3 != null)
        {
            if (available.Count > 0)
            {
                item = rand.Next(0, available.Count);
                SetShopItem(item3.GetComponent<ShopButtons>(), ShopItems.SHOP_ITEM_BOON, (BOONS)item);
                available.RemoveAt(item);
            }
            else
            {
                SetShopItem(item3.GetComponent<ShopButtons>(), ShopItems.SHOP_ITEM_HEALTHREPLENISHMENT, BOONS.BOON_MAX);
            }
        }

        if (item4 != null)
        {
            if (available.Count > 0)
            {
                item = rand.Next(0, available.Count);
                SetShopItem(item4.GetComponent<ShopButtons>(), ShopItems.SHOP_ITEM_BOON, (BOONS)item);
                available.RemoveAt(item);
            }
            else
            {
                SetShopItem(item4.GetComponent<ShopButtons>(), ShopItems.SHOP_ITEM_HEALTHREPLENISHMENT, BOONS.BOON_MAX);
            }
        }
    }

    private void SetShopItem(ShopButtons item, ShopItems type, BOONS boon)
    {
        if (type == ShopItems.SHOP_ITEM_BOON)
        {
            item.SetItem(type, ShopPrice.SHOP_CHEAP, BoonDataHolder.boonType[(int)boon].name, BoonDataHolder.boonType[(int)boon].rewardDescription);
        }
        else
        {
            item.SetItem(type, ShopPrice.SHOP_CHEAP, "Health Replenishment", "Heal for 25% of your max life");
        }
    }

    public void OpenShop()
    {
        shopOpen = true;
        opening = true;
        shopUI.EnableNav(true);
        textPopUp.Enable(false);
        Core.instance.LockInputs(true); ;
        if (defaultButton != null)
            defaultButton.GetComponent<Navigation>().Select();
    }

    public void CloseShop()
    {
        shopOpen = false;
        shopUI.EnableNav(false);
        Core.instance.LockInputs(false); ;
        textPopUp.Enable(true);
        if (currency != null)
            currency.SetParent(hud);
    }
}