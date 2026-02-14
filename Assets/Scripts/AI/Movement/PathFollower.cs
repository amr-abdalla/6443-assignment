using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : AIMovement
{
    public float speed = 5f;
    public float waypointTolerance = 0.05f;

    [SerializeField] private Pathfinding pathfinding;
    [SerializeField] private Arrive arrive;
    public LayerMask obstacleMask;

    private List<Vector3> smoothPath;
    private int currentWaypointIndex;
    private bool isFollowing;
    public Transform goal;

	private void Start()
	{
        FollowPath();
	}

	[Button]
    public void FollowPath()
    {
        var rawPath = pathfinding.FindPath(transform, goal.transform);
        if (rawPath == null || rawPath.Count < 2)
            return;

        smoothPath = GetSmoothedPath(rawPath);
        currentWaypointIndex = 0;
        isFollowing = true;

        //transform.position = smoothPath[0];
    }

    public override Vector3 GetSteeringOutput(AIAgent agent)
    {
        return GetTarget();
    }

    Vector3 GetTarget()
	{
        if (!isFollowing || smoothPath == null || smoothPath.Count == 0)
            return Vector3.zero;

        if (currentWaypointIndex >= smoothPath.Count)
        {
            isFollowing = false;
            arrive.target = goal;
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
                path[current].transform.position + Vector3.up,
                path[next].transform.position + Vector3.up,
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
