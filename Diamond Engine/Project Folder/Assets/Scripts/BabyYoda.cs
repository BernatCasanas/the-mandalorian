using System;
using System.Collections.Generic;
using DiamondEngine;

public class BabyYoda : DiamondComponent
{
    public static BabyYoda instance;

    //Vertical Movement
    public float verticalSpeed = 0.8f;
    public float verticalTimeInterval = 1.2f;
    private float verticalTimer = 0.0f;
    private bool moveDown = true;

    //Horizontal Movement
    public GameObject followPoint = null;
    public float horizontalSpeed = 4f;

    //HUD variables
    private bool force_updating = false;
    private float final_timer_force = 0.0f;

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

    private static float timeForceRegeneration = 4f;
    private float timeForceRegenerationDefault = 4f;
    private float timeWhenStartRecharge = 0f;

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
        if (!force_updating || Core.instance.hud == null)
            return;

        if (Time.totalTime >= final_timer_force)
        {
            Core.instance.hud.GetComponent<HUD>().UpdateForce(100, 100);
            Core.instance.hud.GetComponent<HUD>().ChangeAlphaSkillPush(true);
            force_updating = false;
        }
        else
        {
            float force_bar_normalized = ((Time.totalTime - timeWhenStartRecharge) / (final_timer_force - timeWhenStartRecharge)) * 100;
            Core.instance.hud.GetComponent<HUD>().UpdateForce((int)force_bar_normalized, 100);
        }
    }
    #endregion

    public void Awake()
    {
        instance = this;
        wallSkillOffset = new Vector3(0.0f, 1.5f, 3.0f);
    }

    public void Update()
    {
        UpdateState();
        Move();
        UpdateHUD();
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
        if (followPoint != null)
        {
            GroguFPManager fpManager = followPoint.GetComponent<GroguFPManager>();
            if (fpManager == null)
            {
                Debug.Log("Need to add Follow points manager to Grogu!");
                return;
            }

            Vector3 point = fpManager.GetPointToFollow(gameObject.transform.globalPosition);

            if (point != Vector3.zero)
            {
                float x = Mathf.Lerp(gameObject.transform.localPosition.x, point.x, horizontalSpeed * Time.deltaTime);
                float z = Mathf.Lerp(gameObject.transform.localPosition.z, point.z, horizontalSpeed * Time.deltaTime);
                gameObject.transform.localPosition = new Vector3(x, gameObject.transform.localPosition.y, z);
            }

        }
    }


    private void MoveVertically()
    {
        if (followPoint != null)
        {

            Vector3 movement = gameObject.transform.localPosition;
            if (moveDown == true)
            {
                movement -= new Vector3(0.0f, 1.0f, 0.0f) * verticalSpeed * Time.deltaTime;
                if (movement.y >= followPoint.transform.globalPosition.y - 2)
                    gameObject.transform.localPosition = movement;
            }
            else
            {
                movement += new Vector3(0.0f, 1.0f, 0.0f) * verticalSpeed * Time.deltaTime;
                if (movement.y <= followPoint.transform.globalPosition.y + 2)
                    gameObject.transform.localPosition = movement;
            }
        }

        verticalTimer += Time.deltaTime;

        if (verticalTimer >= verticalTimeInterval)
        {
            moveDown = !moveDown;
            verticalTimer = 0.0f;
        }
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
                        ExecutePushSkill();
                        state = STATE.SKILL_PUSH;
                        break;
                    case INPUTS.IN_SKILL_WALL:
                        ExecuteWallSkill();
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

    private void ExecutePushSkill()
    {
        if (Core.instance.hud != null)
        {
            force_updating = true;
            final_timer_force = Time.totalTime + timeForceRegeneration;
            timeWhenStartRecharge = Time.totalTime;
            Core.instance.hud.GetComponent<HUD>().UpdateForce(0, 100);
            Core.instance.hud.GetComponent<HUD>().ChangeAlphaSkillPush(false);
        }
        skillPushTimer += INIT_TIMER;
        if (Core.instance.gameObject == null)
            return;
        Transform mandoTransform = Core.instance.gameObject.transform;
        InternalCalls.CreatePrefab("Library/Prefabs/541990364.prefab", new Vector3(mandoTransform.globalPosition.x, mandoTransform.globalPosition.y + 1, mandoTransform.globalPosition.z), mandoTransform.globalRotation, new Vector3(1, 1, 1));
    }

    //Execute order 66
    private void ExecuteWallSkill()
    {
        if (Core.instance.hud != null)
        {
            force_updating = true;
            final_timer_force = Time.totalTime + timeForceRegeneration;
            timeWhenStartRecharge = Time.totalTime;
            Core.instance.hud.GetComponent<HUD>().UpdateForce(0, 100);
            Core.instance.hud.GetComponent<HUD>().ChangeAlphaSkillPush(false);
        }
        if (Core.instance.gameObject == null)
            return;
        skillWallTimer += INIT_TIMER;
        Transform mandoTransform = Core.instance.gameObject.transform;
        Vector3 spawnPos = new Vector3(mandoTransform.globalPosition.x, mandoTransform.globalPosition.y, mandoTransform.globalPosition.z);
        spawnPos += mandoTransform.GetRight() * wallSkillOffset.x;
        spawnPos += Vector3.Cross(mandoTransform.GetForward(), mandoTransform.GetRight()) * wallSkillOffset.y;
        spawnPos += mandoTransform.GetForward() * wallSkillOffset.z;

        InternalCalls.CreatePrefab("Library/Prefabs/1850725718.prefab", spawnPos, mandoTransform.globalRotation, new Vector3(1, 1, 1));

    }

    public void ReduceForceRegenerationTime(float time)
    {
        timeForceRegeneration -= time;
    }

    public void OnApplicationQuit()
    {
        timeForceRegeneration = timeForceRegenerationDefault;
    }

}