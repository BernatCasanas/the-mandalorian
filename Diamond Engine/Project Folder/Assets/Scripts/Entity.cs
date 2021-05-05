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
    ACCELERATED,
    DAMAGE_DOWN,
    MOV_SPEED,
    OVERHEAT,
    BLASTER_DAMAGE,
    DMG_TO_BOSSES,
    DMG_PER_HP,
    MAX_HP,
    DMG_RED,
    DMG_PER_HEAT,
    FALL,
    GROGU_COST,
    SKILL_HEAL,
    COMBO_FIRE_RATE,
    FIRE_RATE,
    COMBO_DAMAGE,
    RAW_DAMAGE
}

public enum STATUS_APPLY_TYPE
{
    NONE = -1,
    BIGGER_PERCENTAGE,
    BIGGER_TIME,
    SUBSTITUTE,
    ADDITIVE
}

#region StatusData
public class StatusData
{
    //Constructors
    public StatusData()
    {
        statusType = STATUS_TYPE.NONE;
        applyType = STATUS_APPLY_TYPE.NONE;
        severity = 0f;
        statChange = 0f;
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
        statChange = statusStatTaken;
    }

    public float severity;
    public float statChange;
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
    protected float MovspeedMult = 1f;
    public float OverheatMult = 1f;
    public float RawDamageMult = 1f;
    public float BlasterDamageMult = 1f;
    public float DamageToBosses = 1f;
    public float DamagePerHpMult = 1f;
    public float DamagePerHeatMult = 1f;
    public float DamageRed = 1f;
    public float GroguCost = 1f;
    public float FireRateMult = 1f;

    protected virtual void InitEntity(ENTITY_TYPE myType)
    {
        eType = myType;
        speedMult = 1f;
        MovspeedMult = 1f;
        OverheatMult = 1f;
        BlasterDamageMult = 1f;
        DamageToBosses = 1f;
        DamagePerHpMult = 1f;
        DamagePerHeatMult = 1f;
        DamageRed = 1f;
        GroguCost = 1f;
        myDeltaTime = Time.deltaTime;
        FireRateMult = 1f;
        RawDamageMult = 1f;
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

    public float GetStatusRemainingTime(STATUS_TYPE stat)
    {
        float ret = 0f;

        if(statuses.ContainsKey(stat) == true)
        {
            ret = statuses[stat].remainingTime;
        }

        return ret;
    }

    public StatusData GetStatusData(STATUS_TYPE stat)
    {
        StatusData ret = new StatusData();

        if (statuses.ContainsKey(stat) == true)
        {
            ret = statuses[stat];
        }

        return ret;
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

    protected virtual void OnInitStatus(ref StatusData statusToInit)
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
            case STATUS_TYPE.DAMAGE_DOWN:
                {

                }
                break;
            case STATUS_TYPE.MOV_SPEED:
                {
                    statusToInit.statChange = this.MovspeedMult * statusToInit.severity / 100;
                    this.MovspeedMult += statusToInit.statChange;
                //  Debug.Log(this.MovspeedMult.ToString());
                }
                break;
            case STATUS_TYPE.OVERHEAT:
                {
                    statusToInit.statChange = this.OverheatMult * (statusToInit.severity) / 100;
                    this.OverheatMult += statusToInit.statChange;
                   // Debug.Log(this.OverheatMult.ToString());
                }
                break;
            case STATUS_TYPE.BLASTER_DAMAGE:
                {
                    statusToInit.statChange = this.BlasterDamageMult * (statusToInit.severity) / 100;
                    this.BlasterDamageMult += statusToInit.statChange;
                   // Debug.Log(this.DamageMult.ToString());
                }
                break;
            case STATUS_TYPE.DMG_TO_BOSSES:
                {
                    statusToInit.statChange = this.DamageToBosses * (statusToInit.severity) / 100;
                    this.DamageToBosses += statusToInit.statChange;
                  //  Debug.Log(this.DamageToBosses.ToString());
                }
                break;
            case STATUS_TYPE.MAX_HP:
                {
                    if (Core.instance != null)
                    {
                        PlayerHealth myHealth = Core.instance.gameObject.GetComponent<PlayerHealth>();
                        if (myHealth != null)
                        {
                            Debug.Log("TESTING / max hp");
                            Debug.Log("Old Max Health = " + PlayerHealth.currMaxHealth.ToString());
                            statusToInit.statChange = statusToInit.severity * PlayerHealth.currMaxHealth / 100;
                            myHealth.SetMaxHPValue((int)(PlayerHealth.currMaxHealth + statusToInit.statChange));
                            Debug.Log("New Max Health = " + PlayerHealth.currMaxHealth.ToString());

                        }
                    }
                   
                }
                break;
            case STATUS_TYPE.DMG_RED:
                {
                    statusToInit.statChange = this.DamageRed * (statusToInit.severity) / 100;
                    this.DamageRed += statusToInit.statChange;
                    Debug.Log("Damage red = " + this.DamageRed.ToString());

                }
                break;
            case STATUS_TYPE.GROGU_COST:
                {
                    statusToInit.statChange = statusToInit.severity / 100;
                    this.GroguCost += statusToInit.statChange;
                }
                break;
            case STATUS_TYPE.FIRE_RATE:
                {
                  
                        statusToInit.statChange = - statusToInit.severity / 100;
                        this.FireRateMult += statusToInit.statChange;
                   
                  
                }
                break;
            case STATUS_TYPE.RAW_DAMAGE:
                {
                   
                    statusToInit.statChange = (statusToInit.severity) / 100;
                    this.RawDamageMult += statusToInit.statChange;
                    // Debug.Log(this.DamageMult.ToString());
                }
                break;
            default:
                break;
        }
    }

