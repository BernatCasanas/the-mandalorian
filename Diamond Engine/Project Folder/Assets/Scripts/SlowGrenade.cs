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
	public GameObject granadeAreaObj = null;
	private ParticleSystem granadeArea = null;

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
			Vector3 myForce = gameObject.transform.GetForward() * forwardForce;

			myForce.y = upForce;

			gameObject.AddForce(myForce);

			damage *= 4;
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
				Enemy script = enemies[i].GetComponent<StormTrooper>();
				if (script == null)
                {
					script = enemies[i].GetComponent<Bantha>();
				}
				if (script == null)
				{
					script = enemies[i].GetComponent<Skytrooper>();
				}
				if (script == null)
				{
					script = enemies[i].GetComponent<LaserTurret>();
				}
				if (script !=  null)
                {
					if (script.healthPoints <= 0)
                    {
						enemies.Remove(enemies[i]);
						i--;
					}
					else
                    {
						Core.instance.hud.GetComponent<HUD>().AddToCombo(20 * Time.deltaTime, 1.5f);
						script.TakeDamage(damage * Time.deltaTime);

					}

				}
				else
                {
					Rancor rancorScript = enemies[i].GetComponent<Rancor>();

					if (rancorScript != null)
					{
						if (rancorScript.healthPoints <= 0)
						{
							enemies.Remove(enemies[i]);
							i--;
						}
						else
                        {
							Core.instance.hud.GetComponent<HUD>().AddToCombo(20 * Time.deltaTime, 1.5f);
							rancorScript.TakeDamage(damage * Time.deltaTime);
						}
					}
				}
				

			}
		}

		if (lifeTimer >= lifeTime & !detonate)
		{
			if (grenadeActiveObj != null)
			{
				grenadeActivePar = grenadeActiveObj.GetComponent<ParticleSystem>();
				if (grenadeActivePar != null)
					grenadeActivePar.Play();
			}
			if (granadeAreaObj != null)
			{
				granadeArea = granadeAreaObj.GetComponent<ParticleSystem>();
				if (granadeArea != null)
					granadeArea.Play();
			}
			detonate = true;
			lifeTimer = 0;

		}

		if (explosionTimer >= explosionTime)
			InternalCalls.Destroy(gameObject);
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
                        
						if (enemies.Remove(item))
                            Debug.Log("removed");

						break;
                }
			}
		}
	}
}