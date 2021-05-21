using System;
using System.Collections.Generic;
using DiamondEngine;

public class Bosseslv2 : Entity
{
    public NavMeshAgent agent = null;

    public Random randomNum = new Random();

    public GameObject camera = null;


    public float slerpSpeed = 5.0f;

    //Public Variables
    public float probFollow = 60.0f;
    public float probWander = 40.0f;
    public GameObject projectilePoint = null;
    public Vector3 targetPos = new Vector3(0, 0, 0);
    public float distanceProjectile = 15.0f;
    public float wanderRange = 7.5f;
    public GameObject colliderJumpSlam = null;
    public GameObject colliderBounceRush = null;
    public GameObject jumpPositionIndicator = null;

    //Private Variables
    public bool resting = false;
    public bool firstShot = true;
    public bool secondShot = false;

    //Stats
    public float healthPoints = 1920.0f;
    public float speed = 3.5f;
    public float fastRushSpeed = 14.0f;
    public float slowRushSpeed = 7.0f;
    protected float damageMult = 1f;
    public float damageRecieveMult { get; protected set; } = 1f;

    //Timers
    public float walkingTime = 4.0f;
    public float walkingTimer = 0.0f;
    public float fastChasingTime = 0.5f;
    public float fastChasingTimer = 0.0f;
    public float slowChasingTime = 3.5f;
    public float slowChasingTimer = 0.0f;
    public float shootingTime = 1.0f;
    public float shootingTimer = 0.0f;
    public float dieTime = 2.0f;
    public float dieTimer = 0.0f;
    public float restingTime = 3.0f;
    public float restingTimer = 0.0f;
    public float bounceRushTime = 4.0f;
    public float bounceRushTimer = 0.0f;
    private float currAnimationPlaySpd = 1f;
    public float presentationTimer = 0f;
    public float presentationTime = 1f;

    //Atacks
    public float projectileAngle = 30.0f;
    public float projectileRange = 6.0f;
    public float projectileDamage = 10.0f;
    public float rushDamage = 15.0f;
    public float damageBounceRush = 20f;

    //Jump Slam
    private JUMPSLAM jumpslam = JUMPSLAM.NONE;
    private float jumpslamTimer = 0.0f;
    private float chargeTime = 1f;
    private float upTime = 0.3f;
    private float fallingTime = 1.0f;
    private float recoveryTime = 0.73f;
    public float totalJumpSlamTimer = 0.0f;
    public float totalJumpSlamTime = 3.03f;

    //Bounce Rush
    private GameObject initTarget;
    private GameObject finalTarget;
    private GameObject currentTarget;
    private bool returnToInitTarget = false;
    private float startBounceRushTime = 0.0f;
    //private float startBounceRushTimer = 0.0f;

    private bool firstThrow = true;
    public GameObject companion = null;
    private bool rushOnce = true;
    enum JUMPSLAM : int
    {
        NONE = -1,
        CHARGE,
        UP,
        FALLING,
        RECOVERY
    }

    protected override void InitEntity(ENTITY_TYPE myType)
    {
        eType = myType;
        speedMult = 1f;
        myDeltaTime = Time.deltaTime;
        damageMult = 1f;
        damageRecieveMult = 1f;
    }

    public virtual void Awake()
    {
        if (gameObject.CompareTag("Skel"))
        {
            //presentationTime = Animator.GetAnimationDuration(gameObject, "Skel_Presentation") - 0.016f;
            startBounceRushTime = Animator.GetAnimationDuration(gameObject, "Skel_RushStart") - 0.016f;
            chargeTime = Animator.GetAnimationDuration(gameObject, "Skel_Jump_P1") - 0.05f;
            Debug.Log("Rush Start: " + startBounceRushTime.ToString());
            companion = InternalCalls.FindObjectWithName("WampaBoss");
        }
        else if (gameObject.CompareTag("Wampa"))
        {
            //presentationTime = Animator.GetAnimationDuration(gameObject, "Wampa_Presentation") - 0.016f;
        }
    }

