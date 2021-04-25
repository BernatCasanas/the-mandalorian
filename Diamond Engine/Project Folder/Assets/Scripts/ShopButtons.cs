using System;
using DiamondEngine;
using System.Collections.Generic;

public class ShopButtons : DiamondComponent
{
    public GameObject shopController;
    public GameObject nameTxt;
    public GameObject descTxt;
    public GameObject priceTxt;
    public ShopItems itemType;
    public ShopPrice price_type;    
    public bool closeBtn;

    bool bought = false;
    SHOP shopScript;

    public void Awake()
    {
        shopScript = shopController.GetComponent<SHOP>();
    }

    public void SetItem(ShopItems type, ShopPrice price, string name, string description)
    {
        itemType = type;
        price_type = price;
        if (nameTxt != null) nameTxt.GetComponent<Text>().text = name;
        if (descTxt != null) descTxt.GetComponent<Text>().text = description;
        if (priceTxt != null) priceTxt.GetComponent<Text>().text = ((int)price).ToString();
    }

    public void OnExecuteButton()
    {
        /*if (shopScript.opening)
        {
            shopScript.opening = false;
        }
        else */if(!bought)
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
                    if (nameTxt != null) nameTxt.GetComponent<Text>().text = "-";
                    if (descTxt != null) descTxt.GetComponent<Text>().text = "-";
                    if (priceTxt != null) priceTxt.GetComponent<Text>().text = "-";
                    //gameObject.Enable(false);
                }
            }
        }
    }
}