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

        if (Input.GetKey(DEKeyCode.E) == KeyState.KEY_DOWN)
        {
            if (InRange(player.transform.globalPosition, interactionRange))
            {
                shopUI.Enable(!shopUI.IsEnabled());
            }
        }
       
    }

    public bool InRange(Vector3 point, float givenRange)
    {
        return Mathf.Distance(gameObject.transform.globalPosition, point) < givenRange;
    }

    public void Buy(int item, int currency)
    {
        int cost = -1;

        switch (item)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }

        if (cost == -1) DiamondEngine.Debug.Log("Not enough money");
        else
        {
            currency -= cost;
            hud.GetComponent<HUD>().UpdateCurrency(currency);
        }

    }

}