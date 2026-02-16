using System.Collections.Generic;
using UnityEngine;

public class PathFollower : AIMovement
{
	public float speed = 5f;
	public float waypointTolerance = 0.05f;

	[SerializeField] private Pathfinding pathfinding;
	public LayerMask obstacleMask;

	private List<Vector3> smoothPath;
	private int currentWaypointIndex;
	private bool isFollowing;

	private void Start()
	{
		//StartFollowingPath();
		//GridGraph.Instance.OnGridChanged += OnGridChanged;
	}

	//private void OnGridChanged()
	//{
	//	if (!isFollowing || smoothPath == null || smoothPath.Count == 0 || decisionMaker.currentGoal == null)
	//	{
	//		return;
	//	}

	//	if (!FollowPath())
	//	{
	//		decisionMaker.SetGoal();
	//		isFollowing = false;
	//	}
	//}

	public bool TrySetPath(Transform goal)
	{
		var rawPath = pathfinding.FindPath(transform, goal);
		if (rawPath == null || rawPath.Count < 2)
			return false;

		smoothPath = GetSmoothedPath(rawPath);
		return true;
	}


	public void StartFollowingPath()
	{
		if (smoothPath == null || smoothPath.Count == 0)
		{
			return;
		}

		currentWaypointIndex = 0;
		isFollowing = true;
	}

	public override Vector3 GetSteeringOutput(AIAgentDecisionMaker agent)
	{
		return GetTarget(agent);
	}

	Vector3 GetTarget(AIAgentDecisionMaker agent)
	{
		if (!isFollowing || smoothPath == null || smoothPath.Count == 0)
			return Vector3.zero;

		if (currentWaypointIndex >= smoothPath.Count)
		{
			isFollowing = false;
			agent.ArriveToGoal();
			return Vector3.zero;
		}

		Vector3 target = smoothPath[currentWaypointIndex];
		Vector3 direction = target - transform.position;
		direction.y = 0f;

		float distance = direction.magnitude;

		if (distance < waypointTolerance)
		{
			currentWaypointIndex++;
		}

		return direction.normalized;
	}

	private List<Vector3> GetSmoothedPath(List<GridGraphNode> path)
	{
		int current = 0;
		int next = 1;

		List<Vector3> smoothedPath = new List<Vector3>()
		{
			path[0].transform.position
		};

		while (next < path.Count)
		{
			bool blocked = Physics.Linecast(
				path[current].transform.position + Vector3.up * 2f,
				path[next].transform.position + Vector3.up * 2f,
				obstacleMask
			);

			if (blocked)
			{
				smoothedPath.Add(path[next - 1].transform.position);
				current = next - 1;
			}
			else
			{
				next++;
			}
		}

		if (next > 1)
		{
			smoothedPath.Add(path[next - 1].transform.position);
		}

		return smoothedPath;
	}
}
