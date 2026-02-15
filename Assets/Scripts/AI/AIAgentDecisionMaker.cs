using NaughtyAttributes;
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

	private void Start()
	{
		GridGraph.Instance.OnGridChanged += SetGoal;
	}

	private void OnDestroy()
	{
		GridGraph.Instance.OnGridChanged -= SetGoal;
	}

	[Button]
	public void SetNewGoal()
	{
		if (occupiedCover != null)
		{
			occupiedCover.Unoccupy(this);
		}
		else
		{
			SetGoal();
		}
	}

	public void SetGoal()
	{
		if (occupiedCover != null)
		{
			return;
		}

		AgentStats.HealthStatus healthStatus = health.GetHealthStatus();
		Transform finalGoal = GameStatsManager.Instance.GetGoal(); 
		Vector3 directionToGoal = (finalGoal.position - transform.position).normalized;


		if (healthStatus == AgentStats.HealthStatus.Good)
		{
			HandleGoodHealth(directionToGoal);
		}
		else if (healthStatus == AgentStats.HealthStatus.Mid)
		{
			HandleGoodHealth(directionToGoal);
			//TODO
		}
		else if (healthStatus == AgentStats.HealthStatus.Low)
		{
			HandleGoodHealth(directionToGoal);
			//TODO
		}

		if (currentGoal == null)
		{
			Debug.LogError("No Goal Set");
		}
	}

	private void HandleGoodHealth(Vector3 directionToGoal)
	{
		IOrderedEnumerable<Cover> coversInRadius = DetectFrontCoversInRadius(directionToGoal).OrderByDescending(c => Vector3.Distance(GameStatsManager.Instance.GetGoal().position, c.transform.position));

		if (TryAssignGoal(coversInRadius))
		{
			return;
		}

		var allFrontCovers = DetectAllFrontCovers(directionToGoal);

		var otherFrontCovers = allFrontCovers.Except(coversInRadius);

		if (TryAssignGoal(otherFrontCovers))
		{
			return;
		}

		if (TryAssignGoal(GameStatsManager.Instance.GetGoal()))
		{
			return;
		}

		IEnumerable<Cover> otherCovers = GameStatsManager.Instance.allCovers.Except(allFrontCovers);

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
