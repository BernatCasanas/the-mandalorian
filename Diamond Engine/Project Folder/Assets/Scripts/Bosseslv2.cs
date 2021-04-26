using System;
using System.Collections.Generic;
using DiamondEngine;

public class Bosseslv2 : DiamondComponent
{
    enum BOSS
    {
        WAMPA,
        SKEL
    }

    public NavMeshAgent agent = null;

    public Random randomNum = new Random();


    public float slerpSpeed = 5.0f;

    //Public Variables
    public float probFollow = 60.0f;
    public float probWander = 40.0f;
    public GameObject projectilePoint = null;
    public Vector3 targetPos = new Vector3(0, 0, 0);
    public float distanceProjectile = 15.0f;
    public float wanderRange = 7.5f;
    public GameObject colliderJumpSlam = null;


    //Private Variables
    public bool resting = false;
    public bool firstShot = true;
    public bool secondShot = false;

    //Stats
    public float healthPoints = 1920.0f;
    public float speed = 3.5f;
    public float fastRushSpeed = 14.0f;
    public float slowRushSpeed = 7.0f;

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
    private float startBounceRushTimer = 0.0f;

    enum JUMPSLAM : int
    {
        NONE = -1,
        CHARGE,
        UP,
        FALLING,
        RECOVERY
    }

    public virtual void Awake()
    {
        if (gameObject.CompareTag("Skel"))
        {
            startBounceRushTime = Animator.GetAnimationDuration(gameObject, "Skel_RushStart") - 0.016f;
            Debug.Log("Rush Start: " + startBounceRushTime.ToString());
        }
    }

    #region PROJECTILE
    public void StartProjectile()
    {
        Debug.Log("Starting Throwing Projectile");
        shootingTimer = shootingTime;
        firstShot = true;
        Animator.Play(gameObject, "WP_Projectile");
    }
    public void UpdateProjectile()
    {
        LookAt(Core.instance.gameObject.transform.globalPosition);
        if (shootingTimer > 0.0f)
        {
            shootingTimer -= Time.deltaTime;
            if (shootingTimer < 0.0f)
            {
                if (projectilePoint != null)
                {
                    Vector3 pos = projectilePoint.transform.globalPosition;
                    Quaternion rot = new Quaternion(0,0,90);

                    GameObject projectile = InternalCalls.CreatePrefab("Library/Prefabs/788871013.prefab", pos, rot, null);
                    projectile.GetComponent<RancorProjectile>().targetPos = Core.instance.gameObject.transform.globalPosition;
                    Debug.Log("Throwing projectile");

                    if (firstShot)
                    {
                        shootingTimer = shootingTime;
                        firstShot = false;
                        Animator.Play(gameObject, "WP_Projectile");
                    }
                    else
                        secondShot = true;
                }
            }
        }
        Debug.Log("Projectile");
        Debug.Log(shootingTimer.ToString());
        if (projectilePoint == null) Debug.Log("Prohjectile null");

    }

    public void EndProjectile()
    {
        firstShot = true;
        secondShot = false;
        resting = true;
        restingTimer = restingTime;
        Audio.PlayAudio(gameObject, "Play_Wampa_Skell_Ice_Impact");
    }
    #endregion

