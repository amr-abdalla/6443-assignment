using System.Collections.Generic;
using UnityEngine;

public class SquadsManager : Singleton<SquadsManager>
{
	[SerializeField] private List<SquadAI> squads;
	[SerializeField] private GameObject gameCompleteUI;
	private int currentMovingSquad = -1;

	public void StartSimulation()
	{
		foreach (SquadAI squad in squads)
		{
			squad.OnFinishMoving += OnSquadFinishMoving;
			squad.OnSquadEmptied += OnSquadEmptied;
		}

		StartMoving();
	}

	private void OnSquadFinishMoving()
	{
		StartMoving();
	}

	private void OnSquadEmptied(SquadAI squad)
	{
		currentMovingSquad--;
		squads.Remove(squad);
		Destroy(squad.gameObject);
	}

	private void StartMoving()
	{
		if (squads.Count == 0)
		{
			gameCompleteUI.SetActive(true);
			GamePauseUI.canPause = false;
			Time.timeScale = 0;
			return;
		}

		currentMovingSquad = (currentMovingSquad + 1) % squads.Count;

		squads[currentMovingSquad].StartMoving();
	}

}
