using System;
using DiamondEngine;

public class DynamicProps : DiamondComponent
{
    public enum STATE : int
    {
        NONE = -1,
        WAIT,
        MOVE,
        ROTATE,
        SCREAM,
    }

    //INSPECTOR FLOAT INT GAMEOBJECT BOOl PUBLIC LO DEMAS PRIVATE
    private STATE currentState = STATE.NONE;

    //Variables
    public int RotateAxis = 0;
    public int MoveAxis = 0;
    public float RotateAngle = 90;
    public float moveSpeed = 2.0f;
    public float rotateSpeed = 0.200f;

    //Timers
    private float idleTimer = 0.0f;
    private float screamTimer = 0.0f;
    private float waitTimer = 0.0f;
    private float rotateTimer = 0.0f;

    //Action times
    private float idleTime = 0.0f;
    private float screamTime = 0.0f;
    private float rotateTime = 0.0f;

    private Vector3 initialPos;
    private Quaternion initialRot;

    public bool reset;
    public bool start_Idle = false;
    public bool isScreaming = false;


    public int waitSeconds = 2;

    public int STATUS;
    public void Awake()
    {

        currentState = STATE.WAIT;

        if (gameObject.tag == "LittleMen")
            isScreaming = true;

        if (!start_Idle)
            SwitchState(STATE.MOVE);

        initialPos = new Vector3(gameObject.transform.localPosition);
        initialRot = gameObject.transform.localRotation;

        reset = false;

        //SCREAM
        screamTime = Animator.GetAnimationDuration(gameObject, "PD_Scream");

        //IDLE
        idleTime = Animator.GetAnimationDuration(gameObject, "PD_Idle");

        //ROTATE
        rotateTime = RotateAngle * Mathf.Deg2RRad / rotateSpeed;
    }

    public void DebugState()
    {
        STATUS = (int)currentState;
    }

    public void Update()
    {
        LittleMen();
        UpdateState();
        DebugState();
    }

    public void UpdateState()
    {
        switch (currentState)
        {
            case STATE.WAIT:
                UpdateWait(waitSeconds);
                break;

            case STATE.MOVE:
                UpdateMove(MoveAxis);
                break;

            case STATE.ROTATE:
                UpdateRotate(RotateAxis, RotateAngle);
                break;

            case STATE.SCREAM:
                break;

            default:
                break;
        }
    }

    public void SwitchState(STATE nextState)
    {
        switch (currentState)
        {
            case STATE.WAIT:
                switch (nextState)
                {
                    case STATE.ROTATE:
                        currentState = STATE.ROTATE;
                        StartRotate();
                        break;

                    case STATE.MOVE:
                        currentState = STATE.MOVE;
                        StartMove();
                        break;
                    case STATE.SCREAM:
                        currentState = STATE.SCREAM;
                        StartScream();
                        break;
                }
                break;

            case STATE.MOVE:
                switch (nextState)
                {
                    case STATE.WAIT:
                        currentState = STATE.WAIT;
                        StartWait();
                        break;

                    case STATE.ROTATE:
                        currentState = STATE.ROTATE;
                        StartRotate();
                        break;

                    case STATE.SCREAM:
                        currentState = STATE.SCREAM;
                        break;
                }
                break;

            case STATE.ROTATE:
                switch (nextState)
                {
                    case STATE.MOVE:
                        currentState = STATE.MOVE;
                        StartMove();
                        break;
                }
                break;

            case STATE.SCREAM:
                switch (nextState)
                {
                    case STATE.WAIT:
                        currentState = STATE.WAIT;
                        StartWait();
                        break;
                }
                break;

            default:
                break;
        }
    }

