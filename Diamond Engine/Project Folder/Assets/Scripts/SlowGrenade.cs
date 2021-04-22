using System;
using DiamondEngine;
using System.Collections.Generic;

public class SlowGrenade : DiamondComponent
{
	public float forwardForce = 1000;
	public float upForce = 1000;

	bool start = true;
	bool detonate = false;
	public GameObject grenadeActiveObj = null;
	private ParticleSystem grenadeActivePar = null;

	public float lifeTime = 4.0f;
	private float lifeTimer = 0.0f;

	public float explosionTime = 4.0f; 
	private float explosionTimer = 0.0f;

	public float damage = 5.0f;
	List<GameObject> enemies = new List<GameObject>();

	//public void Awake()
	//   {
	//	Vector3 myForce = gameObject.transform.GetForward() * force;
	//	gameObject.AddForce(myForce);
	//   }
	public void Update()
	{
		if(start == true)
        {
			start = false;

			Quaternion rotation = Quaternion.RotateAroundAxis(Vector3.up, 0.383972f);

			Vector3 myForce = gameObject.transform.GetForward() * forwardForce;

			myForce.y = upForce;

			gameObject.AddForce(myForce);
		}
		
		if (!detonate)
        {
			lifeTimer += Time.deltaTime;
		}
		else
        {
			explosionTimer += Time.deltaTime;
			for (int i = 0; i < enemies.Count; i++)
            {

				StormTrooper script = enemies[i].GetComponent<StormTrooper>();
				if(script !=  null)
                {
					if (script.healthPoints <= 0)
                    {
						enemies.Remove(enemies[i]);
					}
					else
                    {
						Debug.Log("Grenade Dealing Damage");
						script.healthPoints -= (damage * Time.deltaTime);

					}

				}

			}
		}

		if (lifeTimer >= lifeTime)
		{
			if (grenadeActiveObj != null)
			{
				grenadeActivePar = grenadeActiveObj.GetComponent<ParticleSystem>();

				if (grenadeActivePar != null)
					grenadeActivePar.Play();
			}
			detonate = true;
			lifeTimer = 0;

		}

		if (explosionTimer >= explosionTime)
        {
			InternalCalls.Destroy(gameObject);
			
		}
	}
	public void OnTriggerEnter(GameObject triggeredGameObject)
    {
		if(triggeredGameObject != null)
		{
			if(triggeredGameObject.tag == "Enemy")
            {
				enemies.Add(triggeredGameObject);
			}
        }
    }
	public void OnTriggerExit(GameObject triggeredGameObject)
	{
		if (triggeredGameObject != null)
		{
			if (triggeredGameObject.tag == "Enemy")
			{
				//if (!enemies.Remove(triggeredGameObject))
				//	Debug.Log("can't remove");

				foreach (GameObject item in enemies)
                {
					if (item.GetUid() == triggeredGameObject.GetUid())
                        
						if (!enemies.Remove(item))
                            Debug.Log("can't remove");

						break;
                }
			}
		}
	}
}