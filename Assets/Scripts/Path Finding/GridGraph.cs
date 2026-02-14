using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class GridGraph : MonoBehaviour
{
	public Dictionary<Vector2Int, GridGraphNode> nodeDict = new Dictionary<Vector2Int, GridGraphNode>();
	[SerializeField] public GridGraphNode nodePrefab;

	public float generationGridCellSize = 1f;
	public float checkSphereRadius = 1f;
	public LayerMask obstacleMask;

	public int Count => nodeDict.Count;

	private void Awake()
	{
		GenerateGrid();
	}

	public void Clear()
	{
		nodeDict.Clear();
		gameObject.DestroyChildren();
	}

	public Vector2Int GetCoords(Transform node)
	{
		Vector3 delta = node.position - transform.position;

		return new Vector2Int(
			Mathf.RoundToInt(delta.x / generationGridCellSize),
			Mathf.RoundToInt(delta.z / generationGridCellSize)
		);
	}

	public void Remove(GridGraphNode node)
	{
		if (node == null || !nodeDict.ContainsValue(node)) return;

		foreach (GridGraphNode n in node.adjacencyList)
		{
			n.adjacencyList.Remove(node);
		}

		nodeDict.Remove(GetCoords(node.transform));
	}

	public void Add(GridGraphNode node)
	{
		Vector2Int coord = GetCoords(node.transform);

		if (nodeDict.ContainsKey(coord))
			return;

		node.adjacencyList.Clear();

		Vector2Int[] directions =
		{
			new Vector2Int(1, 0),
			new Vector2Int(-1, 0),
			new Vector2Int(0, 1),
			new Vector2Int(0, -1)
		};

		foreach (Vector2Int dir in directions)
		{
			Vector2Int neighborCoord = coord + dir;

			if (!nodeDict.TryGetValue(neighborCoord, out GridGraphNode neighbor))
				continue;

			// Add connection both ways
			node.adjacencyList.Add(neighbor);
			neighbor.adjacencyList.Add(node);
		}

		nodeDict.Add(coord, node);
	}

	[Button]
	public void GenerateGrid(bool checkCollisions = true)
	{
		Clear();

		Queue<Vector2Int> toProcess = new Queue<Vector2Int>();

		Vector3 origin = new Vector3(transform.position.x, 0f, transform.position.z);
		Vector2Int originCoord = Vector2Int.zero;

		// Create origin node
		GridGraphNode originNode = Instantiate(nodePrefab, transform);
		originNode.transform.position = origin;
		originNode.transform.localScale *= generationGridCellSize;

		nodeDict.Add(originCoord, originNode);
		toProcess.Enqueue(originCoord);

		Vector2Int[] directions = new Vector2Int[]
		{
			new Vector2Int(1, 0),
			new Vector2Int(-1, 0),
			new Vector2Int(0, 1),
			new Vector2Int(0, -1)
		};

		while (toProcess.Count > 0)
		{
			Vector2Int currentCoord = toProcess.Dequeue();
			GridGraphNode currentNode = nodeDict[currentCoord];

			foreach (Vector2Int dir in directions)
			{
				Vector2Int neighborCoord = currentCoord + dir;

				if (nodeDict.ContainsKey(neighborCoord))
					continue;

				Vector3 neighborPos = origin + new Vector3(neighborCoord.x * generationGridCellSize, 0f, neighborCoord.y * generationGridCellSize);

				Vector3 currentPos = currentNode.transform.position;

				bool blocked = checkCollisions && (Physics.Linecast(currentPos + Vector3.up, neighborPos + Vector3.up, obstacleMask) || Physics.CheckSphere(neighborPos, checkSphereRadius, obstacleMask));

				if (!blocked)
				{
					// Create neighbor node
					GridGraphNode neighborNode = Instantiate(nodePrefab, transform);
					neighborNode.transform.position = neighborPos;
					neighborNode.transform.localScale *= generationGridCellSize;

					nodeDict.Add(neighborCoord, neighborNode);
					toProcess.Enqueue(neighborCoord);
				}
			}
		}

		foreach (var kvp in nodeDict)
		{
			Vector2Int coord = kvp.Key;
			GridGraphNode node = kvp.Value;
			node.adjacencyList.Clear();

			foreach (Vector2Int dir in directions)
			{
				Vector2Int neighborCoord = coord + dir;

				if (!nodeDict.TryGetValue(neighborCoord, out GridGraphNode neighbor))
					continue;

				// Prevent diagonal corner cutting
				if (Mathf.Abs(dir.x) == 1 && Mathf.Abs(dir.y) == 1)
				{
					Vector2Int side1 = coord + new Vector2Int(dir.x, 0);
					Vector2Int side2 = coord + new Vector2Int(0, dir.y);

					if (!nodeDict.ContainsKey(side1) || !nodeDict.ContainsKey(side2))
						continue;
				}

				node.adjacencyList.Add(neighbor);
			}
		}

	}

	public List<GridGraphNode> GetNeighbors(GridGraphNode node)
	{
		return node.adjacencyList;
	}

	#region grid_generation_properties

	[Header("Gizmos")]
	/// <summary>WARNING: This property is used by Gizmos only and is removed from the build. DO NOT reference it outside of Editor-Only code.</summary>
	public float _nodeGizmoRadius = 0.5f;
	/// <summary>WARNING: This property is used by Gizmos only and is removed from the build. DO NOT reference it outside of Editor-Only code.</summary>
	public Color _edgeGizmoColor = Color.white;

	private void OnDrawGizmos()
	{
		if (nodeDict == null) return;

		// nodes
		foreach (GridGraphNode node in nodeDict.Values)
		{
			if (node == null) continue;

			Gizmos.color = node._nodeGizmoColor;
			Gizmos.DrawSphere(node.transform.position, _nodeGizmoRadius);

			List<GridGraphNode> neighbors = GetNeighbors(node);
			Gizmos.color = _edgeGizmoColor;
			foreach (GridGraphNode neighbor in neighbors)
			{
				Gizmos.DrawLine(node.transform.position, neighbor.transform.position);
			}
		}
	}
	#endregion
}
