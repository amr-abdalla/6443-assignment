using UnityEngine;

public class AvoidNeighbors : AIMovement
{
	public float avoidanceRadius = 3.5f;
	public float neighborRadius = 5;
	public LayerMask neighborMask;
	[Range(0, 180)] public float visionCone= 120f;

	public override Vector3 GetSteeringOutput(AIAgentDecisionMaker agent)
	{
        return Avoidance(GetNeighborContext());
	}

	public Collider[] GetNeighborContext()
	{
		return Physics.OverlapSphere(transform.position, neighborRadius, neighborMask);
	}

    Vector3 Avoidance(Collider[] neighbors)
    {
        Vector3 avoidanceMovement = Vector3.zero;
        float avoidanceRadiusSquared = avoidanceRadius * avoidanceRadius;
        float halfAngle = visionCone * 0.5f;
        float cosThreshold = Mathf.Cos(halfAngle * Mathf.Deg2Rad);

        int validNeighbors = 0;

        foreach (Collider neighbor in neighbors)
        {
            if (transform == neighbor.transform) continue;

            Vector3 toNeighbor = neighbor.transform.position - transform.position;
            float sqrDist = toNeighbor.sqrMagnitude;

            // Distance check
            if (sqrDist > avoidanceRadiusSquared)
                continue;

            Vector3 dirToNeighbor = toNeighbor.normalized;

            // Angle check (is neighbor in front?)
            float dot = Vector3.Dot(transform.forward, dirToNeighbor);

            if (dot < cosThreshold)
                continue; // Not inside front angle

            // Avoid
            Vector3 neighborToAgent = -dirToNeighbor;

            avoidanceMovement += neighborToAgent;
            validNeighbors++;
        }

        if (validNeighbors > 0)
        {
            avoidanceMovement /= validNeighbors;
            return avoidanceMovement.normalized;
        }

        return Vector3.zero;
    }

    private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(transform.position, neighborRadius);
	}

}
