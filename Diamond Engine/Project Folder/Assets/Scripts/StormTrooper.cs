using System;
using DiamondEngine;

public class StormTrooper : Enemy
{
	public GameObject hitParticles;

	private int shotSequences = 0;
	public int maxShots = 2;
	public int maxSequences = 2;

	private float pushSkillTimer = 0.15f;
	private float pushSkillSpeed = 0.2f;
    public float stormTrooperDamage = 5.0f;
    

	public void Start()
	{
		currentState = STATES.IDLE;
		targetPosition = CalculateNewPosition(wanderRange);
		shotTimes = 0;
        stormTrooperDamage = 1.0f;
   
	}

	public void Update()
	{
		if (player == null)
        {
			Debug.Log("Null player");
			player = Core.instance.gameObject;
        }

		switch (currentState)
		{
			case STATES.IDLE:
				//Debug.Log("Idle");

				timePassed += Time.deltaTime;

				if (InRange(player.transform.globalPosition, range))
				{
					LookAt(player.transform.globalPosition);

					if(timePassed > idleTime)
                    {
						currentState = STATES.SHOOT;
						timePassed = timeBewteenShots;					
					}
				}
				else
                {
					if (timePassed > idleTime && !turretMode)
					{
						currentState = STATES.WANDER;
						timePassed = 0.0f;
						targetPosition = CalculateNewPosition(wanderRange);
                        if (shotSequences == 1)
                        {
							currentState = STATES.SHOOT;
                        }
						else
                        {
							Animator.Play(gameObject, "ST_Run");
							Audio.StopAudio(this.gameObject);
						}
					}
				}

				break;

			case STATES.RUN:
				//Debug.Log("Run");

				LookAt(targetPosition);
				MoveToPosition(targetPosition, runningSpeed);

				if (Mathf.Distance(gameObject.transform.localPosition, targetPosition) < stoppingDistance)
				{
					Animator.Play(gameObject, "ST_Idle");
					currentState = STATES.IDLE;
					timePassed = 0.0f;
					Audio.StopAudio(this.gameObject);
				}

				break;

			case STATES.WANDER:

				//Debug.Log("Wander");

				if (player == null)
					Debug.Log("Null player");

				// If the player is in range attack him
				if (InRange(player.transform.globalPosition, range))
				{
					currentState = STATES.SHOOT;
					Animator.Play(gameObject, "ST_Shoot");
					timePassed = timeBewteenShots;
					//Play Sound("Shoot")
					Audio.PlayAudio(gameObject, "Play_Blaster_Stormtrooper");
				}
				else if(!turretMode) //if not, keep wandering
				{
					if (targetPosition == null)
                    {
						targetPosition = CalculateNewPosition(wanderRange);
						Animator.Play(gameObject, "ST_Run");
						Audio.PlayAudio(gameObject, "Play_Footsteps_Stormtrooper");
					}

					LookAt(targetPosition);
					MoveToPosition(targetPosition, wanderSpeed);

					if (Mathf.Distance(gameObject.transform.globalPosition, targetPosition) < stoppingDistance)
					{
						//targetPosition = CalculateNewPosition(wanderRange);
						currentState = STATES.IDLE;
						Animator.Play(gameObject, "ST_Idle");
						timePassed = 0.0f;
						Audio.StopAudio(this.gameObject);
					}
				}
				else
                {
					currentState = STATES.IDLE;
					Animator.Play(gameObject, "ST_Idle");
					timePassed = 0.0f;
					Audio.StopAudio(this.gameObject);
				}
				
				break;

			case STATES.SHOOT:
				//Debug.Log("Shoot");
				
				timePassed += Time.deltaTime;

				LookAt(player.transform.globalPosition);

				if (timePassed > timeBewteenShots)
				{
					Shoot(stormTrooperDamage);
					//Play Sound("Shoot")
					Animator.Play(gameObject, "ST_Shoot");
					Audio.PlayAudio(gameObject, "PLay_Blaster_Stormtrooper");

					if (shotTimes >= maxShots)
					{
						shotTimes = 0;
						shotSequences++;
						
						if (shotSequences >= maxSequences && !turretMode)
						{
							currentState = STATES.RUN;
							Animator.Play(gameObject, "ST_Run");
							targetPosition = CalculateNewPosition(runningRange);
							shotSequences = 0;
							//Play Sound("Run")
							Audio.PlayAudio(gameObject, "Play_Footsteps_Stormtrooper");
						}
						else
						{
							Animator.Play(gameObject, "ST_Idle");
							currentState = STATES.IDLE;
							Audio.StopAudio(this.gameObject);
						}
					}
				}
				break;

			case STATES.PUSHED:
				Vector3 pushDirection = Core.instance.gameObject.transform.localPosition - gameObject.transform.localPosition;
				pushDirection = pushDirection.normalized;

				pushDirection.y = 0.0f;

				gameObject.transform.localPosition -= pushDirection * pushSkillSpeed;

				timePassed += Time.deltaTime;

                if (timePassed >= pushSkillTimer)
                {
					currentState = STATES.WANDER;
					timePassed = 0.0f;
				}

				break;

			case STATES.HIT:
				break;

			case STATES.DIE:
				//Debug.Log("ST_Die");

				timePassed += Time.deltaTime;

				if(timePassed > 1.2f)
                {
					Counter.SumToCounterType(Counter.CounterTypes.ENEMY_STORMTROOP);
					Counter.roomEnemies--;
					Debug.Log("Enemies: " + Counter.roomEnemies.ToString());
					if (Counter.roomEnemies <= 0)
					{
                       Core.instance.gameObject.GetComponent<BoonSpawn>().SpawnBoons();
                    }
					player.GetComponent<PlayerHealth>().TakeDamage(-PlayerHealth.healWhenKillingAnEnemy);
					InternalCalls.Destroy(gameObject);
                }
				break;
		}
		
	}

