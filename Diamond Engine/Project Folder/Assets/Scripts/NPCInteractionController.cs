using System;
using DiamondEngine;

public class NPCInteractionController : DiamondComponent
{
    public float maxDistance = 5;
    public GameObject colliderPosition = null;
    public GameObject displayText = null;
    public GameObject selectButton = null;
    public GameObject hubTextController = null;

    public void Update()
    {
        if (displayText == null || hubTextController == null)
            return;

        if (IsInside() && displayText.IsEnabled() == false)
        {
            displayText.Enable(true);

            if (hubTextController != null)
                hubTextController.GetComponent<HubTextController>().insideColliderTextActive = true;

            if (selectButton != null)
            {
                Navigation navComponent = selectButton.GetComponent<Navigation>();

                if (navComponent != null)
                    navComponent.Select();
            }
        }
        else if (!IsInside() && displayText.IsEnabled())
        {
            displayText.Enable(false);
            if (hubTextController != null)
                hubTextController.GetComponent<HubTextController>().insideColliderTextActive = false;
        }
    }

    public bool IsInside()
    {
        if (Core.instance == null || colliderPosition == null)
            return false;

        Vector3 playerPos = Core.instance.gameObject.transform.globalPosition;
        Vector3 colliderPos = colliderPosition.transform.globalPosition;
        double distance = playerPos.DistanceNoSqrt(colliderPos);

        if (distance >= -maxDistance && distance <= maxDistance)
            return true;
        else
            return false;
    }

}