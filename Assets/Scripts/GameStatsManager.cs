using System.Collections.Generic;
using UnityEngine;

public class GameStatsManager : Singleton<GameStatsManager>
{
	[SerializeField] private Transform goal;
	[SerializeField] public List<Cover> allCovers { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		allCovers = new List<Cover>();
		Time.timeScale = 2f;
	}

	public void AddCover(Cover cover)
	{
		allCovers.Add(cover);
	}

	public Transform GetGoal() => goal;

}
