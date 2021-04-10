using System;
using DiamondEngine;

public class InsideCollider : DiamondComponent
{
	public GameObject colliderPosition;
	public GameObject player;
    public float maxDistance = 5;
    public GameObject displayText;

    public void Update()
	{
        if (IsInside() && displayText.IsEnabled() == false) 
            displayText.Enable(true);
        else if(!IsInside() && displayText.IsEnabled()) 
            displayText.Enable(false);
    }

    public bool IsInside()
    {
        Vector3 playerPos = player.transform.globalPosition;
        Vector3 colliderPos = colliderPosition.transform.globalPosition;
        double insideNum = Math.Pow(playerPos.x - colliderPos.x, 2) + Math.Pow(playerPos.y - colliderPos.y, 2) + Math.Pow(playerPos.z - colliderPos.z, 2);
        double distance = Math.Sqrt(insideNum);

        if (distance >= -maxDistance && distance <= maxDistance)        
            return true;    
        else
            return false;
    }
}