using UnityEngine;

public class TurretController : MonoBehaviour
{
	[SerializeField] private float range;
	[SerializeField] private float cooldown;
	[SerializeField] private LayerMask characterMask;
	[SerializeField] private TurretBullet bulletPrefab;
	[SerializeField] private Transform bulletSpawnPoint;

	private AIAgentDecisionMaker currentTarget;
	private float lastHitTime;

	private void Update()
	{
		if (currentTarget == null)
		{
			if (TryUpdateTarget())
			{
				lastHitTime = Time.time;
			}
			else
			{
				return;
			}
		}

		float sqrDistance = (currentTarget.transform.position - transform.position).sqrMagnitude;

		if (currentTarget.IsCovered() || sqrDistance > range * range)
		{
			currentTarget = null;
			return;
		}

		AIMover aIMover = currentTarget.GetComponent<AIMover>();
		Vector3 predictedPos = currentTarget.transform.position + aIMover.Velocity * aIMover.maxSpeed * Time.deltaTime;

		transform.rotation = Quaternion.LookRotation((predictedPos - transform.position).normalized);

		if (Time.time - lastHitTime >= cooldown)
		{
			Shoot();
			lastHitTime = Time.time;
		}
	}

	private void Shoot()
	{
		TurretBullet bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
		bullet.Fire(transform.forward);
	}

	private bool TryUpdateTarget()
	{
		Collider[] hits = Physics.OverlapSphere(transform.position, range, characterMask);

		AIAgentDecisionMaker closestAgent = null;
		float closestDistance = float.MaxValue;

		foreach (var hit in hits)
		{
			AIAgentDecisionMaker agent = hit.GetComponent<AIAgentDecisionMaker>();

			if (agent == null || agent.IsCovered())
				continue;

			float sqrDistance = (agent.transform.position - transform.position).sqrMagnitude;

			if (sqrDistance < closestDistance)
			{
				closestDistance = sqrDistance;
				closestAgent = agent;
			}
		}

		currentTarget = closestAgent;

		return currentTarget != null;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(transform.position, range);
	}

}
