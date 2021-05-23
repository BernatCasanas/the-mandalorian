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
        canUpgrade = false;
        if (hubTextController == null)
        {
            return;
        }
        HubTextController hubScript = hubTextController.GetComponent<HubTextController>();
        if (hubScript == null)
        {
            return;
        }

        hubScript.onUpgrade += UpdateUpgrade;

        switch (npc)
        {
            case Interaction.BO_KATAN:
                canInteract = hubScript.BoKatanHasInteractions();
                canUpgrade = hubScript.BoKatanCanUpgrade();
                break;
            case Interaction.GREEF:
                canInteract = hubScript.GreefHasInteractions();
                canUpgrade = hubScript.GreefCanUpgrade();
                break;
            case Interaction.ASHOKA:
                canInteract = hubScript.AshokaHasInteractions();
                canUpgrade = hubScript.AshokaCanUpgrade();
                break;
            case Interaction.CARA_DUNE:
                canInteract = hubScript.CaraDuneHasInteractions();
                canUpgrade = hubScript.CaraDuneCanUpgrade();
                break;
            case Interaction.GROGU:
                canInteract = hubScript.GroguHasInteractions();
                canUpgrade = hubScript.GroguCanUpgrade();
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
            if (canInteract || canUpgrade)
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
        if (!canUpgrade && notificationImage.IsEnabled())
        {
            notificationImage.Enable(false);
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
    public void UpdateUpgrade()
    {
        HubTextController hubScript = hubTextController.GetComponent<HubTextController>();
        if (hubScript == null)
        {
            return;
        }

        switch (npc)
        {
            case Interaction.BO_KATAN:
                canUpgrade = hubScript.BoKatanCanUpgrade();
                break;
            case Interaction.GREEF:
                canUpgrade = hubScript.GreefCanUpgrade();
                break;
            case Interaction.ASHOKA:
                canUpgrade = hubScript.AshokaCanUpgrade();
                break;
            case Interaction.CARA_DUNE:
                canUpgrade = hubScript.CaraDuneCanUpgrade();
                break;
            case Interaction.GROGU:
                canUpgrade = hubScript.GroguCanUpgrade();
                break;
        }
    }
}