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
    private float currentRotation;

    //Timers
    private float idleTimer = 0.0f;
    private float screamTimer = 0.0f;
    private float waitTimer = 0.0f;

    //Action times
    private float idleTime = 0.0f;
    private float screamTime = 0.0f;

    private Vector3 initialPos;
    private Quaternion initialRot;

    public bool reset;
    public int waitSeconds = 2;

    public int STATUS;
    public void Awake()
    {

        currentState = STATE.WAIT;

        initialPos = new Vector3(gameObject.transform.localPosition);
        initialRot = gameObject.transform.localRotation;

        reset = false;

        //SCREAM
        screamTime = Animator.GetAnimationDuration(gameObject, "PD_Scream");

        //IDLE
        idleTime = Animator.GetAnimationDuration(gameObject, "PD_Idle");
    }

    public void DebugState()
    {
        STATUS = (int)currentState;
    }

    public void Update()
    {
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
                //switch (nextState)
                //{

                //}
                break;

            default:
                break;
        }
    }

    public void TriggerAction(string tag)
    {
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

                if(reset == true)
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
            Vector3 newPos = new Vector3(gameObject.transform.localPosition.x + moveSpeed * Time.deltaTime, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);
            
            gameObject.transform.localPosition = newPos;
        }
        else if (axis == 1)
        {
            Vector3 newPos = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y + moveSpeed * Time.deltaTime, gameObject.transform.localPosition.z);
            
            gameObject.transform.localPosition = newPos;
        }
        else
        {
            Vector3 newPos = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z + moveSpeed * Time.deltaTime);

            gameObject.transform.localPosition = newPos;
        }

    }
    #endregion

    #region ROTATE
    public void StartRotate()
    {
        Animator.Play(gameObject, "PD_Rotate");
    }

    public void UpdateRotate(int axis, float angle)
    {
        if (axis == 0)
        {
            currentRotation += rotateSpeed;

            if (currentRotation >= angle)
            {
                currentRotation = 0;

                SwitchState(STATE.MOVE);
            }
            else
            {
                gameObject.transform.localRotation = Quaternion.RotateAroundAxis(Vector3.right, currentRotation * Mathf.Deg2RRad);
            }

        }
        else if (axis == 1)
        {
            currentRotation += rotateSpeed;

            if (currentRotation >= angle)
            {
                currentRotation = 0;
                SwitchState(STATE.MOVE);
            }
            else
            {
                gameObject.transform.localRotation = Quaternion.RotateAroundAxis(Vector3.up, currentRotation * Mathf.Deg2RRad);
            }
        }
        else
        {
            currentRotation += rotateSpeed;

            if (currentRotation >= angle)
            {
                currentRotation = 0;

                SwitchState(STATE.MOVE);
            }
            else
            {
                gameObject.transform.localRotation = Quaternion.RotateAroundAxis(Vector3.forward, currentRotation * Mathf.Deg2RRad);
            }

        }
    }

    #endregion

    #region SCREAM
    public void StartScream()
    {
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
}