using System;
using DiamondEngine;

public class DebugAddResources : DiamondComponent
{
    public void OnExecuteButton()
    {
        Debug.Log("Add resources");
        PlayerResources.AddRunCoins(500);
        Debug.Log("Add run coins");
        PlayerResources.AddResourceByValue(RewardType.REWARD_BESKAR,500);
        Debug.Log("Add beskar");
        PlayerResources.AddResourceByValue(RewardType.REWARD_MACARON, 500);
        Debug.Log("Add macaron");
        PlayerResources.AddResourceByValue(RewardType.REWARD_MILK, 500);
        Debug.Log("Add milk");
        PlayerResources.AddResourceByValue(RewardType.REWARD_SCRAP, 500);
        Debug.Log("Add scrap");
        if (Core.instance.hud != null)
        {
            Core.instance.hud.GetComponent<HUD>().UpdateCurrency(PlayerResources.GetRunCoins());
            Debug.Log("Update run coins");
        }
    }
}