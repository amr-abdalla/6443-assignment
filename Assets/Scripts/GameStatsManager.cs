using UnityEngine;

public class GameStatsManager : Singleton<GameStatsManager>
{
	[SerializeField] private Transform goal;

	public Transform GetGoal() => goal;

}
