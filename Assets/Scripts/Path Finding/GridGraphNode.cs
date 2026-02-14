using UnityEngine;

using System.Collections.Generic;

[ExecuteInEditMode]
public class GridGraphNode : MonoBehaviour
{
    [SerializeField] public List<GridGraphNode> adjacencyList = new List<GridGraphNode>();

    private GridGraph graph;
    private GridGraph Graph
    {
        get
        {
            if (graph == null)
                graph = GetComponentInParent<GridGraph>();

            return graph;
        }
    }

    private void OnDestroy()
    {
        if (Graph != null)
        {
            Graph.Remove(this);
        }
    }

#if UNITY_EDITOR
    public Color _nodeGizmoColor = new Color(Color.white.r, Color.white.g, Color.white.b, 0.5f);
#endif
}
