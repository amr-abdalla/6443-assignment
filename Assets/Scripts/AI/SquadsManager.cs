using System;
using System.Collections.Generic;
using UnityEngine;

public class SquadsManager : MonoBehaviour
{
	[SerializeField] private List<SquadAI> squads;
	private int currentMovingSquad = -1;

	void Start()
	{
		foreach(SquadAI squad in squads)
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
			Debug.Log("Game Complete");
			return;
		}

		currentMovingSquad = (currentMovingSquad + 1) % squads.Count;

		squads[currentMovingSquad].StartMoving();
	}

}
