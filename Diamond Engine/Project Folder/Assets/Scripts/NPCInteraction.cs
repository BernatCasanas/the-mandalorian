using System;
using DiamondEngine;



public class NPCInteraction : DiamondComponent
{


    public float maxDistance = 5;
    public GameObject interactionImage = null;
    public GameObject notificationImage = null;
    public GameObject hubTextController = null;

    private Interaction npc = Interaction.GREEF;
    public bool canInteract = false;
    public bool canUpgrade = false;

    
    public void Awake()
    {
        HubTextController hubScript = hubTextController.GetComponent<HubTextController>();
        switch (npc)
        {
            case Interaction.BO_KATAN:
                canInteract = hubScript.BoKatanHasInteractions();
                break;
            case Interaction.GREEF:
                canInteract = hubScript.GreefHasInteractions();
                break;
            case Interaction.ASHOKA:
                canInteract = hubScript.AshokaHasInteractions();
                break;
            case Interaction.CARA_DUNE:
                canInteract = hubScript.CaraDuneHasInteractions();
                break;
            case Interaction.GROGU:
                canInteract = hubScript.GroguHasInteractions();
                break;
        }

    }

    public void Update()
    {
        InteractionImage();
        NotificationImage();
    }

    private void InteractionImage()
    {
        if (interactionImage == null)
            return;

        if (IsInside())
        {
            if (canInteract)
            {
                interactionImage.Enable(true);

                if (hubTextController != null)
                    hubTextController.GetComponent<HubTextController>().insideColliderTextActive = true;
            }
            else
            {
                interactionImage.Enable(false);
                if (hubTextController != null)
                    hubTextController.GetComponent<HubTextController>().insideColliderTextActive = false;
            }
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

        if (canUpgrade && !notificationImage.IsEnabled())
        {
            notificationImage.Enable(true);
        }

        if (IsInside())
        {
            if (hubTextController != null)
                hubTextController.GetComponent<HubTextController>().insideColliderTextActive = true;
        }
        else
        {
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

    public void SetEnum(Interaction interaction)
    {
        npc = interaction;
    }
}