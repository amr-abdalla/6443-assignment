using UnityEngine;

public abstract class AIMovement : MonoBehaviour
{
	public abstract Vector3 GetSteeringOutput(AIAgent agent);
}