    protected virtual void OnUpdateStatus(StatusData statusToUpdate)
    {
        switch (statusToUpdate.statusType)
        {
            case STATUS_TYPE.DMG_PER_HP:
                {
                    if(Core.instance != null)
                    {
                        //PlayerHealth myHealth = Core.instance.gameObject.GetComponent<PlayerHealth>();
                        //if (myHealth != null)
                        //{
                            float missingHealth = PlayerHealth.currMaxHealth - PlayerHealth.currHealth;
                            float missingHealtPercentage =  missingHealth / PlayerHealth.currMaxHealth;
                            DamagePerHpMult = 1 + missingHealtPercentage;
                      //  Debug.Log("TESTING / dmg per missing hp = " + DamagePerHpMult.ToString());
                        // }
                    }
            
                }
                break;
            case STATUS_TYPE.DMG_PER_HEAT:
                {
                    if (Core.instance != null)
                    {
                        //PlayerHealth myHealth = Core.instance.gameObject.GetComponent<PlayerHealth>();
                        //if (myHealth != null)
                        //{
                        if (Core.instance.hud != null)
                        {
                            float currentheat = Core.instance.hud.GetComponent<HUD>().primaryWeaponHeat;
                            float HeatPercentage = currentheat / Core.instance.hud.GetComponent<HUD>().primaryWeaponMaxHeat;
                            DamagePerHeatMult = 1 + HeatPercentage;
                        //    Debug.Log("TESTING / dmg per heat = " + DamagePerHeatMult.ToString());
                        }
                           
                      
                        // }
                    }

                }
                break;
            default:
                break;
        }
    }

    protected virtual void OnDeleteStatus(StatusData statusToDelete)
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
            case STATUS_TYPE.DAMAGE_DOWN:
                {
                }
                break;
            case STATUS_TYPE.MOV_SPEED:
                {
                    this.MovspeedMult -= statusToDelete.statChange;
                  //  Debug.Log(this.MovspeedMult.ToString());
                }
                break;
            case STATUS_TYPE.OVERHEAT:
                {
                    this.OverheatMult -= statusToDelete.statChange;
                    //    Debug.Log(this.MovspeedMult.ToString());
                }
                break;
            case STATUS_TYPE.BLASTER_DAMAGE:
                {
                    this.BlasterDamageMult -= statusToDelete.statChange;
                    //    Debug.Log(this.MovspeedMult.ToString());
                }
                break;
            case STATUS_TYPE.DMG_TO_BOSSES:
                {
                    this.DamageToBosses -= statusToDelete.statChange;
                    //    Debug.Log(this.MovspeedMult.ToString());
                }
                break;
            case STATUS_TYPE.DMG_PER_HP:
                {
                    this.DamagePerHpMult = 1;
                    //    Debug.Log(this.MovspeedMult.ToString());
                }
                break;
            case STATUS_TYPE.MAX_HP:
                {
                    if (Core.instance != null)
                    {
                        PlayerHealth myHealth = Core.instance.gameObject.GetComponent<PlayerHealth>();
                        if (myHealth != null)
                        {

                            myHealth.SetMaxHPValue((int)(PlayerHealth.currMaxHealth - statusToDelete.statChange));

                        }
                    }
                }
                break;
            case STATUS_TYPE.DMG_RED:
                {
                    this.DamageRed -= statusToDelete.statChange;
                    Debug.Log("Damage red = " + this.DamageRed.ToString());
                    //    Debug.Log(this.MovspeedMult.ToString());
                }
                break;
            case STATUS_TYPE.DMG_PER_HEAT:
                {
                    this.DamagePerHeatMult = 1;
                    //    Debug.Log(this.MovspeedMult.ToString());
                }
                break;
            case STATUS_TYPE.GROGU_COST:
                {
                   this.GroguCost -= statusToDelete.statChange;
                    //    Debug.Log(this.MovspeedMult.ToString());
                }
                break;
            case STATUS_TYPE.FIRE_RATE:
                {
                        Debug.Log("delete fire rate buff");
                    this.FireRateMult -= statusToDelete.statChange;
                    if (this.FireRateMult <= 0)
                        this.FireRateMult = 1;
                }
                break;
            case STATUS_TYPE.RAW_DAMAGE:
                {
                    Debug.Log("delete raw damage buff");

                    this.RawDamageMult -= statusToDelete.statChange;
                    if(this.RawDamageMult > 2)
                        this.RawDamageMult = 1;
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

            if (statuses[statusType].remainingTime > 0f || statuses[statusType].isPermanent)
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

    protected void copyBuffs(ref Dictionary<STATUS_TYPE, StatusData> newstatuses)
    {
        newstatuses = statuses;
    }
}