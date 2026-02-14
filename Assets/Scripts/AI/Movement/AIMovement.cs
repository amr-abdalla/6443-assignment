using UnityEngine;

public abstract class AIMovement : MonoBehaviour
{
    public struct SteeringOutput
    {
        public Vector3 linear;
        public Quaternion angular;
    }

    public bool debug;

    public virtual SteeringOutput GetSteeringOutput(AIAgent agent)
    {
        return new SteeringOutput { angular = agent.transform.rotation };
    }

}
