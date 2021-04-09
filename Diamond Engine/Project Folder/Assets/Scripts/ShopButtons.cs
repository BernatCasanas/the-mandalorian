using System;
using DiamondEngine;
using System.Collections.Generic;

public class ShopButtons : DiamondComponent
{
    public GameObject shopController;

    public void OnExecuteButton()
    {
        if(shopController.GetComponent<SHOP>().firstClick)
        {
            shopController.GetComponent<SHOP>().firstClick = false;
        }

        if (gameObject.Name == "Button1")
        {
            shopController.GetComponent<SHOP>().Buy(0);
        }
        else if (gameObject.Name == "Button2")
        {
            shopController.GetComponent<SHOP>().Buy(1);
        }
        else if (gameObject.Name == "Button3")
        {
            shopController.GetComponent<SHOP>().Buy(2);
        }
        else if (gameObject.Name == "ButtonHealth")
        {
            shopController.GetComponent<SHOP>().Buy(3);
        }
        else if (gameObject.Name == "ButtonBack")
        {
            shopController.GetComponent<SHOP>().CloseShop();
        }
    }
}