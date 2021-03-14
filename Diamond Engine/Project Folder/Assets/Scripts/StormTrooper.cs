using System;
using DiamondEngine;

public class Stormtrooper : Enemy
{
	public void Start()
	{
		currentState = STATES.WANDER;
		targetPosition = CalculateNewPosition(wanderRange);
		shotTimes = 0;
	}

	public void Update()
	{
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
					}
				}

				break;

			case STATES.RUN:
				//Debug.Log("Run");

				LookAt(targetPosition);
				MoveToPosition(targetPosition, runningSpeed);

				if (Mathf.Distance(gameObject.transform.localPosition, targetPosition) < stoppingDistance)
				{
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
					timePassed = timeBewteenShots;
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
				//Debug.Log("Shoot");
				
				timePassed += Time.deltaTime;

				LookAt(player.transform.globalPosition);

				if (timePassed > timeBewteenShots)
				{
					Shoot();

					if (shotTimes >= 2)
					{
						shotSequences++;
						if (shotSequences >= 2)
						{
							currentState = STATES.RUN;
							targetPosition = CalculateNewPosition(runningRange);
							shotTimes = 0;
							shotSequences = 0;
						}
						else
						{
							currentState = STATES.IDLE;
							shotTimes = 0;
                        }
					}
				}
				break;

			case STATES.HIT:
				break;

			case STATES.DIE:
				Debug.Log("Die");
				InternalCalls.Destroy(gameObject);
				break;
		}
		
	}

	public void OnTriggerEnter()
	{
		//state = STATES.HIT;
	}
}