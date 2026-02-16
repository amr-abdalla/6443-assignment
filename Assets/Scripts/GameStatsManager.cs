using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStatsManager : Singleton<GameStatsManager>
{
	private const int _StartingSoldiersCount = 24;
	[SerializeField] private Transform goal;
	[SerializeField] public List<Cover> allCovers { get; private set; }
	private int survivalCount = 0;
	private int deathCount = 0;
	public Action OnGameStatsChanged;

	public int SurvivalCount
	{
		get
		{
			return survivalCount;
		}
		set
		{
			survivalCount = value;
			OnGameStatsChanged?.Invoke();
		}
	}

	public int DeathCount
	{
		get
		{
			return deathCount;
		}
		set
		{
			deathCount = value;
			OnGameStatsChanged?.Invoke();
		}
	}

	public int TotalSoldiers => _StartingSoldiersCount - SurvivalCount - DeathCount;

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

	public void initCovers()
	{
		foreach(Cover cover in allCovers)
		{
			cover.StartSimulation();
		}
	}
}
