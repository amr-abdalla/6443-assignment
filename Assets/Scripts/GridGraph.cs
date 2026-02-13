using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Very quick basic graph implementation that was created to be used only for COMP 476 Lab on pathfinding.
/// It is most likely not suitable for more practical use cases without modification.
/// </summary>
public class GridGraph : MonoBehaviour
{
	[SerializeField, HideInInspector] public List<GridGraphNode> nodes = new List<GridGraphNode>();
	[SerializeField] public GameObject nodePrefab;

	public float generationGridCellSize = 1f;
	public float collisionCheckRadius = 0.4f;
	public float wallCheckDistance = 0.5f;
	public LayerMask obstacleMask;

	public int Count => nodes.Count;

	public void Clear()
	{
		nodes.Clear();
		gameObject.DestroyChildren();
	}

	public void Remove(GridGraphNode node)
	{
		if (node == null || !nodes.Contains(node)) return;

		foreach (GridGraphNode n in node.adjacencyList)
		{
			n.adjacencyList.Remove(node);
		}

		nodes.Remove(node);
	}

	public void GenerateGrid(bool checkCollisions = true)
	{
		Clear();

		List<List<GridGraphNode>> nodeGrid = new List<List<GridGraphNode>>();

		Vector3 origin = new Vector3(transform.position.x, 0f, transform.position.z);

		float step = generationGridCellSize;

		// fill nodeGrid by checking steps and making sure there is no colliders between nodes.

		// ---- Find bounds by probing in 4 directions ----
		int minX = 0, maxX = 0;
		int minZ = 0, maxZ = 0;

		// Expand +X
		while (!Physics.Raycast(origin + new Vector3(maxX * step, 0f, 0f),
								 Vector3.right,
								 wallCheckDistance,
								 obstacleMask))
		{
			maxX++;
		}

		// Expand -X
		while (!Physics.Raycast(origin + new Vector3(minX * step, 0f, 0f),
								 Vector3.left,
								 wallCheckDistance,
								 obstacleMask))
		{
			minX--;
		}

		// Expand +Z
		while (!Physics.Raycast(origin + new Vector3(0f, 0f, maxZ * step),
								 Vector3.forward,
								 wallCheckDistance,
								 obstacleMask))
		{
			maxZ++;
		}

		// Expand -Z
		while (!Physics.Raycast(origin + new Vector3(0f, 0f, minZ * step),
								 Vector3.back,
								 wallCheckDistance,
								 obstacleMask))
		{
			minZ--;
		}

		// ---- Generate nodes inside discovered bounds ----
		for (int x = minX; x <= maxX; x++)
		{
			List<GridGraphNode> column = new List<GridGraphNode>();
			nodeGrid.Add(column);

			for (int z = minZ; z <= maxZ; z++)
			{
				Vector3 worldPos = origin + new Vector3(x * step, 0f, z * step);

				bool blocked = false;

				if (checkCollisions)
					blocked = Physics.CheckSphere(worldPos, collisionCheckRadius, obstacleMask);

				if (!blocked)
				{
					GridGraphNode node = new GridGraphNode();
					node.transform.position = worldPos; // assumes this exists
					column.Add(node);
				}
				else
				{
					column.Add(null);
				}
			}
		}

		// ---- Build adjacency (4-directional) ----
		int width = nodeGrid.Count;
		int height = nodeGrid[0].Count;

		for (int x = 0; x < width; x++)
		{
			for (int z = 0; z < height; z++)
			{
				GridGraphNode current = nodeGrid[x][z];
				if (current == null) continue;

				// Left
				if (x > 0 && nodeGrid[x - 1][z] != null)
					current.adjacencyList.Add(nodeGrid[x - 1][z]);

				// Right
				if (x < width - 1 && nodeGrid[x + 1][z] != null)
					current.adjacencyList.Add(nodeGrid[x + 1][z]);

				// Down
				if (z > 0 && nodeGrid[x][z - 1] != null)
					current.adjacencyList.Add(nodeGrid[x][z - 1]);

				// Up
				if (z < height - 1 && nodeGrid[x][z + 1] != null)
					current.adjacencyList.Add(nodeGrid[x][z + 1]);
			}
		}
	}

	public List<GridGraphNode> GetNeighbors(GridGraphNode node)
	{
		return node.adjacencyList;
	}

	#region grid_generation_properties

#if UNITY_EDITOR
	[Header("Gizmos")]
	/// <summary>WARNING: This property is used by Gizmos only and is removed from the build. DO NOT reference it outside of Editor-Only code.</summary>
	public float _nodeGizmoRadius = 0.5f;
	/// <summary>WARNING: This property is used by Gizmos only and is removed from the build. DO NOT reference it outside of Editor-Only code.</summary>
	public Color _edgeGizmoColor = Color.white;

	private void OnDrawGizmos()
	{
		if (nodes == null) return;

		// nodes
		foreach (GridGraphNode node in nodes)
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
#endif
	#endregion
}
