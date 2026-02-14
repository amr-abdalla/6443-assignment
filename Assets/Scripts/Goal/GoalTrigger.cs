using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
	public LayerMask obstacleMask;

    private void OnTriggerEnter(Collider other)
    {
        if (IsInLayerMask(other.gameObject, obstacleMask))
        {
            Destroy(other.gameObject);
        }
    }

    bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return (mask.value & (1 << obj.layer)) != 0;
    }
}
