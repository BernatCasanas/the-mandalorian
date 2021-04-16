using DiamondEngine;

class RancorHandSlamHitCollider : DiamondComponent
{
    private int damage = 25;

    private Vector3 initPos = new Vector3(0f, 3.5f, 1.5f);
    private Vector3 finalPos = new Vector3(0f, 0.4f, 2f);
    private float speed = 2f;

    public void Start()
    {
        gameObject.transform.localPosition = initPos;
        Debug.Log("Init HandSlam");
       
    }

    public void Update()
    {
        gameObject.transform.localPosition = Vector3.Lerp(initPos, finalPos, Time.deltaTime * speed);
        Debug.Log("Update HandSlam");
    }

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        if (triggeredGameObject.CompareTag("Player"))
        {
            PlayerHealth health = triggeredGameObject.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(damage);
            Debug.Log("Damaged From HandSlamHit");
        }
    }
}

