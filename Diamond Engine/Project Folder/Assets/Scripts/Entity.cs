using System;
using DiamondEngine;
using System.Collections.Generic;


public enum ENTITY_TYPE
{
    NONE = -1,

    //Player
    PLAYER,

    //Enemies
    BANTHA,
    STROMTROOPER,
    SKYTROOPER,
    TURRET,
    DEATHTROOPER,
    HEAVYTROOPER,

    //Bosses
    RANCOR,
    WAMPA,
    SKEL,
    MOFF,

    MAX_ENTITIES

}

public enum STATUS_TYPE
{
    NONE = -1,

    SLOWED,
    ACCELERATED
}

public enum STATUS_APPLY_TYPE
{
    NONE = -1,
    BIGGER_PERCENTAGE,
    BIGGER_TIME,
    SUBSTITUTE,
    ADDITIVE,
}

#region StatusData
class StatusData
{
    //Constructors
    public StatusData()
    {
        statusType = STATUS_TYPE.NONE;
        applyType = STATUS_APPLY_TYPE.NONE;
        severity = 0f;
        statTaken = 0f;
        remainingTime = 0f;
        isPermanent = false;
    }
    public StatusData(STATUS_TYPE statType, STATUS_APPLY_TYPE statusApplyType, float statusSeverirt, float statusTime, bool isstatusPermanent, float statusStatTaken = 0f)
    {
        statusType = statType;
        applyType = statusApplyType;
        severity = statusSeverirt;
        remainingTime = statusTime;
        isPermanent = isstatusPermanent;
        statTaken = statusStatTaken;
    }

    public float severity;
    public float statTaken;
    public float remainingTime;
    public bool isPermanent;
    public STATUS_APPLY_TYPE applyType;
    public STATUS_TYPE statusType;
}

#endregion

public class Entity : DiamondComponent
{
    protected ENTITY_TYPE eType = ENTITY_TYPE.NONE;

    private Dictionary<STATUS_TYPE, StatusData> statuses = new Dictionary<STATUS_TYPE, StatusData>();
    private List<STATUS_TYPE> statusToDelete = new List<STATUS_TYPE>();

    protected float speedMult = 1f;
    protected float myDeltaTime = 1f;

    protected void InitEntity(ENTITY_TYPE myType)
    {
        eType = myType;
        speedMult = 1f;
        myDeltaTime = Time.deltaTime;
    }

    #region ENTITY TYPES


    public ENTITY_TYPE GetEntityType()
    {
        return eType;
    }

    public bool IsEnemy()
    {
        return eType == ENTITY_TYPE.BANTHA || eType == ENTITY_TYPE.STROMTROOPER || eType == ENTITY_TYPE.SKYTROOPER || eType == ENTITY_TYPE.TURRET || eType == ENTITY_TYPE.DEATHTROOPER || eType == ENTITY_TYPE.HEAVYTROOPER;
    }

    public bool IsBoss()
    {
        return eType == ENTITY_TYPE.RANCOR || eType == ENTITY_TYPE.WAMPA || eType == ENTITY_TYPE.SKEL || eType == ENTITY_TYPE.MOFF;
    }

    public bool IsPlayer()
    {
        return eType == ENTITY_TYPE.PLAYER;
    }

    #endregion


    #region STATUS PUBLIC CALLS
    public void AddStatus(STATUS_TYPE statusType, STATUS_APPLY_TYPE applyType, float percentage, float time, bool isPermanent = false)
    {
        if (CheckStatusApply(ref statusType, ref applyType, ref percentage, ref time, ref isPermanent) == false)
            return;

        statuses.Add(statusType, InitStatus(statusType, applyType, percentage, time, isPermanent));
    }

    public void RemoveStatus(STATUS_TYPE statusType, bool affectPermanents = false)
    {
        if (statuses.ContainsKey(statusType) == true)
        {
            if (statuses[statusType].isPermanent == false || affectPermanents == true)
            {
                DeleteStatus(statusType);
            }
        }

    }

    public void ClearStatuses(bool affectPermanents = false)
    {
        var mapKeys = statuses.Keys;
        foreach (STATUS_TYPE statusType in mapKeys)
        {
            if (statuses[statusType].isPermanent == false || affectPermanents == true)
            {
                statusToDelete.Add(statusType);
            }
        }

        for (int i = 0; i < statusToDelete.Count; i++)
        {
            RemoveStatus(statusToDelete[i]);
        }
        statusToDelete.Clear();
    }

    public bool HasStatus(STATUS_TYPE stat)
    {
        return statuses.ContainsKey(stat);
    }

