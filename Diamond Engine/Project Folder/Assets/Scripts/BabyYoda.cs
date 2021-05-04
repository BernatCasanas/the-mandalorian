using System;
using System.Collections.Generic;
using DiamondEngine;

public class BabyYoda : DiamondComponent
{
    public static BabyYoda instance;
    private Core Mando = null;
    //Vertical Movement
    public float verticalSpeed = 0.8f;

    public float verticalAmplitude;
    private bool flipVertical = false;
    private float verticalOffset = 0.0f;
    public bool followPlayer = false;

    public float avoidHitDistance = 0.0f;

    //Horizontal Movement
    public float horizontalSpeed = 4f;

    //Force (HUD) variables
    private float currentForce = 0;
    private static int totalForce = 100;
    private static float forceRegenerationSpeed = 4f;
    private float forceRegenerationSpeedDft = 4f;

    // Force Skills Cost
    private static int skillPushForceCost = 40;
    private static int skillWallForceCost = 60;

    //Push
    public float pushHorizontalForce = 100;
    public float pushVerticalForce = 10;
    public float PushStun = 2;

    //Force flow feedback
    public GameObject forceParticle = null;
    private ParticleSystem forcePartSys = null;
    private bool canPlayParticles = false;
    public GameObject forceGaugeFeedback = null;
    private float forceFBTimer = 0.0f;
    public float forceFBTime = 0.25f;

    //State (INPUT AND STATE LOGIC)
    #region STATE
    private STATE state = STATE.MOVE;
    private INPUTS input = INPUTS.NONE;

    public float skillPushDuration = 0.8f;
    private float skillPushTimer = 0.0f;

    private float INIT_TIMER = 0.0001f;
    private bool lefttTriggerPressed = false;

    private Vector3 wallSkillOffset = Vector3.zero;
    private float wallSkillDuration = 1.0f; //TODO provisional we have to use the force bar with a cost instead
    private float skillWallTimer = 0.0f;
    private bool leftButtonPressed = false;
    private Vector3 pointToFollow;


    #region STATE_ENUMS
    enum STATE
    {
        NONE = -1,
        MOVE,
        SKILL_PUSH,
        SKILL_WALL
    }

    enum INPUTS
    {
        NONE = -1,
        IN_SKILL_PUSH,
        IN_SKILL_PUSH_END,
        IN_SKILL_WALL,
        IN_SKILL_WALL_END,
    }
    #endregion

    #endregion


    #region HUD
    private void UpdateHUD()
    {
        if (Core.instance.hud == null)
            return;

        if (currentForce < totalForce)
        {
            if (Skill_Tree_Data.IsEnabled((int)Skill_Tree_Data.SkillTreesNames.GROGU, (int)Skill_Tree_Data.GroguSkillNames.FORCE_REGENERATION))
                currentForce += (GetForceRegenSpeedWithSkill() * Time.deltaTime);
            else currentForce += (forceRegenerationSpeed * Time.deltaTime);

            if (currentForce > totalForce)
            {
                currentForce = totalForce;
                if (canPlayParticles)
                {
                    Debug.Log("PARTICLES PLAY!!");
                    forcePartSys.Play();
                    canPlayParticles = false;
                    forceGaugeFeedback.Enable(true);
                    forceFBTimer = forceFBTime;
                }
            }

            Core.instance.hud.GetComponent<HUD>().UpdateForce((int)currentForce, totalForce);
        }
    }
    #endregion

    public void Awake()
    {
        instance = this;
        wallSkillOffset = new Vector3(0.0f, 1.5f, 3.0f);

        currentForce = totalForce;
        flipVertical = false;

        if (forceParticle == null)
        {
            forceParticle = InternalCalls.FindObjectWithName("ForceParticle");
            if (forceParticle == null)
                Debug.Log("Force particle game object not found");
        }

        if (forceParticle != null)
            forcePartSys = forceParticle.GetComponent<ParticleSystem>();

        if (forceGaugeFeedback == null)
        {
            forceGaugeFeedback = InternalCalls.FindObjectWithName("ForceFeedback");
            if (forceGaugeFeedback == null)
                Debug.Log("Force feedback game object not found");
        }

        if (forceGaugeFeedback != null)
            forceGaugeFeedback.Enable(false);
    }

