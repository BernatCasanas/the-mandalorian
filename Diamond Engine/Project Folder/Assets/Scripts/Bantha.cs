using System;
using DiamondEngine;

public class Bantha : Enemy
{
	public float chargeSpeed = 60.0f;
	public float chargeRange = 25.0f;
	public float chargeTime = 1.5f;
	public float chargeDuration = 1.0f;
    

    private float chargeCounter = 0.0f;
	private float chargeStartYPos = 0.0f;

	public void Start()
	{
		currentState = STATES.WANDER;
		targetPosition = CalculateNewPosition(wanderRange);
		shotTimes = 0;
	}
	public void Update()
	{
        if (player == null)
        {
            return;
        }
        switch (currentState)
        {
			case STATES.IDLE:
				Debug.Log("Idle");

				timePassed += Time.deltaTime;

				if (InRange(player.transform.globalPosition, range))
				{
					LookAt(player.transform.globalPosition);

					if (timePassed > idleTime)
					{
						currentState = STATES.SHOOT;
						//timePassed = timeBewteenShots;
					}
				}
				else
				{
					if (timePassed > idleTime)
					{
						currentState = STATES.WANDER;
						timePassed = 0.0f;
						targetPosition = CalculateNewPosition(wanderRange);
					}
				}
				break;

			case STATES.RUN:
				Debug.Log("Running");

				LookAt(player.transform.globalPosition);
				MoveToPosition(player.transform.localPosition, runningSpeed);

				// If the player is in range attack him
				if (InRange(player.transform.globalPosition, chargeRange))
				{
					currentState = STATES.SHOOT;
					//timePassed = timeBewteenShots;
				}

				if (Mathf.Distance(gameObject.transform.localPosition, player.transform.localPosition) < stoppingDistance)
				{
					currentState = STATES.IDLE;
					timePassed = 0.0f;
				}
				break;

			case STATES.WANDER:
				Debug.Log("Wander");

				// If the player is in range run to him
				if (InRange(player.transform.globalPosition, range))
				{
					currentState = STATES.RUN;
					//timePassed = timeBewteenShots;
				}
				else  //if not, keep wandering
				{
					if (targetPosition == null)
						targetPosition = CalculateNewPosition(wanderRange);

					LookAt(targetPosition);
					MoveToPosition(targetPosition, wanderSpeed);

					if (Mathf.Distance(gameObject.transform.localPosition, targetPosition) < stoppingDistance)
					{
						//targetPosition = CalculateNewPosition(wanderRange);
						currentState = STATES.IDLE;
						timePassed = 0.0f;
					}
				}
				break;

			case STATES.SHOOT:
				Debug.Log("Charging");

				timePassed += Time.deltaTime;

                //LookAt(player.transform.globalPosition);

                //Debug.Log(player.transform.localPosition.ToString());
                //Debug.Log(gameObject.transform.localPosition.ToString());

                if (timePassed < chargeTime)
                {
					LookAt(player.transform.globalPosition);
                }
                else
                {
					if (chargeCounter < chargeDuration)
					{
						chargeCounter += Time.deltaTime;
						gameObject.SetVelocity(gameObject.transform.GetForward().normalized * chargeSpeed);

						

					}
					else
					{
						

						chargeCounter = 0.0f;
						currentState = STATES.RUN;
						timePassed = 0.0f;
					}
				}

				if (Mathf.Distance(gameObject.transform.localPosition, player.transform.localPosition) < stoppingDistance)
				{
					chargeCounter = 0.0f;
					currentState = STATES.RUN;
					timePassed = 0.0f;
				}
				
				break;

			case STATES.HIT:
				Debug.Log("Being Hit");

				break;

			case STATES.DIE:
				Debug.Log("Dying");
				Counter.SumToCounterType(Counter.CounterTypes.ENEMY_BANTHA);
				Counter.roomEnemies--;
				if (Counter.roomEnemies <= 0 && Core.instance != null)
				{
                    Core.instance.gameObject.GetComponent<BoonSpawn>().SpawnBoons();
                }
                InternalCalls.Destroy(gameObject);
				break;
		}
	}
    public void OnCollisionEnter(GameObject collidedGameObject)
    {
       
        if (collidedGameObject.CompareTag("Bullet"))
        {
            Debug.Log("Collision bullet");
            healthPoints -= collidedGameObject.GetComponent<BH_Bullet>().damage;
            if (currentState != STATES.DIE && healthPoints <= 0.0f)
            {
                currentState = STATES.DIE;
                timePassed = 0.0f;
                Animator.Play(gameObject, "ST_Die", 1.0f);
                //Play Sound("Die")
                Audio.PlayAudio(gameObject, "Play_Stormtrooper_Death");
                Audio.PlayAudio(gameObject, "Play_Mando_Voice");

                RemoveFromSpawner();

                if (Core.instance.hud != null)
                {
                    Core.instance.hud.GetComponent<HUD>().IncrementCombo(1, 1.0f);
                }
            }
        }
        else if (collidedGameObject.CompareTag("Grenade"))
        {
            Debug.Log("Collision Grenade");

            if (currentState != STATES.DIE)
            {
                currentState = STATES.DIE;
                timePassed = 0.0f;
                Animator.Play(gameObject, "ST_Die", 1.0f);
                //Play Sound("Die")
                Audio.PlayAudio(gameObject, "Play_Stormtrooper_Death");
                Audio.PlayAudio(gameObject, "Play_Mando_Voice");

                RemoveFromSpawner();


                if (Core.instance.hud != null)
                {
                    Core.instance.hud.GetComponent<HUD>().IncrementCombo(1, 2.0f);
                }
            }
        }
        else if (collidedGameObject.CompareTag("WorldLimit"))
        {
            Debug.Log("Collision w/ The End");

            if (currentState != STATES.DIE)
            {
                currentState = STATES.DIE;
                timePassed = 0.0f;
                Animator.Play(gameObject, "ST_Die", 1.0f);
                Audio.PlayAudio(gameObject, "Play_Stormtrooper_Death");
            }
        }

    }
}