    public bool HasAnyStatus()
    {
        return statuses.Count > 0;
    }

    #endregion


    private bool CheckStatusApply(ref STATUS_TYPE statusType, ref STATUS_APPLY_TYPE applyType, ref float percentage, ref float time, ref bool isPermanent)
    {
        bool ret = true;

        switch (applyType)
        {
            case STATUS_APPLY_TYPE.BIGGER_PERCENTAGE:
                {
                    if (statuses.ContainsKey(statusType) == true)
                    {
                        if (statuses[statusType].severity < percentage)
                        {
                            DeleteStatus(statusType);
                        }
                        else
                        {
                            ret = false;
                        }
                    }
                }
                break;
            case STATUS_APPLY_TYPE.BIGGER_TIME:
                {
                    if (statuses.ContainsKey(statusType) == true)
                    {
                        if (statuses[statusType].remainingTime < time)
                        {
                            DeleteStatus(statusType);
                        }
                        else
                        {
                            ret = false;
                        }
                    }
                }
                break;
            case STATUS_APPLY_TYPE.SUBSTITUTE:
                {
                    if (statuses.ContainsKey(statusType) == true)
                    {
                        DeleteStatus(statusType);
                    }
                }
                break;
            case STATUS_APPLY_TYPE.ADDITIVE:
                {
                    if (statuses.ContainsKey(statusType) == true)
                    {
                        percentage += statuses[statusType].severity;
                        time += statuses[statusType].remainingTime;

                        DeleteStatus(statusType);
                    }
                }
                break;
            default:
                {
                    Debug.Log("The status apply type is none!");
                }
                break;
        }


        return ret;
    }

    private StatusData InitStatus(STATUS_TYPE statusType, STATUS_APPLY_TYPE applyType, float percentage, float time, bool isPermanent)
    {
        StatusData statuserstatusData = new StatusData(statusType, applyType, percentage, time, isPermanent);

        OnInitStatus(ref statuserstatusData);

        return statuserstatusData;
    }

    private void OnInitStatus(ref StatusData statusToInit)
    {
        switch (statusToInit.statusType)
        {
            case STATUS_TYPE.SLOWED:
                {
                    this.speedMult -= statusToInit.severity;

                    if (speedMult < 0.1f)
                    {
                        statusToInit.severity = statusToInit.severity - (Math.Abs(this.speedMult) + 0.1f);

                        speedMult = 0.1f;
                    }

                    this.myDeltaTime = Time.deltaTime * speedMult;

                }
                break;
            case STATUS_TYPE.ACCELERATED:
                {
                    this.speedMult += statusToInit.severity;

                    this.myDeltaTime = Time.deltaTime * speedMult;
                }
                break;
            default:
                break;
        }
    }

    private void OnUpdateStatus(StatusData statusToUpdate)
    {
        switch (statusToUpdate.statusType)
        {
            case STATUS_TYPE.SLOWED:
                {

                }
                break;
            case STATUS_TYPE.ACCELERATED:
                {

                }
                break;
            default:
                break;
        }
    }
    private void OnDeleteStatus(StatusData statusToDelete)
    {
        switch (statusToDelete.statusType)
        {
            case STATUS_TYPE.SLOWED:
                {
                    this.speedMult += statusToDelete.severity;

                    this.myDeltaTime = Time.deltaTime * speedMult;
                }
                break;
            case STATUS_TYPE.ACCELERATED:
                {
                    this.speedMult -= statusToDelete.severity;

                    this.myDeltaTime = Time.deltaTime * speedMult;
                }
                break;
            default:
                break;
        }
    }

    protected void UpdateStatuses()
    {
        var mapKeys = statuses.Keys;
        foreach (STATUS_TYPE statusType in mapKeys)
        {
            if (statuses.ContainsKey(statusType) == false)
                continue;

            statuses[statusType].remainingTime -= Time.deltaTime;

            if (statuses[statusType].remainingTime > 0f)
            {
                OnUpdateStatus(statuses[statusType]);
            }
            else
            {
                statusToDelete.Add(statusType);
            }

        }

        for (int i = 0; i < statusToDelete.Count; i++)
        {
            RemoveStatus(statusToDelete[i]);
        }
        statusToDelete.Clear();

    }

    private void DeleteStatus(STATUS_TYPE statusType, bool triggerOnDeleteEffects = true)
    {
        if (statuses.ContainsKey(statusType) == true)
        {
            if (triggerOnDeleteEffects == true)
                OnDeleteStatus(statuses[statusType]);

            statuses.Remove(statusType);
        }

    }




}