    public void Update()
    {
        if (Mando == null && Core.instance != null)
        {
            Mando = Core.instance;
            Transform playerTransform = Mando.gameObject.transform;
            pointToFollow = playerTransform.globalPosition + (Vector3.up * 1.5f) - playerTransform.GetForward() + (playerTransform.GetRight() * 0.5f);
            gameObject.transform.localPosition = pointToFollow;
        }

        UpdateState();
        Move();
        UpdateHUD();

        if (forceFBTimer >= 0.0f)
            forceFBTimer -= Time.deltaTime;
        if (forceFBTimer < 0.0f)
        {
            forceGaugeFeedback.Enable(false);
            forceFBTimer = -1.0f;
        }
    }

    #region MOVEMENT
    private void Move()
    {
        FollowPoint();
        MoveVertically();
        LookAtMando();
    }


    private void FollowPoint()
    {
        if (!followPlayer)
            return;

        Vector3 frameIncrement = Vector3.zero;
        if (Core.instance != null)
        {
            Transform playerTransform = Core.instance.gameObject.transform;
            pointToFollow = playerTransform.globalPosition + (Vector3.up * 1.5f) - playerTransform.GetForward() + (playerTransform.GetRight() * 0.5f);
            frameIncrement = pointToFollow;

            //(flipVertical)
            verticalOffset += (flipVertical == true ? verticalSpeed : -verticalSpeed) * Time.deltaTime;

            if(flipVertical == true && gameObject.transform.globalPosition.y - pointToFollow.y >= verticalAmplitude)
            {
                flipVertical = false;
                //Debug.Log("Flip to down");
            }
            else if(flipVertical == false && pointToFollow.y - gameObject.transform.globalPosition.y >= verticalAmplitude)
            {
                flipVertical = true;
                //Debug.Log("Flip to up");
            }

        }

        float angleIncrement = 360 / 8;
        for (int i = 0; i < 8; i++)
        {
            ////Quaternion rotation = 
            Quaternion q = Quaternion.RotateAroundAxis(new Vector3(0, 1, 0), (angleIncrement * i) * 0.0174532925f);
            Vector3 v = gameObject.transform.GetForward() /** laserRange*/;

            // Do the math
            Vector3 rayDirection = Vector3.RotateAroundQuaternion(q, v);

            float hitDistance = 0.0f;
            GameObject _hit = InternalCalls.RayCast(gameObject.transform.globalPosition, rayDirection, avoidHitDistance, ref hitDistance);

            if (_hit != null && !_hit.CompareTag("Player"))
            {
                Debug.Log("Should avoid");

                Vector3 avoidVector = (rayDirection.normalized * hitDistance) * -1;
                avoidVector.y = 0.0f;
                frameIncrement += avoidVector;
            }

        }
        frameIncrement.x = Mathf.Lerp(gameObject.transform.localPosition.x, frameIncrement.x, horizontalSpeed * Time.deltaTime);
        frameIncrement.y = pointToFollow.y + verticalOffset;
        frameIncrement.z = Mathf.Lerp(gameObject.transform.localPosition.z, frameIncrement.z, horizontalSpeed * Time.deltaTime);

        gameObject.transform.localPosition = frameIncrement;
        //if (followPoint != null)
        //{
        //    GroguFPManager fpManager = followPoint.GetComponent<GroguFPManager>();
        //    if (fpManager == null)
        //    {
        //        Debug.Log("Need to add Follow points manager to Grogu!");
        //        return;
        //    }

        //    Vector3 point = fpManager.GetPointToFollow(gameObject.transform.globalPosition);

        //    if (point != Vector3.zero)
        //    {
        //        float x = Mathf.Lerp(gameObject.transform.localPosition.x, point.x, horizontalSpeed * Time.deltaTime);
        //        float z = Mathf.Lerp(gameObject.transform.localPosition.z, point.z, horizontalSpeed * Time.deltaTime);
        //        gameObject.transform.localPosition = new Vector3(x, gameObject.transform.localPosition.y, z);
        //    }

        //}
    }


    private void MoveVertically()
    {
        //if (followPoint != null)
        //{

        //    Vector3 movement = gameObject.transform.localPosition;
        //    if (moveDown == true)
        //    {
        //        movement -= new Vector3(0.0f, 1.0f, 0.0f) * verticalSpeed * Time.deltaTime;
        //        if (movement.y >= followPoint.transform.globalPosition.y - 2)
        //            gameObject.transform.localPosition = movement;
        //    }
        //    else
        //    {
        //        movement += new Vector3(0.0f, 1.0f, 0.0f) * verticalSpeed * Time.deltaTime;
        //        if (movement.y <= followPoint.transform.globalPosition.y + 2)
        //            gameObject.transform.localPosition = movement;
        //    }
        //}

        //verticalTimer += Time.deltaTime;

        //if (verticalTimer >= verticalTimeInterval)
        //{
        //    moveDown = !moveDown;
        //    verticalTimer = 0.0f;
        //}
    }


