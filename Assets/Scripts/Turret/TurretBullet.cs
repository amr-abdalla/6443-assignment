using UnityEngine;

public class TurretBullet : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float expiryTime;

	private Vector3 direction;
	private float spawnTime;

    public void Fire(Vector3 shootDirection)
    {
        direction = shootDirection.normalized;
        spawnTime = Time.time;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        if (Time.time - spawnTime >= expiryTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent(out AgentHealth health))
		{
			health.TakeDamage(damage);
		}

        Destroy(gameObject);
    }
}
