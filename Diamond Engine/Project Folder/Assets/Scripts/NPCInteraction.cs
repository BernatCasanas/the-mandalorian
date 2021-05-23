using System;
using DiamondEngine;

public class NPCInteraction : DiamondComponent
{
    public enum NPC
    {
        GREEF,
        BOKATAN,
    }

    public float maxDistance = 5;
    public GameObject interactionImage = null;
    public GameObject notificationImage = null;
    public GameObject hubTextController = null;

    private NPC npc;
    public bool canInteract = false;

    public void Awake()
    {
        SetNPC();

        HubTextController hubScript = hubTextController.GetComponent<HubTextController>();
        switch (npc)
        {
            case NPC.GREEF:
                canInteract = hubScript.GreefHasInteractions();
                break;
        }
    }

    public void Update()
    {
        InteractionImage();
    }

    private void InteractionImage()
    {
        if (interactionImage == null || !canInteract)
            return;

        if (IsInside() && interactionImage.IsEnabled() == false)
        {
            interactionImage.Enable(true);

            if (hubTextController != null)
                hubTextController.GetComponent<HubTextController>().insideColliderTextActive = true;

        }
        else if (!IsInside() && interactionImage.IsEnabled())
        {
            interactionImage.Enable(false);
            if (hubTextController != null)
                hubTextController.GetComponent<HubTextController>().insideColliderTextActive = false;
        }
    }  
    private void NotificationImage()
    {
        if (notificationImage == null)
            return;

        if (IsInside() && notificationImage.IsEnabled() == false)
        {
            notificationImage.Enable(true);

            if (hubTextController != null)
                hubTextController.GetComponent<HubTextController>().insideColliderTextActive = true;

        }
        else if (!IsInside() && notificationImage.IsEnabled())
        {
            notificationImage.Enable(false);
            if (hubTextController != null)
                hubTextController.GetComponent<HubTextController>().insideColliderTextActive = false;
        }
    }

    public bool IsInside()
    {
        if (Core.instance == null)
            return false;

        Vector3 playerPos = Core.instance.gameObject.transform.globalPosition;
        Vector3 colliderPos = gameObject.transform.globalPosition;
        double distance = playerPos.DistanceNoSqrt(colliderPos);

        if (distance >= -maxDistance && distance <= maxDistance)
            return true;
        else
            return false;
    }

    private void SetNPC()
    {
        if (gameObject.CompareTag("Greef"))
        {
            npc = NPC.GREEF;
        }
        else if (gameObject.CompareTag("BoKatan"))
        {
            npc = NPC.BOKATAN;
        }
    }
}