    private void LookAtMando()
    {
        if (Core.instance.gameObject == null)
            return;
        Vector3 worldForward = new Vector3(0, 0, 1);

        Vector3 vec = Core.instance.gameObject.transform.localPosition + Core.instance.gameObject.transform.GetForward() * 2 - gameObject.transform.globalPosition;
        vec = vec.normalized;

        float dotProduct = vec.x * worldForward.x + vec.z * worldForward.z;
        float determinant = vec.x * worldForward.z - vec.z * worldForward.x;

        float angle = (float)Math.Atan2(determinant, dotProduct);

        gameObject.transform.localRotation = Quaternion.RotateAroundAxis(new Vector3(0, 1, 0), angle);
    }
    #endregion

    #region STATE
    private void UpdateState()
    {
        input = INPUTS.NONE;

        ProcessInternalInput();
        ProcessExternalInput();

        ProcessState();

        switch (state)
        {
            case STATE.MOVE:
                //Do animation / play sounds / whathever
                break;
            case STATE.SKILL_PUSH:
                break;
            case STATE.SKILL_WALL:
                break;
            default:
                break;
        }
    }


    //All button inputs must be handled here
    private void ProcessExternalInput()
    {
        if (Input.GetLeftTrigger() > 0 && lefttTriggerPressed == false)
        {
            if (input == INPUTS.NONE)
            {
                input = INPUTS.IN_SKILL_PUSH;
                lefttTriggerPressed = true;
            }
        }
        else if (Input.GetLeftTrigger() == 0)
        {
            lefttTriggerPressed = false;
        }

        if (Input.GetGamepadButton(DEControllerButton.LEFTSHOULDER) > 0 && leftButtonPressed == false)
        {
            if (input == INPUTS.NONE)
            {
                input = INPUTS.IN_SKILL_WALL;
                leftButtonPressed = true;
            }
        }
        else if (Input.GetGamepadButton(DEControllerButton.LEFTSHOULDER) == 0)
        {
            leftButtonPressed = false;
        }
    }


    //All timer things must be updated here
    private void ProcessInternalInput()
    {
        if (skillPushTimer > 0.0f)
        {
            skillPushTimer += Time.deltaTime;

            if (skillPushTimer >= skillPushDuration && input == INPUTS.NONE)
            {
                input = INPUTS.IN_SKILL_PUSH_END;
                skillPushTimer = 0.0f;
            }
        }

        if (skillWallTimer > 0.0f)
        {
            skillWallTimer += Time.deltaTime;

            if (skillWallTimer >= wallSkillDuration && input == INPUTS.NONE)
            {
                input = INPUTS.IN_SKILL_WALL_END;
                skillWallTimer = 0.0f;
            }
        }

    }

    private void ProcessState()
    {
        switch (state)
        {
            case STATE.NONE:
                break;
            case STATE.MOVE:
                switch (input)
                {
                    case INPUTS.IN_SKILL_PUSH:
                        bool skillPushExecuted = ExecutePushSkill();
                        if (skillPushExecuted == true)
                            state = STATE.SKILL_PUSH;
                        break;
                    case INPUTS.IN_SKILL_WALL:
                        bool skillWallExecuted = ExecuteWallSkill();
                        if (skillWallExecuted == true)
                            state = STATE.SKILL_WALL;
                        break;
                }
                break;

            case STATE.SKILL_PUSH:
                switch (input)
                {
                    case INPUTS.IN_SKILL_PUSH_END:
                        state = STATE.MOVE;
                        break;
                }
                break;

            case STATE.SKILL_WALL:
                switch (input)
                {
                    case INPUTS.IN_SKILL_WALL_END:
                        state = STATE.MOVE;
                        break;
                }
                break;

            default:
                break;
        }
    }
    #endregion

