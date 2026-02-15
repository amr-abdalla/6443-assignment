using UnityEngine;

public class FollowUI : MonoBehaviour
{
	[Header("Target")]
	[SerializeField] private Transform target;
	[SerializeField] private Vector3 worldOffset = new Vector3(0, 12.5f, 0);


	[Header("Zoom Scaling")]
	[SerializeField] private float scaleMultiplier = 20f;
	[SerializeField] private float minScale = 0.5f;
	[SerializeField] private float maxScale = 2f;

	private RectTransform rectTransform;
	private Camera worldCamera;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();

		if (worldCamera == null)
			worldCamera = Camera.main;
	}

	private void LateUpdate()
	{
		if (target == null)
			return;

		Vector3 worldPosition = target.position + worldOffset;
		Vector3 screenPosition = worldCamera.WorldToScreenPoint(worldPosition);

		if (screenPosition.z < 0)
		{
			gameObject.SetActive(false);
			return;
		}

		gameObject.SetActive(true);
		rectTransform.position = screenPosition;

		float distance = Vector3.Distance(worldCamera.transform.position, target.position);
		float scale = 1f * scaleMultiplier / distance;
		scale = Mathf.Clamp(scale, minScale, maxScale);
		rectTransform.localScale = Vector3.one * scale;
	}

}