    #region PROJECTILE
    public void StartProjectile()
    {
        Debug.Log("Starting Throwing Projectile");
        shootingTimer = shootingTime;
        firstShot = true;
        Animator.Play(gameObject, "WP_Projectile", speedMult);
        UpdateAnimationSpd(speedMult);
        firstThrow = true;
    }
    public void UpdateProjectile()
    {
        LookAt(Core.instance.gameObject.transform.globalPosition);
        if (shootingTimer > 0.0f)
        {
            shootingTimer -= myDeltaTime;
            if (shootingTimer < 0.0f)
            {
                if (projectilePoint != null)
                {
                    Vector3 pos = projectilePoint.transform.globalPosition;
                    Quaternion rot = new Quaternion(0, 0, 90);
                    if (firstThrow)
                        Audio.PlayAudio(gameObject, "Play_Wampa_Projectile_Throw_1");
                    else
                        Audio.PlayAudio(gameObject, "Play_Wampa_Projectile_Throw_2");
                    firstThrow = false;
                    WampaProjectile projectile = InternalCalls.CreatePrefab("Library/Prefabs/788871013.prefab", pos, rot, null).GetComponent<WampaProjectile>();

                    if (projectile != null)
                    {
                        projectile.targetPos = Core.instance.gameObject.transform.globalPosition;
                        projectile.damage = (int)(projectile.damage * damageMult);
                    }

                    //Debug.Log("Throwing projectile");

                    if (firstShot)
                    {
                        shootingTimer = shootingTime;
                        firstShot = false;
                        Animator.Play(gameObject, "WP_Projectile", speedMult);
                        UpdateAnimationSpd(speedMult);
                    }
                    else
                        secondShot = true;
                    Audio.PlayAudio(gameObject, "Play_Wampa_Ice_Pick");
                }
            }
        }
        //Debug.Log("Projectile");
        //Debug.Log(shootingTimer.ToString());
        if (projectilePoint == null) Debug.Log("Prohjectile null");

        UpdateAnimationSpd(speedMult);
    }

    public void EndProjectile()
    {
        firstShot = true;
        secondShot = false;
        resting = true;
        restingTimer = restingTime;
    }
    #endregion

    #region RUSH
    public void StartFastRush()
    {
        fastChasingTimer = fastChasingTime;
        Debug.Log("Fast Rush");
        Animator.Play(gameObject, "WP_Rush", speedMult);
        UpdateAnimationSpd(speedMult);
        if (gameObject.CompareTag("Wampa"))
            Audio.PlayAudio(gameObject, "Play_Wampa_Rush");
        else if (gameObject.CompareTag("Skel"))
            Audio.PlayAudio(gameObject, "Play_Skel_Preparation");

        if (colliderBounceRush == null)
        {
            colliderBounceRush = gameObject.GetChild("Bounce Rush");    // ... You would assume assigning a GameObject to the GameObject would put that GameObject in the GameObject... It doesn't. So I did this. I'm as confused as you are
        }

        if (colliderBounceRush != null)
        {
            colliderBounceRush.GetComponent<AtackBosslv2>().active = true;
        }
    }
    public void UpdateFastRush()
    {
        agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition);
        LookAt(agent.GetDestination());
        agent.MoveToCalculatedPos(fastRushSpeed * speedMult);
        //Debug.Log("Rush");

