using System;
using DiamondEngine;
using System.Collections.Generic;

public class ShopButtons : DiamondComponent
{
    public GameObject shopController;

    bool bought = false;

    public void OnExecuteButton()
    {
        if(shopController.GetComponent<SHOP>().firstClick)
        {
            shopController.GetComponent<SHOP>().firstClick = false;
        }

        if (gameObject.Name == "Button1")
        {
            if (!bought)
            {
                shopController.GetComponent<SHOP>().Buy(0);
                bought = true;
            }
        }
        else if (gameObject.Name == "Button2")
        {
            if (!bought)
            {
                shopController.GetComponent<SHOP>().Buy(1);
                bought = true;
            }
        }
        else if (gameObject.Name == "Button3")
        {
            if (!bought)
            {
                shopController.GetComponent<SHOP>().Buy(2);
                bought = true;
            }
        }
        else if (gameObject.Name == "ButtonHealth")
        {
            if (!bought)
            {
                shopController.GetComponent<SHOP>().Buy(3);
                bought = true;
            }
        }
        else if (gameObject.Name == "ButtonBack")
        {
            shopController.GetComponent<SHOP>().CloseShop();
        }
    }
}