using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIAgentDecisionMaker : MonoBehaviour
{
	[SerializeField] private AgentHealth health;
	[SerializeField] private Arrive arrive;
	[SerializeField] private PathFollower pathFollower;
	public Cover occupiedCover;
	public Transform currentGoal { get; private set; }
	[SerializeField] private LayerMask coverMask;
	[SerializeField] private float detectionRadius;
	public SquadAI squadAI;

	private void Start()
	{
		GridGraph.Instance.OnGridChanged += SetGoal;
		arrive.OnGoalReached += OnGoalReached;
	}

	private void OnGoalReached()
	{
		currentGoal = null;
		squadAI.CheckIfAllMembersFinished();
	}

	private void OnDestroy()
	{
		GridGraph.Instance.OnGridChanged -= SetGoal;
		arrive.OnGoalReached -= OnGoalReached;
		squadAI.RemoveMember(this);
		squadAI.CheckIfAllMembersFinished();
	}

	[Button]
	public void SetNewGoal(List<Cover> bookedCovers)
	{
		if (occupiedCover != null)
		{
			occupiedCover.Unoccupy(this);
		}

		currentGoal = null;
		SetGoal(bookedCovers);
		
	}

	public void SetGoal()
	{
		if (currentGoal != null)
		{
			TryAssignGoal(currentGoal);
			return;
		}

		SetGoal(squadAI.bookedCovers);
	}


	public void SetGoal(List<Cover> bookedCovers)
	{
		if (occupiedCover != null)
		{
			return;
		}

		if (currentGoal != null)
		{
			TryAssignGoal(currentGoal);
		}

		AgentStats.HealthStatus healthStatus = health.GetHealthStatus();
		Transform finalGoal = GameStatsManager.Instance.GetGoal(); 
		Vector3 directionToGoal = (finalGoal.position - transform.position).normalized;


		if (healthStatus == AgentStats.HealthStatus.Good)
		{
			HandleGoodHealth(directionToGoal, bookedCovers);
		}
		else if (healthStatus == AgentStats.HealthStatus.Mid)
		{
			HandleGoodHealth(directionToGoal, bookedCovers);
			//TODO
		}
		else if (healthStatus == AgentStats.HealthStatus.Low)
		{
			HandleGoodHealth(directionToGoal, bookedCovers);
			//TODO
		}

		if (currentGoal == null)
		{
			Debug.LogError("No Goal Set");
		}
	}

	private void HandleGoodHealth(Vector3 directionToGoal, List<Cover> bookedCovers)
	{
		IOrderedEnumerable<Cover> coversInRadius = DetectFrontCoversInRadius(directionToGoal).Except(bookedCovers).OrderByDescending(c => Vector3.Distance(GameStatsManager.Instance.GetGoal().position, c.transform.position));

		if (TryAssignGoal(coversInRadius))
		{
			return;
		}

		var allFrontCovers = DetectAllFrontCovers(directionToGoal).Except(bookedCovers);

		var otherFrontCovers = allFrontCovers.Except(coversInRadius);

		if (TryAssignGoal(otherFrontCovers))
		{
			return;
		}

		if (TryAssignGoal(GameStatsManager.Instance.GetGoal()))
		{
			return;
		}

		IEnumerable<Cover> otherCovers = GameStatsManager.Instance.allCovers.Except(allFrontCovers).Except(bookedCovers);

		if (TryAssignGoal(otherCovers))
		{
			return;
		}
	}


	private HashSet<Cover> DetectFrontCoversInRadius(Vector3 goalDirection)
	{
		Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, coverMask);
		HashSet<Cover> detectedCovers = new HashSet<Cover>();

		foreach (var hit in hits)
		{
			Cover cover = hit.GetComponent<Cover>();

			if (cover == null || cover == occupiedCover)
			{
				continue;
			}

			Vector3 dir = (cover.transform.position - transform.position).normalized;

			if (Vector3.Dot(goalDirection, dir) > 0f)
			{
				detectedCovers.Add(cover);
			}
		}

		return detectedCovers;
	}

	private List<Cover> DetectAllFrontCovers(Vector3 goalDirection)
	{
		List<Cover> covers = GameStatsManager.Instance.allCovers;

		return covers.Where(c =>
				{
					Vector3 dir = (c.transform.position - transform.position).normalized;
					return Vector3.Dot(goalDirection, dir) > 0f; // In front of agent
				}).ToList();
	}

	private bool TryAssignGoal(IEnumerable<Cover> covers)
	{
		foreach (var cover in covers)
		{
			if (pathFollower.TrySetPath(cover.transform))
			{
				currentGoal = cover.transform;
				StartFollowingPath();
				return true;
			}
		}

		return false;
	}

	private bool TryAssignGoal(Transform target)
	{
		if (pathFollower.TrySetPath(target))
		{
			currentGoal = target;
			StartFollowingPath();
			return true;
		}

		return false;
	}

	private void StartFollowingPath()
	{
		pathFollower.StartFollowingPath();
	}

	public void ArriveToGoal()
	{
		arrive.target = currentGoal;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, detectionRadius);
	}
}
