using System;
using DiamondEngine;

public class StormTrooper : Enemy
{
	private int shotSequences = 0;
	public int maxShots = 2;
	public int maxSequences = 2;

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
			player = Scene.FindObjectWithTag("Player");
			
			if (player == null)
				Debug.Log("Null player");
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
					if (timePassed > idleTime)
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
				}
				else  //if not, keep wandering
				{
					if (targetPosition == null)
                    {
						targetPosition = CalculateNewPosition(wanderRange);
						Animator.Play(gameObject, "ST_Run");
                    }

					LookAt(targetPosition);
					MoveToPosition(targetPosition, wanderSpeed);

					if (Mathf.Distance(gameObject.transform.localPosition, targetPosition) < stoppingDistance)
					{
						//targetPosition = CalculateNewPosition(wanderRange);
						currentState = STATES.IDLE;
						Animator.Play(gameObject, "ST_Idle");
						timePassed = 0.0f;
					}
				}
				
				break;

			case STATES.SHOOT:
				//Debug.Log("Shoot");
				
				timePassed += Time.deltaTime;

				LookAt(player.transform.globalPosition);

				if (timePassed > timeBewteenShots)
				{
					Shoot();
					Animator.Play(gameObject, "ST_Shoot");

					if (shotTimes >= maxShots)
					{
						shotTimes = 0;
						shotSequences++;
						
						if (shotSequences >= maxSequences)
						{
							currentState = STATES.RUN;
							Animator.Play(gameObject, "ST_Run");
							targetPosition = CalculateNewPosition(runningRange);
							shotSequences = 0;
						}
						else
						{
							Animator.Play(gameObject, "ST_Idle");
							currentState = STATES.IDLE;
                        }
					}
				}
				break;

			case STATES.HIT:
				break;

			case STATES.DIE:
				//Debug.Log("ST_Die");

				timePassed += Time.deltaTime;

				if(timePassed > 3.0f)
                {
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

			if(currentState != STATES.DIE)
            {
				currentState = STATES.DIE;
				timePassed = 0.0f;
				Animator.Play(gameObject, "ST_Run");
			}
		}
	}

	
	public void OnTriggerEnter(GameObject triggeredGameObject)
	{
		//Debug.Log("CS: Collided object: " + gameObject.tag + ", Collider: " + triggeredGameObject.tag);
		if (triggeredGameObject.CompareTag("PushSkill"))
		{
			Debug.Log("Skill collision");

		/*	Vector3 pushDirection = Core.instance.gameObject.transform.localPosition - gameObject.transform.globalPosition;
			pushDirection = pushDirection.normalized;

			gameObject.transform.localPosition += pushDirection * 5;*/
		}

		//Debug.Log("Triggered by tag: " + triggeredGameObject.tag);
	}
	
}