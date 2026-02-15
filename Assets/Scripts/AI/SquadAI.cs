using System;
using System.Collections.Generic;
using UnityEngine;

public class SquadAI : MonoBehaviour
{
	[SerializeField] private List<AIAgentDecisionMaker> members;
	public List<Cover> bookedCovers = new List<Cover>();
	public int finishedCount = 0;
	public Action OnFinishMoving;
	public Action<SquadAI> OnSquadEmptied;

	private void Start()
	{
		foreach (AIAgentDecisionMaker aIAgent in members)
		{
			aIAgent.squadAI = this;
		}
	}

	public void StartMoving()
	{
		bookedCovers.Clear();

		foreach (AIAgentDecisionMaker aIAgent in members)
		{
			aIAgent.isSearchingForGoal = true;
			aIAgent.SetNewGoal(bookedCovers);

			if (aIAgent.currentGoal.TryGetComponent(out Cover cover))
			{
				bookedCovers.Add(cover);
			}
		}

	}

	public void RemoveMember(AIAgentDecisionMaker aIAgent)
	{
		members.Remove(aIAgent);
		if (members.Count == 0)
		{
			OnSquadEmptied?.Invoke(this);
		}
	}

	public void CheckIfAllMembersFinished()
	{
		foreach (AIAgentDecisionMaker aIAgentDecisionMaker in members)
		{
			if (aIAgentDecisionMaker.occupiedCover == null)
			{
				return;
			}
		}

		OnFinishMoving?.Invoke();
	}

}
