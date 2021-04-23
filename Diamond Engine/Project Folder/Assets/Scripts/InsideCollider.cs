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
        if (Core.instance != null)
            player = Core.instance.gameObject;

        if (displayText == null)
            return;

        if (IsInside() && displayText.IsEnabled() == false) 
            displayText.Enable(true);
        else if(!IsInside() && displayText.IsEnabled()) 
            displayText.Enable(false);
    }

    public bool IsInside()
    {
        if (player == null || colliderPosition == null)
            return false;

        Vector3 playerPos = player.transform.globalPosition;
        Vector3 colliderPos = colliderPosition.transform.globalPosition;
        double distance = playerPos.DistanceNoSqrt(colliderPos);

        if (distance >= -maxDistance && distance <= maxDistance)        
            return true;    
        else
            return false;
    }
}