    #region RUSH
    public void StartFastRush()
    {
        fastChasingTimer = fastChasingTime;
        Debug.Log("Fast Rush");
        Animator.Play(gameObject, "WP_Rush");
        Audio.PlayAudio(gameObject, "Play_Wampa_Skell_Previous_Rush_Roar");
    }
    public void UpdateFastRush()
    {
        agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition);
        LookAt(agent.GetDestination());
        agent.MoveToCalculatedPos(fastRushSpeed);
        Debug.Log("Rush");
    }

    public void EndFastRush()
    {

    }

    public void StartSlowRush()
    {
        slowChasingTimer = slowChasingTime;
        Debug.Log("Slow Rush");
        Animator.Play(gameObject, "WP_Rush");
    }
    public void UpdateSlowRush()
    {
        Debug.Log("Slow Rush");
        agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition);
        LookAt(agent.GetDestination());
        agent.MoveToCalculatedPos(slowRushSpeed);
    }

    public void EndSlowRush()
    {
        resting = true;
        restingTimer = restingTime;
    }
    #endregion

    #region RUSH STUN
    public void StartRushStun()
    {

    }
    public void UpdateRushStun()
    {
        Debug.Log("Rush Stun");
    }

    public void EndRushStun()
    {
        resting = true;
        restingTimer = restingTime;
    }
    #endregion

    #region BOUNCE RUSH

    public void StartBounceRush()
    {
        if (gameObject.CompareTag("Skel"))
        {
            Animator.Play(gameObject, "Skel_Rush");
        }
        bounceRushTimer = bounceRushTime;
        GameObject nearestColumn = Level2BossRoom.columns[0];
        float nerestDistance = 10000f;
        foreach (GameObject column in Level2BossRoom.columns)
        {
            float distance = Mathf.Distance(gameObject.transform.globalPosition, column.transform.globalPosition);
            if(nerestDistance > distance)
            {
                nerestDistance = distance;
                nearestColumn = column;
            }
        }

        initTarget = nearestColumn;
        finalTarget = nearestColumn.GetComponent<TargetColumn>().GetTarget(gameObject.transform.globalPosition);
        Debug.Log("Nearest column: " + nearestColumn.Name);
        Debug.Log("Final target: " + finalTarget.Name);
        Debug.Log("Started Bounce Rush");
        currentTarget = initTarget;
        returnToInitTarget = false;
    }

    public void UpdateBounceRush()
    {
        float distance = Mathf.Distance(gameObject.transform.globalPosition, currentTarget.transform.globalPosition);
        if (distance > 2f)
        {
            MoveToPosition(currentTarget.transform.globalPosition, 12f);
            LookAt(currentTarget.transform.globalPosition);
            if(currentTarget == finalTarget)
            {
                returnToInitTarget = true;
            }
        }
        else if(currentTarget != finalTarget && !returnToInitTarget)
        {
            currentTarget = finalTarget;
        }else if(currentTarget == finalTarget && returnToInitTarget)
        {
            currentTarget = initTarget;
        }
        Debug.Log("Bounce Rush");
    }

    public void EndBounceRush()
    {
        Debug.Log("END BOUNCE RUSH");
        resting = true;
        restingTimer = restingTime;
        if (gameObject.CompareTag("Skel")) {
            Animator.Play(gameObject,"Skel_Rush_Recover");
        }
    }

    #endregion

    #region JUMP SLAM

    public void StartJumpSlam()
    {
        Debug.Log("Starting Jumping");
        jumpslam = JUMPSLAM.CHARGE;
        jumpslamTimer = chargeTime;
        totalJumpSlamTimer = totalJumpSlamTime;
    }

    public void UpdateJumpSlam()
    {
        Debug.Log("Jump Slam");
        switch (jumpslam)
        {
            case JUMPSLAM.CHARGE:
                Debug.Log("Jump Slam: Charge");
                if (jumpslamTimer > 0)
                {
                    jumpslamTimer -= Time.deltaTime;

                    if (jumpslamTimer <= 0)
                    {
                        jumpslamTimer = upTime;
                        jumpslam = JUMPSLAM.UP;
                    }
                }

                Audio.PlayAudio(gameObject, "Play_Wampa_Skell_Jump_Snow");
                break;

            case JUMPSLAM.UP:
                Debug.Log("Jump Slam: Up");
                if (jumpslamTimer > 0)
                {
                    gameObject.transform.localPosition += Vector3.up * 50f * Time.deltaTime;

                    jumpslamTimer -= Time.deltaTime;

                    if (jumpslamTimer <= 0)
                    {
                        jumpslamTimer = fallingTime;
                        jumpslam = JUMPSLAM.FALLING;
                        targetPos = Core.instance.gameObject.transform.globalPosition;
                        float speed = Mathf.Distance(targetPos, gameObject.transform.globalPosition) / fallingTime;
                        if (colliderJumpSlam != null)
                        {
                            colliderJumpSlam.EnableCollider();
                        }
                    }
                }
                break;

            case JUMPSLAM.FALLING:
                Debug.Log("Jump Slam: Falling");
                if (jumpslamTimer > 0)
                {
                    MoveToPosition(targetPos, speed * 10);
                    jumpslamTimer -= Time.deltaTime;

                    if (jumpslamTimer <= 0 || Mathf.Distance(targetPos, gameObject.transform.globalPosition) <= 0.1f)
                    {
                        jumpslamTimer = recoveryTime;
                        jumpslam = JUMPSLAM.RECOVERY;
                        colliderJumpSlam.DisableCollider();
                    }
                }
                Audio.PlayAudio(gameObject, "Play_Wampa_Skell_Jump_Impact");
                break;

            case JUMPSLAM.RECOVERY:
                Debug.Log("Jump Slam: Recovery");
                if (jumpslamTimer > 0)
                {
                    jumpslamTimer -= Time.deltaTime;

                    if (jumpslamTimer <= 0)
                    {
                        jumpslam = JUMPSLAM.NONE;
                    }
                }
                break;

            case JUMPSLAM.NONE:
            default:
                Debug.Log("Something gone wrong with jump slam");
                break;
        }
    }

    public void EndJumpSlam()
    {
        resting = true;
        restingTimer = restingTime;
    }

    #endregion

    #region FOLLOW
    public void StartFollowing()
    {
        walkingTimer = walkingTime;
        if (gameObject.CompareTag("Skel"))
        {
            Animator.Play(gameObject, "Skel_Walk");
        }
        else if (gameObject.CompareTag("Wampa"))
        {

            Animator.Play(gameObject, "WP_Walk");
        }
        Audio.PlayAudio(gameObject, "PLay_Rancor_Footsteps");
    }
    public void UpdateFollowing()
    {
        agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition);
        LookAt(agent.GetDestination());
        agent.MoveToCalculatedPos(speed);
        Debug.Log("Following player");
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
        Animator.Play(gameObject, "WP_Walk");
        Audio.PlayAudio(gameObject, "PLay_Rancor_Footsteps");
    }
    public void UpdateWander()
    {
        LookAt(agent.GetDestination());
        agent.MoveToCalculatedPos(speed);
        Debug.Log("Following player");
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
        Animator.Play(gameObject, "WP_Die");
        Audio.PlayAudio(gameObject, "Play_Wampa_Skell_Death_Roar");
        Debug.Log("Dying");
    }
    public void UpdateDie()
    {
        if (dieTimer > 0.0f)
        {
            dieTimer -= Time.deltaTime;

            if (dieTimer <= 0.0f)
            {
                EndDie();
            }
        }
        Debug.Log("Dying");
    }

    public void EndDie()
    {
        Debug.Log("DEAD");

        Counter.roomEnemies--;

        if (Counter.roomEnemies <= 0)
            Counter.allEnemiesDead = true;

        EnemyManager.RemoveEnemy(gameObject);
        InternalCalls.Destroy(gameObject);
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

        float rotationSpeed = Time.deltaTime * slerpSpeed;

        Quaternion desiredRotation = Quaternion.Slerp(gameObject.transform.localRotation, dir, rotationSpeed);

        gameObject.transform.localRotation = desiredRotation;
    }

    public void MoveToPosition(Vector3 positionToReach, float speed)
    {
        Vector3 direction = positionToReach - gameObject.transform.localPosition;

        gameObject.transform.localPosition += direction.normalized * speed * Time.deltaTime;
    }

    private void PlayAnimation(string animName)
    {
        if (gameObject.CompareTag("Skel"))
        {
            Animator.Play(gameObject, animName);
        }
        else if (gameObject.CompareTag("Wampa"))
        {
            Animator.Play(gameObject, animName);
        }
    }
}