    public void TriggerAction(string tag)
    {
        Debug.Log(tag);
        switch (tag)
        {
            case "Rotate90":
                SwitchState(STATE.ROTATE);
                break;

            case "Rotate-90":
                SwitchState(STATE.ROTATE);
                break;

            case "Scream":
                SwitchState(STATE.SCREAM);
                break;

            case "Wait":
                SwitchState(STATE.WAIT);
                break;

            case "Reset":
                reset = true;
                SwitchState(STATE.WAIT);
                break;
            case "First_Pos":
                SwitchState(STATE.ROTATE);
                break;
            case "Second_Pos":
                SwitchState(STATE.ROTATE);
                break;
            case "LVL2_3_1":
                Debug.Log("Entra");
                SwitchState(STATE.ROTATE);
                break;
            case "LVL2_3_2":
                Debug.Log("Entra");
                Reset();
                break;

            default:
                break;
        }
    }

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        Debug.Log("TRIGGER ENTER");

        TriggerAction(triggeredGameObject.tag);
    }

    public void OnTriggerExit()
    {

    }

    #region WAIT
    public void StartWait()
    {
        waitTimer = waitSeconds;
        Animator.Play(gameObject, "PD_Idle");
    }

    public void UpdateWait(int seconds)
    {
        if (waitTimer > 0.0f)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0.0f)
            {
                isScreaming = true;

                if (reset == true)
                    Reset();
            }
        }
    }
    #endregion

    #region MOVE
    public void StartMove()
    {
        Animator.Play(gameObject, "PD_Walk");
    }

    public void UpdateMove(int axis)
    {
        if (axis == 0)
        {
            //Vector3 newPos = new Vector3(gameObject.transform.localPosition.x + moveSpeed * Time.deltaTime, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);

            gameObject.transform.localPosition += gameObject.transform.GetRight().normalized * moveSpeed * Time.deltaTime;
        }
        else if (axis == 1)
        {
            Vector3 newPos = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y + moveSpeed * Time.deltaTime, gameObject.transform.localPosition.z);

            gameObject.transform.localPosition = newPos;
        }
        else
        {
            //Vector3 newPos = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z + moveSpeed * Time.deltaTime);

            gameObject.transform.localPosition += gameObject.transform.GetForward().normalized * moveSpeed * Time.deltaTime;
        }

    }
    #endregion

    #region ROTATE
    public void StartRotate()
    {
        Animator.Play(gameObject, "PD_Rotate");

        if (rotateTime != 0.0f)
            rotateTimer = rotateTime;
        else
            rotateTimer = 1.0f;

    }

    public void UpdateRotate(int axis, float angle)
    {

        if (rotateTimer > 0.0f)
        {
            rotateTimer -= Time.deltaTime;

            if (rotateTimer <= 0.0f)
            {
                SwitchState(STATE.MOVE);
                return;
            }
        }

        if (axis == 0)
            gameObject.transform.localRotation *= Quaternion.RotateAroundAxis(Vector3.right, rotateSpeed * Time.deltaTime);

        else if (axis == 1)
            gameObject.transform.localRotation *= Quaternion.RotateAroundAxis(Vector3.up, rotateSpeed * Time.deltaTime);

        else
            gameObject.transform.localRotation *= Quaternion.RotateAroundAxis(Vector3.forward, rotateSpeed * Time.deltaTime);


    }

    #endregion

    #region SCREAM
    public void StartScream()
    {
        screamTimer = screamTime;
        Animator.Play(gameObject, "PD_Scream");
    }

    public void UpdateScream()
    {

    }
    #endregion

    public void Reset()
    {

        gameObject.transform.localPosition = initialPos;
        gameObject.transform.localRotation = initialRot;

        Debug.Log("RESET");
    }

    public void LittleMen()
    {
        if (gameObject.tag == "LittleMen")
        {
            if (isScreaming)
                SwitchState(STATE.SCREAM);

            Debug.Log(screamTimer.ToString());

            if (screamTimer > 0.0f)
            {
                screamTimer -= Time.deltaTime;

                if (screamTimer <= 0.0f)
                {
                    SwitchState(STATE.WAIT);
                    isScreaming = false;
                }

            }

        }
    }
}