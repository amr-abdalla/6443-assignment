using System;
using UnityEngine;

public class Arrive : AIMovement
{
	public Transform target;
	public float stopDistance = 0.1f;
	public Action OnGoalReached;

	public override Vector3 GetSteeringOutput(AIAgentDecisionMaker agent)
	{
		if (target == null)
		{
			return Vector3.zero;
		}

		if (Vector3.Distance(agent.transform.position, target.position) <= stopDistance)
		{
			OnArrive();
			target = null;
			return Vector3.zero;
		}

		return (target.position - agent.transform.position).normalized;
	}

	private void OnArrive()
	{
		if (target.TryGetComponent(out Cover cover))
		{
			cover.Occupy(GetComponent<AIAgentDecisionMaker>());
		}

		OnGoalReached?.Invoke();
	}
}
