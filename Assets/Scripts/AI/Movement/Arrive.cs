using UnityEngine;

public class Arrive : AIMovement
{
	public Transform target;
	public float stopDistance = 0.1f;

	public override Vector3 GetSteeringOutput(AIAgent agent)
	{
		if (target == null)
		{
			return Vector3.zero;
		}

		if (Vector3.Distance(agent.transform.position, target.position) <= stopDistance)
		{
			target = null;
			return Vector3.zero;
		}

		return (target.position - agent.transform.position).normalized;
	}

}
