using DiamondEngine;

class RancorHandSlamWave : DiamondComponent
{
    public float colliderSpeed = 0.2f;
    public float lifeTime = 2.0f;

    public int damage = 25;
    public GameObject slamWave = null;
    private ParticleSystem wave = null;

    public void Awake()
    {
        if (slamWave != null)
            wave = slamWave.GetComponent<ParticleSystem>();
        if (wave != null)
            wave.Play();
    }

    public void Update()
    {
        if (lifeTime > 0.0f)
        {
            lifeTime -= Time.deltaTime;

            if (lifeTime <= 0.0f)
            {
                InternalCalls.Destroy(gameObject);
            }
        }

        gameObject.transform.localPosition += gameObject.transform.GetForward().normalized * colliderSpeed;
    }

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        if (triggeredGameObject.CompareTag("Player"))
        {
            PlayerHealth health = triggeredGameObject.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(damage);
        }
    }

}

