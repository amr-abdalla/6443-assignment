using UnityEngine;
using UnityEngine.InputSystem;

public class GamePauseUI : MonoBehaviour
{
	[SerializeField] private GameObject pauseUI;
	[SerializeField] private InputAction pauseAction;
	private bool isPaused = false;
	public static bool canPause = true;

	private void Awake()
	{
		canPause = true;
		Time.timeScale = 1;
	}

	private void OnEnable()
	{
		pauseAction.Enable();
		pauseAction.performed += OnPause;
	}

	private void OnDisable()
	{
		pauseAction.performed -= OnPause;
		pauseAction.Disable();
	}

	private void OnPause(InputAction.CallbackContext context)
	{
		TogglePause();
	}

	public void TogglePause()
	{
		if (!canPause)
		{
			return;
		}

		isPaused = !isPaused;

		pauseUI.SetActive(isPaused);
		Time.timeScale = isPaused ? 0f : 1f;
	}
}
