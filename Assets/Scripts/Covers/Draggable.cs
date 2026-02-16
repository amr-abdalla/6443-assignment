using UnityEngine;
using UnityEngine.InputSystem;

public class Draggable : MonoBehaviour
{
	private Camera mainCamera;
	public static Draggable currentlyDragging;

	private Plane dragPlane;
	private Vector3 offset;

	private void Awake()
	{
		mainCamera = Camera.main;
	}

	private void Update()
	{
		if (Mouse.current == null)
			return;

		if (Mouse.current.leftButton.wasPressedThisFrame)
		{
			TryStartDrag();
		}

		if (Mouse.current.leftButton.isPressed && currentlyDragging == this)
		{
			Drag();
		}

		if (Mouse.current.leftButton.wasReleasedThisFrame)
		{
			currentlyDragging = null;
		}
	}

	private void TryStartDrag()
	{
		Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

		if (Physics.Raycast(ray, out RaycastHit hit))
		{
			if (hit.transform == transform)
			{
				currentlyDragging = this;

				dragPlane = new Plane(Vector3.up, Vector3.zero);

				if (dragPlane.Raycast(ray, out float enter))
				{
					Vector3 hitPoint = ray.GetPoint(enter);
					offset = transform.position - hitPoint;
				}
			}
		}
	}

	private void Drag()
	{
		Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

		if (dragPlane.Raycast(ray, out float enter))
		{
			Vector3 hitPoint = ray.GetPoint(enter);
			Vector3 newPosition = hitPoint + offset;

			newPosition.y = 0f;

			transform.position = newPosition;
		}
	}
}
