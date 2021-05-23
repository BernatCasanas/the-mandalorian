﻿using System;
using DiamondEngine;
using System.Collections.Generic;

public class ShopButtons : DiamondComponent
{
    public GameObject shopController = null;
    public GameObject nameObj = null;
    public GameObject descriptionObj = null;
    public GameObject priceObj = null;

    private Text nameText = null;
    private Text descriptionText = null;
    private Text priceText = null;

    public ShopItems itemType;
    public ShopPrice price;
    public GameResources resource = null;
    public bool closeBtn = false;

    bool bought = false;
    SHOP shopScript = null;

    public void Awake()
    {
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
                if(shopScript != null)
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