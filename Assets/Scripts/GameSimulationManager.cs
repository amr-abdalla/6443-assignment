using NaughtyAttributes;
using UnityEngine;

public class GameSimulationManager : MonoBehaviour
{
	[Button]
	public void StartSimulation()
	{
		GridGraph.Instance.GenerateGrid();
		GameStatsManager.Instance.initCovers();
		SquadsManager.Instance.StartSimulation();
	}
}
