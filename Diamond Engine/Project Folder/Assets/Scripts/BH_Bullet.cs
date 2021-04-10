using System;
using DiamondEngine;


public class BH_Bullet : DiamondComponent
{
    public float speed = 60.0f;
    public float maxLifeTime = 5.0f;

    public float currentLifeTime = 0.0f;

    public float yVel = 0.0f;
    public float damage = 9.0f;

    private bool started = false;
    private bool triggered = false;
    private float timer = 0;

    public GameObject destroyOBJ = null;
    public GameObject mesh = null;

    public ParticleSystem destroyPar = null;

    public void Update()
    {
        if (started == false)
        {
            started = true;
            InternalCalls.CreatePrefab("Library/Prefabs/150008798.prefab", gameObject.transform.globalPosition, gameObject.transform.globalRotation, gameObject.transform.globalScale);
        }

        currentLifeTime += Time.deltaTime;

        // gameObject.transform.localPosition += gameObject.transform.GetForward() * (speed * Time.deltaTime);
       if(!triggered)
        gameObject.SetVelocity(gameObject.transform.GetForward() * speed);
       else
        {  if(mesh != null)
            {
                InternalCalls.Destroy(mesh);
                mesh = null;
            }
            timer += Time.deltaTime;
            gameObject.SetVelocity(new Vector3(0, 0, 0));

        }
        //yVel -= Time.deltaTime / 15.0f;
        //gameObject.transform.localPosition += (Vector3.up * yVel);

        if (currentLifeTime >= maxLifeTime)
        {
            InternalCalls.Destroy(this.gameObject);
        }
  
        if (timer > 0.5)
        InternalCalls.Destroy(gameObject);

    }

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        if (triggeredGameObject.tag != "Player")
        {
            triggered = true;

            if (destroyOBJ != null)
            {
                destroyPar = destroyOBJ.GetComponent<ParticleSystem>();

                if (destroyPar != null)
                    destroyPar.Play();
            }
        }
    }
}