	public void OnCollisionEnter(GameObject collidedGameObject)
	{
		//Debug.Log("CS: Collided object: " + gameObject.tag + ", Collider: " + collidedGameObject.tag);
		//Debug.Log("Collided by tag: " + collidedGameObject.tag);

		if (collidedGameObject.CompareTag("Bullet"))
		{
			Debug.Log("Collision bullet");
            healthPoints -= collidedGameObject.GetComponent<BH_Bullet>().damage;
            if (currentState != STATES.DIE && healthPoints <= 0.0f )
            {
				currentState = STATES.DIE;
				timePassed = 0.0f;
				Animator.Play(gameObject, "ST_Die", 1.0f);
				//Play Sound("Die")
				Audio.PlayAudio(gameObject, "Play_Stormtrooper_Death");
				Audio.PlayAudio(gameObject, "Play_Mando_Voice");

				if(hitParticles != null)
				hitParticles.GetComponent<ParticleSystem>().Play();

				RemoveFromSpawner();

				if (Core.instance.hud != null)
                {
					Core.instance.hud.GetComponent<HUD>().AddToCombo(20, 1.0f);
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
					Core.instance.hud.GetComponent<HUD>().AddToCombo(20, 0.5f);
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
				RemoveFromSpawner();
			}
		}



	}

	
	public void OnTriggerEnter(GameObject triggeredGameObject)
	{
		//Debug.Log("CS: Collided object: " + gameObject.tag + ", Collider: " + triggeredGameObject.tag);
		if (triggeredGameObject.CompareTag("PushSkill"))
		{
			Debug.Log("Skill collision");

            if (currentState != STATES.DIE && currentState != STATES.PUSHED)
            {
				currentState = STATES.PUSHED;
				timePassed = 0.0f;
            }
		}


		//Debug.Log("Triggered by tag: " + triggeredGameObject.tag);
	}

   
}