        UpdateAnimationSpd(speedMult);
        if (gameObject.CompareTag("Skel") && rushOnce)
        {
            Audio.PlayAudio(gameObject, "Play_Skel_In_Progress");
            rushOnce = false;
        }
    }

    public void EndFastRush()
    {
        if (gameObject.CompareTag("Wampa"))
            Audio.StopOneAudio(gameObject, "Play_Wampa_Rush");
        else if (gameObject.CompareTag("Skel"))
        {
            Audio.StopAudio(gameObject);
            Audio.PlayAudio(gameObject, "Play_Skel_Rush_Recovery");
        }
        rushOnce = true;
    }

    public void StartSlowRush()
    {
        slowChasingTimer = slowChasingTime;
        Debug.Log("Slow Rush");
        Animator.Play(gameObject, "WP_Rush", speedMult);
        UpdateAnimationSpd(speedMult);
        if (gameObject.CompareTag("Wampa"))
            Audio.PlayAudio(gameObject, "Play_Wampa_Rush");
        else if (gameObject.CompareTag("Skel"))
            Audio.PlayAudio(gameObject, "Play_Skel_Preparation");
    }
    public void UpdateSlowRush()
    {
        //Debug.Log("Slow Rush");
        agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition);
        LookAt(agent.GetDestination());
        agent.MoveToCalculatedPos(slowRushSpeed * speedMult);

        UpdateAnimationSpd(speedMult);
        if (gameObject.CompareTag("Skel") && rushOnce)
        {
            Audio.PlayAudio(gameObject, "Play_Skel_In_Progress");
            rushOnce = false;
        }
    }

    public void EndSlowRush()
    {
        resting = true;
        restingTimer = restingTime;
        if (colliderBounceRush != null)
        {
            colliderBounceRush.GetComponent<AtackBosslv2>().active = false;
        }
        if (gameObject.CompareTag("Wampa"))
            Audio.StopOneAudio(gameObject, "Play_Wampa_Rush");
        else if (gameObject.CompareTag("Skel"))
        {
            Audio.StopAudio(gameObject);
            Audio.PlayAudio(gameObject, "Play_Skel_Rush_Recovery");
        }
        rushOnce = true;
    }
    #endregion

    #region BOUNCE RUSH

    public void StartBounceRush()
    {
        if (gameObject.CompareTag("Skel"))
            Audio.PlayAudio(gameObject, "Play_Skel_Preparation");
        else if (gameObject.CompareTag("Wampa"))
            Audio.PlayAudio(gameObject, "Play_Wampa_Rush");
        if (gameObject.CompareTag("Skel"))
        {
            Animator.Play(gameObject, "Skel_Rush", speedMult);
            UpdateAnimationSpd(speedMult);
        }
        bounceRushTimer = bounceRushTime;
        GameObject nearestColumn = Level2BossRoom.columns[0];
        float nerestDistance = 10000f;
        foreach (GameObject column in Level2BossRoom.columns)
        {
            float distance = Mathf.Distance(gameObject.transform.globalPosition, column.transform.globalPosition);
            if (nerestDistance > distance)
            {
                nerestDistance = distance;
                nearestColumn = column;
            }
        }

        initTarget = nearestColumn;
        finalTarget = nearestColumn.GetComponent<TargetColumn>().GetTarget(gameObject.transform.globalPosition);
        //Debug.Log("Nearest column: " + nearestColumn.Name);
        //Debug.Log("Final target: " + finalTarget.Name);
        //Debug.Log("Started Bounce Rush");
        currentTarget = initTarget;
        returnToInitTarget = false;

        if (colliderBounceRush != null)
        {
            colliderBounceRush.GetComponent<AtackBosslv2>().active = true;
        }
    }

    public void UpdateBounceRush()
    {


        float distance = Mathf.Distance(gameObject.transform.globalPosition, currentTarget.transform.globalPosition);
        if (distance > 2f)
        {
            MoveToPosition(currentTarget.transform.globalPosition, 12f);
            LookAt(currentTarget.transform.globalPosition);
            if (currentTarget == finalTarget)
            {
                returnToInitTarget = true;
            }
        }
        else if (currentTarget != finalTarget && !returnToInitTarget)
        {
            currentTarget = finalTarget;
        }
        else if (currentTarget == finalTarget && returnToInitTarget)
        {
            currentTarget = initTarget;
        }
        //Debug.Log("Bounce Rush");

        UpdateAnimationSpd(speedMult);
        if (gameObject.CompareTag("Skel") && rushOnce)
        {
            Audio.PlayAudio(gameObject, "Play_Skel_In_Progress");
            rushOnce = false;
        }
    }

    public void EndBounceRush()
    {
        Debug.Log("END BOUNCE RUSH");
        resting = true;
        restingTimer = restingTime;

        if (gameObject.CompareTag("Skel"))
        {
            Animator.Play(gameObject, "Skel_Rush_Recover", speedMult);
            UpdateAnimationSpd(speedMult);
        }

        if (colliderBounceRush != null)
        {
            colliderBounceRush.GetComponent<AtackBosslv2>().active = false;
        }
        if (gameObject.CompareTag("Wampa"))
            Audio.StopOneAudio(gameObject, "Play_Wampa_Rush");
        else if (gameObject.CompareTag("Skel"))
        {
            Audio.StopAudio(gameObject);
            Audio.PlayAudio(gameObject, "Play_Skel_Rush_Recovery");
        }
        rushOnce = true;
    }

    #endregion

    #region JUMP SLAM

    public void StartJumpSlam()
    {
        Audio.PlayAudio(gameObject, "Play_Wampa_Skell_Jump_Snow");
        Debug.Log("Starting Jumping");
        jumpslam = JUMPSLAM.CHARGE;
        jumpslamTimer = chargeTime;
        totalJumpSlamTimer = totalJumpSlamTime;
        if (gameObject.CompareTag("Skel"))
        {
            Animator.Play(gameObject, "Skel_Jump_P1", speedMult);
            UpdateAnimationSpd(speedMult);
        }
    }

    public void UpdateJumpSlam()
    {
        LookAt(Core.instance.gameObject.transform.globalPosition);
        //Debug.Log("Jump Slam");
        switch (jumpslam)
        {
            case JUMPSLAM.CHARGE:
                //Debug.Log("Jump Slam: Charge");
                if (jumpslamTimer > 0)
                {
                    jumpslamTimer -= myDeltaTime;

                    if (jumpslamTimer <= 0)
                    {
                        jumpslamTimer = upTime;
                        jumpslam = JUMPSLAM.UP;

                        if (gameObject.CompareTag("Skel"))
                        {
                            Animator.Play(gameObject, "Skel_Jump_P2", speedMult);
                            UpdateAnimationSpd(speedMult);

                            if (jumpPositionIndicator != null)
                            {
                                jumpPositionIndicator.Enable(true);
                                jumpPositionIndicator.transform.localPosition = Core.instance.gameObject.transform.globalPosition;
                            }

                        }
                        else if (gameObject.CompareTag("Wampa"))
                        {

                        }

                    }
                }

                //Audio.PlayAudio(gameObject, "Play_Wampa_Skell_Jump_Snow");
                break;

            case JUMPSLAM.UP:
                //Debug.Log("Jump Slam: Up");
                if (jumpslamTimer > 0)
                {
                    gameObject.transform.localPosition += Vector3.up * 50f * myDeltaTime;
                    Vector3 positionVec = (Core.instance.gameObject.transform.globalPosition - gameObject.transform.localPosition) * myDeltaTime;
                    gameObject.transform.localPosition += new Vector3(positionVec.x, 0.0f, positionVec.z);

                    jumpslamTimer -= myDeltaTime;

                    if (Mathf.Distance(jumpPositionIndicator.transform.localPosition, Core.instance.gameObject.transform.globalPosition) > 0.3f) {
                        jumpPositionIndicator.transform.localPosition += (Core.instance.gameObject.transform.globalPosition - jumpPositionIndicator.transform.localPosition) * 0.1f;
                    }

                    if (jumpslamTimer <= 0)
                    {
                        jumpslamTimer = fallingTime;
                        jumpslam = JUMPSLAM.FALLING;
                        jumpPositionIndicator.transform.localPosition = targetPos = Core.instance.gameObject.transform.globalPosition;

                        float speed = Mathf.Distance(targetPos, gameObject.transform.globalPosition) / fallingTime;
                        if (colliderJumpSlam != null)
                        {
                            colliderJumpSlam.GetComponent<AtackBosslv2>().active = true;
                        }
                    }
                }
                break;

            case JUMPSLAM.FALLING:
                //Debug.Log("Jump Slam: Falling");
                if (jumpslamTimer > 0)
                {
                    MoveToPosition(targetPos, speed * 10);
                    jumpslamTimer -= myDeltaTime;

                    if (jumpslamTimer <= 0 || Mathf.Distance(targetPos, gameObject.transform.globalPosition) <= 0.1f)
                    {
                        jumpslamTimer = recoveryTime;
                        jumpslam = JUMPSLAM.RECOVERY;

                        if (gameObject.CompareTag("Skel"))
                        {
                            Animator.Play(gameObject, "Skel_Jump_P3", speedMult);
                            UpdateAnimationSpd(speedMult);
                        }
                        else if (gameObject.CompareTag("Wampa"))
                        {

                        }

                        Audio.PlayAudio(gameObject, "Play_Wampa_Skell_Jump_Impact");
                    }
                }
                //Audio.PlayAudio(gameObject, "Play_Wampa_Skell_Jump_Impact");
                break;

            case JUMPSLAM.RECOVERY:
                //Debug.Log("Jump Slam: Recovery");
                if (jumpslamTimer > 0)
                {
                    jumpslamTimer -= myDeltaTime;

                    if (jumpslamTimer <= 0)
                    {
                        jumpslam = JUMPSLAM.NONE;
                    }
                }
                break;

            case JUMPSLAM.NONE:
            default:
                Debug.Log("Jump slam motion ended");
                break;
        }

        UpdateAnimationSpd(speedMult);
    }

    public void EndJumpSlam()
    {
        resting = true;
        restingTimer = restingTime;

        if (jumpPositionIndicator != null)
        {
            jumpPositionIndicator.Enable(false);
        }

        if (colliderJumpSlam != null)
        {
            colliderJumpSlam.GetComponent<AtackBosslv2>().active = false;
        }

    }

    #endregion

    #region FOLLOW
    public void StartFollowing()
    {
        walkingTimer = walkingTime;
        if (gameObject.CompareTag("Skel"))
        {
            Animator.Play(gameObject, "Skel_Walk", speedMult);
            UpdateAnimationSpd(speedMult);
            Audio.PlayAudio(gameObject, "Play_Skel_Footsteps");
        }
        else if (gameObject.CompareTag("Wampa"))
        {
            Animator.Play(gameObject, "WP_Walk", speedMult);
            UpdateAnimationSpd(speedMult);
            Audio.PlayAudio(gameObject, "Play_Wampa_Footsteps");
        }
    }
    public void UpdateFollowing()
    {
        agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition);
        LookAt(agent.GetDestination());
        agent.MoveToCalculatedPos(speed * speedMult);
        //Debug.Log("Following player");

        UpdateAnimationSpd(speedMult);
    }

    public void EndFollowing()
    {
        Audio.StopAudio(gameObject);
    }
    #endregion

    #region WANDER
    public void StartWander()
    {
        walkingTimer = walkingTime;
        agent.CalculateRandomPath(gameObject.transform.globalPosition, wanderRange);
        if (gameObject.CompareTag("Skel"))
        {
            Animator.Play(gameObject, "Skel_Walk", speedMult);
            UpdateAnimationSpd(speedMult);
            Audio.PlayAudio(gameObject, "Play_Skel_Footsteps");
        }
        else if (gameObject.CompareTag("Wampa"))
        {

            Animator.Play(gameObject, "WP_Walk", speedMult);
            UpdateAnimationSpd(speedMult);
            Audio.PlayAudio(gameObject, "Play_Wampa_Footsteps");
        }
    }
    public void UpdateWander()
    {
        LookAt(agent.GetDestination());
        agent.MoveToCalculatedPos(speed * speedMult);
        //Debug.Log("Following player");

        UpdateAnimationSpd(speedMult);
    }

    public void EndWander()
    {
        Audio.StopAudio(gameObject);
    }
    #endregion

    #region DIE
    public void StartDie()
    {
        dieTimer = dieTime;
        if (gameObject.CompareTag("Skel"))
        {
            Animator.Play(gameObject, "Skel_Die", speedMult);
            UpdateAnimationSpd(speedMult);
            Audio.PlayAudio(gameObject, "Play_Skel_Death");
            Counter.SumToCounterType(Counter.CounterTypes.SKEL);
            Debug.Log("BOOLEAN IS TRUE");
        }
        else if (gameObject.CompareTag("Wampa"))
        {
            Animator.Play(gameObject, "WP_Die", speedMult);
            UpdateAnimationSpd(speedMult);
            Audio.PlayAudio(gameObject, "Play_Wampa_Death");
            Counter.SumToCounterType(Counter.CounterTypes.WAMPA);
        }
        Debug.Log("Dying");
    }
    public void UpdateDie()
    {
        if (dieTimer > 0.0f)
        {
            dieTimer -= myDeltaTime;

            if (dieTimer <= 0.0f)
            {
                EndDie();
            }
        }
        //Debug.Log("Dying");

        UpdateAnimationSpd(speedMult);
    }

    public void EndDie()
    {
        Debug.Log("DEAD");


        EnemyManager.RemoveEnemy(gameObject);
        if (gameObject.CompareTag("Wampa") && companion != null)
        {
            companion.GetComponent<Skel>().firstSorrowRoar = true;
        }
        else if (gameObject.CompareTag("Skel") && companion != null)
        {
            companion.GetComponent<Wampa>().firstSorrowRoar = true;
        }
        gameObject.Enable(false);
        //companion = null;
        //InternalCalls.Destroy(gameObject);
    }
    #endregion

    #region PRESENTATION
    public void StartPresentation()
    {
        presentationTimer = presentationTime;

        if (gameObject.CompareTag("Skel"))
        {
            Animator.Play(gameObject, "Skel_Presentation", speedMult);
            UpdateAnimationSpd(speedMult);
            //Audio.PlayAudio(gameObject, "Play_Skel_Death");
        }
        else if (gameObject.CompareTag("Wampa"))
        {
            Animator.Play(gameObject, "WP_Presentation", speedMult);
            UpdateAnimationSpd(speedMult);
            //Audio.PlayAudio(gameObject, "Play_Wampa_Death");
        }

        if (camera != null)
        {
            Shake3D shake = camera.GetComponent<Shake3D>();
            if (shake != null)
            {
                shake.StartShaking(1.5f, 0.2f);
                Input.PlayHaptic(0.9f, 800);
            }
        }

        Debug.Log("Start Presentation");
    }
    public void UpdatePresentation()
    {

        Debug.Log("Presentation");

        UpdateAnimationSpd(speedMult);
    }

    public void EndPresentation()
    {

    }
    #endregion

    public void LookAt(Vector3 pointToLook)
    {
        Vector3 direction = pointToLook - gameObject.transform.globalPosition;
        direction = direction.normalized;
        float angle = (float)Math.Atan2(direction.x, direction.z);

        if (Math.Abs(angle * Mathf.Rad2Deg) < 1.0f)
            return;

        Quaternion dir = Quaternion.RotateAroundAxis(Vector3.up, angle);

        float rotationSpeed = myDeltaTime * slerpSpeed;

        Quaternion desiredRotation = Quaternion.Slerp(gameObject.transform.localRotation, dir, rotationSpeed);

        gameObject.transform.localRotation = desiredRotation;
    }

    public void MoveToPosition(Vector3 positionToReach, float speed)
    {
        Vector3 direction = positionToReach - gameObject.transform.localPosition;

        gameObject.transform.localPosition += direction.normalized * speed * myDeltaTime;
    }

    private void PlayAnimation(string animName)
    {
        if (gameObject.CompareTag("Skel"))
        {
            Animator.Play(gameObject, animName, speedMult);
            UpdateAnimationSpd(speedMult);
        }
        else if (gameObject.CompareTag("Wampa"))
        {
            Animator.Play(gameObject, animName, speedMult);
            UpdateAnimationSpd(speedMult);
        }
    }

    private void UpdateAnimationSpd(float newSpd)
    {
        if (currAnimationPlaySpd != newSpd)
        {
            Animator.SetSpeed(gameObject, newSpd);
            currAnimationPlaySpd = newSpd;
        }
    }

    protected override void OnInitStatus(ref StatusData statusToInit)
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
            case STATUS_TYPE.ENEMY_DAMAGE_DOWN:
                {
                    this.damageMult -= statusToInit.severity;
                }
                break;
            case STATUS_TYPE.ENEMY_VULNERABLE:
                {
                    this.damageRecieveMult += statusToInit.severity;
                }
                break;
            default:
                break;
        }
    }

    protected override void OnDeleteStatus(StatusData statusToDelete)
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
            case STATUS_TYPE.ENEMY_DAMAGE_DOWN:
                {
                    this.damageMult += statusToDelete.severity;
                }
                break;
            case STATUS_TYPE.ENEMY_VULNERABLE:
                {
                    this.damageRecieveMult -= statusToDelete.severity;
                }
                break;
            default:
                break;
        }
    }


}