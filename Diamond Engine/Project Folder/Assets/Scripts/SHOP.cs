using System;
using DiamondEngine;
using System.Collections.Generic;

public class SHOP : DiamondComponent
{
    public GameObject player;
    public GameObject shopUI;
    public GameObject hud;
    public float interactionRange = 2.0f;
    
    public void Update()
    {
        if (!shopUI.IsEnabled() && Input.GetGamepadButton(DEControllerButton.A) == KeyState.KEY_DOWN)
        { 
            if (InRange(player.transform.globalPosition, interactionRange))
            {
                shopUI.Enable(true);
                Time.PauseGame();
            }
        }

    }

    public bool InRange(Vector3 point, float givenRange)
    {
        return Mathf.Distance(gameObject.transform.globalPosition, point) < givenRange;
    }

    public void Buy(int item)
    {
        int cost = -1;
        int currency = hud.GetComponent<HUD>().currency;
        switch (item)
        {
            case 0:
                if(currency >= 150)
                {
                    Debug.Log("Bought Item 1");
                    cost = currency - 150;
                }
                break;
            case 1:
                if (currency >= 200)
                {
                    Debug.Log("Bought Item 2");
                    cost = currency - 200;
                }
                break;
            case 2:
                if (currency >= 250)
                {
                    Debug.Log("Bought Item 3");
                    cost = currency - 250;
                }
                break;
            case 3:
                if (currency >= 75)
                {
                    Debug.Log("Bought Health");
                    cost = currency - 75;
                }
                break;
        }

        if (cost == -1) Debug.Log("Not enough money");
        else
        {
            hud.GetComponent<HUD>().UpdateCurrency(cost);
        }

    }

    public void OnExecuteButton()
    {
        if (gameObject.Name == "Button1")
        {
            Buy(0);
        }
        else if (gameObject.Name == "Button2")
        {
            Buy(1);
        }
        else if (gameObject.Name == "Button3")
        {
            Buy(2);
        }
        else if (gameObject.Name == "ButtonHealth")
        {
            Buy(3);
        }
        else if (gameObject.Name == "ButtonBack")
        {
            shopUI.Enable(!shopUI.IsEnabled());
            Time.ResumeGame();
        }
    }

}