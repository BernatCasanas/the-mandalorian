using System;
using System.Collections.Generic;
using DiamondEngine;

public class BabyYoda : DiamondComponent
{
    //Vertical Movement
    public float verticalSpeed = 0.8f;
    public float verticalTimeInterval = 1.2f;
    private float verticalTimer = 0.0f;
    private bool moveDown = true;

    //Horizontal Movement
    public GameObject followPoint = null;
    public float horizontalSpeed = 4f;

    //State (INPUT AND STATE LOGIC)
    #region STATE
    private STATE state = STATE.MOVE;
    private INPUTS input = INPUTS.NONE;

    public float skillPushDuration = 0.8f;
    private float skillPushTimer = 0.0f;

    private float INIT_TIMER = 0.0001f;

   

    #region STATE_ENUMS
    enum STATE
    {
        NONE = -1,
        MOVE,
        SKILL_PUSH
    }

    enum INPUTS
    {
        NONE = -1,
        IN_SKILL_PUSH,
        IN_SKILL_PUSH_END,
    }
    #endregion

    #endregion

    public void Update()
    {
        UpdateState();
        Move();
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
            float x = Mathf.Lerp(gameObject.transform.localPosition.x, followPoint.transform.globalPosition.x, horizontalSpeed * Time.deltaTime);
            float z = Mathf.Lerp(gameObject.transform.localPosition.z, followPoint.transform.globalPosition.z, horizontalSpeed * Time.deltaTime);
            gameObject.transform.localPosition = new Vector3(x, gameObject.transform.localPosition.y, z);
        }
    }


    private void MoveVertically()
    {
        if (moveDown == true)
        {
            gameObject.transform.localPosition -= new Vector3(0.0f, 1.0f, 0.0f) * verticalSpeed * Time.deltaTime;
        }

        else
            gameObject.transform.localPosition += new Vector3(0.0f, 1.0f, 0.0f) * verticalSpeed * Time.deltaTime;

        verticalTimer += Time.deltaTime;

        if (verticalTimer >= verticalTimeInterval)
        {
            moveDown = !moveDown;
            verticalTimer = 0.0f;
        }
    }


    private void LookAtMando()
    {
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
            default:
                break;
        }
    }


    //All button inputs must be handled here
    private void ProcessExternalInput()
    {
        if (Input.GetGamepadButton(DEControllerButton.RIGHTSHOULDER) == KeyState.KEY_DOWN)
        {
            if (input == INPUTS.NONE)
                input = INPUTS.IN_SKILL_PUSH;
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

            default:
                break;
        }
    }
    #endregion

    private void ExecutePushSkill()
    {
        skillPushTimer += INIT_TIMER;

        Transform mandoTransform = Core.instance.gameObject.transform;
        InternalCalls.CreatePrefab("Library/Prefabs/541990364.prefab", new Vector3(mandoTransform.globalPosition.x, mandoTransform.globalPosition.y + 1, mandoTransform.globalPosition.z), mandoTransform.globalRotation, new Vector3(1, 1, 1));
    }
}