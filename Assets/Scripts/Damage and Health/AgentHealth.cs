using System;
using UnityEngine;

public class AgentHealth : MonoBehaviour
{
    [SerializeField] private AgentStats stats;
    private float currentHP;
	public Action<float, float> OnHealthChanged;

	private void Start()
	{
		currentHP = stats.GetMaxHP();
		OnHealthChanged?.Invoke(currentHP, stats.GetMaxHP());
	}

	public void TakeDamage(float damage)
	{
		currentHP -= damage;
		OnHealthChanged?.Invoke(currentHP, stats.GetMaxHP());

		if (currentHP <= 0)
		{
			Die();
		}
	}

	public void Heal(float healAmount)
	{
		currentHP = MathF.Min(currentHP + healAmount, stats.GetMaxHP());
		OnHealthChanged?.Invoke(currentHP, stats.GetMaxHP());
	}

	public AgentStats.HealthStatus GetHealthStatus() => stats.GetHealthStatus(currentHP);

	private void Die()
	{
		Destroy(gameObject);
	}

}
