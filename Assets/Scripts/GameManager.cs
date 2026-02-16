using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	[Button]
	public void StartSimulation()
	{
		GridGraph.Instance.GenerateGrid();
		GameStatsManager.Instance.initCovers();
		SquadsManager.Instance.StartSimulation();
	}

	[Button]
	public void RestartLevel()
	{
		SceneManager.LoadScene(0);
	}

	public void ExitGame()
	{
		Application.Quit();
	}
}
