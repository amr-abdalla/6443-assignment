using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
	[SerializeField] private AgentHealth health;
	[SerializeField] private Slider healthSlider;
	[SerializeField] private Color goodHealthColor;
	[SerializeField] private Color midHealthColor;
	[SerializeField] private Color lowHealthColor;

	void Awake()
	{
		health.OnHealthChanged += OnHealthChanged;
	}

	private void OnHealthChanged(float current, float max)
	{
		healthSlider.value = current / max;
		SetSliderColor(health.GetHealthStatus());
	}

	private void SetSliderColor(AgentStats.HealthStatus healthStatus)
	{
		ColorBlock colors = healthSlider.colors;

		if (healthStatus == AgentStats.HealthStatus.Good)
		{
			colors.disabledColor = goodHealthColor;
		}
		else if (healthStatus == AgentStats.HealthStatus.Mid)
		{
			colors.disabledColor = midHealthColor;
		}
		else if (healthStatus == AgentStats.HealthStatus.Low)
		{
			colors.disabledColor = lowHealthColor;
		}

		healthSlider.colors = colors;
	}

}
