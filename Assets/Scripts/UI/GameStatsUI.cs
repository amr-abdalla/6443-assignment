using TMPro;
using UnityEngine;

public class GameStatsUI : MonoBehaviour
{
	[SerializeField] TMP_Text tmp;

	void Start()
    {
		UpdateUI();
		GameStatsManager.Instance.OnGameStatsChanged += UpdateUI;
	}

	private void UpdateUI()
	{
		tmp.text =
			$"Game Stats: \n" +
			$"\t Soldiers Count: {GameStatsManager.Instance.TotalSoldiers} \n" +
			$"\t Survivors Count: {GameStatsManager.Instance.SurvivalCount} \n" +
			$"\t Death Count: {GameStatsManager.Instance.DeathCount}";
	}

}