    private bool ExecutePushSkill()
    {
        if (Core.instance.gameObject == null || currentForce < skillPushForceCost)
            return false;

        if (Core.instance.hud != null)
        {
            SetCurrentForce((int)currentForce - skillPushForceCost);
        }
        skillPushTimer += INIT_TIMER;

        Transform mandoTransform = Core.instance.gameObject.transform;
        InternalCalls.CreatePrefab("Library/Prefabs/541990364.prefab", new Vector3(mandoTransform.globalPosition.x, mandoTransform.globalPosition.y + 1, mandoTransform.globalPosition.z), mandoTransform.globalRotation, new Vector3(1, 1, 1));
        Audio.PlayAudio(gameObject, "Play_Force_Push");
        
        //Talent Trees
        //if (Skill_Tree_Data.IsEnabled((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.UTILITY_HEAL_WHEN_GROGU_SKILL))        
        //    Core.instance.gameObject.GetComponent<PlayerHealth>().TakeDamage(-Skill_Tree_Data.GetMandoSkillTree().U7_healAmount);

        //if (Skill_Tree_Data.IsEnabled((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.UTILITY_INCREASE_DAMAGE_WHEN_GROGU))
        //    Core.instance.SetSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.UTILITY_INCREASE_DAMAGE_WHEN_GROGU);

        return true;
    }

    //Execute order 66
    private bool ExecuteWallSkill()
    {
        if (Core.instance.gameObject == null || currentForce < skillWallForceCost)
            return false;

        if (Core.instance.hud != null)
        {
            currentForce -= skillWallForceCost;
            Core.instance.hud.GetComponent<HUD>().UpdateForce((int)currentForce, totalForce);
        }

        skillWallTimer += INIT_TIMER;
        Transform mandoTransform = Core.instance.gameObject.transform;
        Vector3 spawnPos = new Vector3(mandoTransform.globalPosition.x, mandoTransform.globalPosition.y, mandoTransform.globalPosition.z);
        spawnPos += mandoTransform.GetRight() * wallSkillOffset.x;
        spawnPos += Vector3.Cross(mandoTransform.GetForward(), mandoTransform.GetRight()) * wallSkillOffset.y;
        spawnPos += mandoTransform.GetForward() * wallSkillOffset.z;

        InternalCalls.CreatePrefab("Library/Prefabs/1850725718.prefab", spawnPos, mandoTransform.globalRotation, new Vector3(1, 1, 1));
        Audio.PlayAudio(gameObject, "Play_Grogu_Wall");
        
        if (Skill_Tree_Data.IsEnabled((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.UTILITY_HEAL_WHEN_GROGU_SKILL))        
            Core.instance.gameObject.GetComponent<PlayerHealth>().TakeDamage(-Skill_Tree_Data.GetMandoSkillTree().U7_healAmount);                

        return true;
    }

    public static float GetForceRegenerationSpeed()
    {
        if (Skill_Tree_Data.IsEnabled((int)Skill_Tree_Data.SkillTreesNames.GROGU, (int)Skill_Tree_Data.GroguSkillNames.FORCE_REGENERATION))
            return GetForceRegenSpeedWithSkill();
        else return forceRegenerationSpeed;
    }

    public static void AugmentForceRegenSpeed(float newSpeed)
    {
        forceRegenerationSpeed += newSpeed;
    }

    public static void SetForceRegenerationSpeed(float newSpeed)
    {
        forceRegenerationSpeed = newSpeed;
    }

    public int GetCurrentForce()
    {
        return (int)currentForce;
    }

    public void SetCurrentForce(int newForceValue)
    {
        currentForce = newForceValue;
        canPlayParticles = true;
        Core.instance.hud.GetComponent<HUD>().UpdateForce((int)currentForce, totalForce);
    }

    public static int GetMaxForce()
    {
        return totalForce;
    }

    public static void SetMaxForce(int newValue)
    {
        totalForce = newValue;
    }

    public static int GetPushCost()
    {
        return skillPushForceCost;
    }

    public static void SetPushCost(int newValue)
    {
        skillPushForceCost = newValue;
    }


    public void OnApplicationQuit()
    {
        forceRegenerationSpeed = forceRegenerationSpeedDft;
    }

    private static float GetForceRegenSpeedWithSkill()
    {
        int HPDiff = PlayerHealth.currMaxHealth - PlayerHealth.currHealth;
        float steps = (float)HPDiff / Skill_Tree_Data.GetGroguSkillTree().Grogu8_HPMissingPercentage;

        return forceRegenerationSpeed + steps * Skill_Tree_Data.GetGroguSkillTree().Grogu8_gainPassiveForceRegeneration;
    }
}