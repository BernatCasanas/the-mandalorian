using System;
using DiamondEngine;


public class Enemy : DiamondComponent
{
	public GameObject player = null;

	public float detectionRange = 20.0f;
	public float damage = 20.0f;
    public float healthPoints = 60.0f;

	protected Vector3 targetPosition = null;
	protected float stoppingDistance = 1.0f;

	public float slerpSpeed = 5.0f;
	public bool turretMode = false;

	//protected STATES currentState = STATES.WANDER;

    public Spawn dieCallBack = null;

	protected NavMeshAgent agent;

	public virtual bool TakeDamage()
	{
		return true;
	}

	public virtual Vector3 CalculateNewPosition(float maxPos)
	{
        Vector3 newPosition = new Vector3(0, 0, 0);
        Random random = new Random();

        newPosition.x = random.Next((int)maxPos);
        newPosition.y = gameObject.transform.localPosition.y;
        newPosition.z = random.Next((int)maxPos);

        return newPosition;

        //return InternalCalls.GetWalkablePointAround(gameObject.transform.globalPosition, maxPos);
    }

	public void MoveToPosition(Vector3 positionToReach, float speed)
	{
		Vector3 direction = positionToReach - gameObject.transform.localPosition;

		gameObject.transform.localPosition += direction.normalized * speed * Time.deltaTime;
	}

	public void LookAt(Vector3 pointToLook)
	{
		Vector3 direction = pointToLook - gameObject.transform.globalPosition;
		direction = direction.normalized;
		float angle = (float)Math.Atan2(direction.x, direction.z);

		//Debug.Log("Desired angle: " + (angle * Mathf.Rad2Deg).ToString());

		if (Math.Abs(angle * Mathf.Rad2Deg) < 1.0f)
			return;

		Quaternion dir = Quaternion.RotateAroundAxis(Vector3.up, angle);

		float rotationSpeed = Time.deltaTime * slerpSpeed;
		//Debug.Log("CS: Rotation speed: " + rotationSpeed.ToString());
		//Debug.Log("CS: Time: " + Time.deltaTime);

		Quaternion desiredRotation = Quaternion.Slerp(gameObject.transform.localRotation, dir, rotationSpeed);

		gameObject.transform.localRotation = desiredRotation;

	}

	public bool InRange(Vector3 point, float givenRange)
    {
		return Mathf.Distance(gameObject.transform.globalPosition, point) < givenRange;
	}
    public void RemoveFromSpawner()
    {
        if (dieCallBack != null)
        {
			foreach (GameObject item in dieCallBack.currentEnemies)
			{
				if (item.GetUid() == gameObject.GetUid())
				{
					dieCallBack.currentEnemies.Remove(item);
					//Debug.Log("Enemy Killed!");
					break;
				}
			}
		}
    }
}
