using System;
using DiamondEngine;
using System.Collections.Generic;

public class ShopButtons : DiamondComponent
{
    public GameObject shopController;
    public GameObject nameObj;
    public GameObject descriptionObj;
    public GameObject priceObj;

    private Text nameText;
    private Text descriptionText;
    private Text priceText;

    public ShopItems itemType;
    public ShopPrice price;
    public GameResources resource;
    public bool closeBtn;

    bool bought = false;
    SHOP shopScript;

    public void Init()
    {
        Debug.Log("Shop Button 1");
        shopScript = shopController.GetComponent<SHOP>();

        if(nameObj != null)
            nameText = nameObj.GetComponent<Text>();

        if(descriptionObj != null)
            descriptionText = descriptionObj.GetComponent<Text>();
        
        if(priceObj != null)
            priceText = priceObj.GetComponent<Text>();
    }

    public void SetItem(ShopItems type, ShopPrice shop_price, string name, string description)
    {
        itemType = type;
        price = shop_price;
        if (nameText != null) nameText.text = name;
        if (descriptionText != null) descriptionText.text = description;
        if (priceText != null) priceText.text = ((int)price).ToString();
    }

    public void OnExecuteButton()
    {
        if (!bought)
        {
            if (closeBtn)
            {
                shopScript.CloseShop();
            }
            else
            {
                if (shopScript.Buy(this))
                {
                    bought = true;
                    if (nameText != null) nameText.text = "-";
                    if (descriptionText != null) descriptionText.text = "-";
                    if (priceText != null) priceText.text = "-";
                    //gameObject.Enable(false);
                }
            }
        }
    }

    public void SetDefaultPrice()
    {
        if (descriptionText != null)
            descriptionText.text = ((int)price).ToString();
    }
}