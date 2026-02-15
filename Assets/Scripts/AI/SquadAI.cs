using System.Collections.Generic;
using UnityEngine;

public class SquadAI : MonoBehaviour
{
	[SerializeField] private List<AIAgentDecisionMaker> members;
	public List<Cover> bookedCovers = new List<Cover>();
	public int finishedCount = 0;

	private void Start()
	{
		foreach (AIAgentDecisionMaker aIAgent in members)
		{
			aIAgent.squadAI = this;
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

		bookedCovers.Clear();

		foreach (AIAgentDecisionMaker aIAgent in members)
		{
			aIAgent.SetNewGoal(bookedCovers);

			if (aIAgent.currentGoal != null && aIAgent.currentGoal.TryGetComponent(out Cover cover))
			{
				bookedCovers.Add(cover);
			}
		}
	}

}
