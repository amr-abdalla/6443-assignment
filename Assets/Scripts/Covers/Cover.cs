using UnityEngine;

public class Cover : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask defaultLayer;
    [SerializeField] private GameObject coverSphere;

    private GridGraphNode correspondingNode;
    private AIAgentDecisionMaker occupiedAgent;
    public bool isBooked = false;

    public bool IsOccupied() => occupiedAgent != null;

    public bool IsActive() => !IsOccupied() && !isBooked;

    // TODO: Change to be after generating graph
	private void Start()
	{
        correspondingNode = FindOccupiedNode();
        GameStatsManager.Instance.AddCover(this);
	}

	public void Occupy(AIAgentDecisionMaker aIAgent)
	{
        isBooked = false;
        occupiedAgent = aIAgent;
        aIAgent.occupiedCover = this;
        gameObject.layer = GetFirstLayer(obstacleLayer);

        GridGraph.Instance.Remove(correspondingNode);
        coverSphere.SetActive(true);
    }

    public void Unoccupy(AIAgentDecisionMaker aIAgent)
    {
        if (occupiedAgent != aIAgent)
        {
            Debug.LogError("Calling Unoccupy with the wrong agent");
            return;
        }

        occupiedAgent = null;
        aIAgent.occupiedCover = null;
        gameObject.layer = GetFirstLayer(defaultLayer);
        GridGraph.Instance.Add(correspondingNode);
        coverSphere.SetActive(false);
    }

    int GetFirstLayer(LayerMask mask)
    {
        for (int i = 0; i < 32; i++)
        {
            if ((mask.value & (1 << i)) != 0)
                return i;
        }
        return -1;
    }

    private GridGraphNode FindOccupiedNode()
	{
        Vector2Int coords = GridGraph.Instance.GetCoords(transform);

        if (GridGraph.Instance.nodeDict.TryGetValue(coords, out GridGraphNode node))
		{
            return node;
		}

        return null;
	}

}
