using UnityEngine;

[CreateAssetMenu(fileName = "AgentStats", menuName = "Stats/Agent Stats")]
public class AgentStats : ScriptableObject
{
    public enum HealthStatus
	{
        Good,
        Mid,
        Low
	}

    [SerializeField] private float maxHP;
    [SerializeField] [Range(0, 1)] private float goodHealthPercentage;
    [SerializeField] [Range(0, 1)] private float midHealthPercentage;

    [SerializeField] private float speed;

    public float GetMaxHP() => maxHP;
    public float GetSpeed() => speed;

    public HealthStatus GetHealthStatus(float currentHP)
	{
        float percentage = currentHP / maxHP;

        if (percentage >= goodHealthPercentage)
		{
            return HealthStatus.Good;
		}

        if (percentage >= midHealthPercentage)
		{
            return HealthStatus.Mid;
		}

        return HealthStatus.Low;
	}
}
