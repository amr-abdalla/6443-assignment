using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
	[Header("Drag")]
	public float dragSpeed = 0.7f;

	[Header("Zoom")]
	public float zoomSpeed = 5f;
	public float maxZoom = 20f;
	private Vector3 startPosition;

	[Header("Bounds (X,Z World Space)")]
	public Vector2 minBounds;
	public Vector2 maxBounds;

	private Camera cam;
	private Vector3 lastMouseWorldPos;
	private bool isDragging;

	private void Awake()
	{
		cam = GetComponent<Camera>();
		startPosition = transform.position;
	}

	void Update()
	{
		if (!Application.isPlaying) return;

		HandleDrag();
		HandleZoom();
	}

	void HandleDrag()
	{
		if (Mouse.current == null) return;

		if (Mouse.current.leftButton.wasPressedThisFrame)
		{
			Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

			if (Physics.Raycast(ray, out RaycastHit hit))
			{
				if (hit.transform.GetComponent<Draggable>() != null)
				{
					return;
				}
			}

			isDragging = true;
			lastMouseWorldPos = GetMouseWorldPosition();
		}

		if (Mouse.current.leftButton.wasReleasedThisFrame)
		{
			isDragging = false;
		}

		if (isDragging)
		{
			Vector3 currentMouseWorldPos = GetMouseWorldPosition();
			Vector3 delta = lastMouseWorldPos - currentMouseWorldPos;

			Vector3 newPosition = transform.position + delta * dragSpeed;

			newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
			newPosition.z = Mathf.Clamp(newPosition.z, minBounds.y, maxBounds.y);

			transform.position = newPosition;

			lastMouseWorldPos = currentMouseWorldPos;
		}
	}

	Vector3 GetMouseWorldPosition()
	{
		Vector2 mousePos = Mouse.current.position.ReadValue();
		Ray ray = cam.ScreenPointToRay(mousePos);

		Plane plane = new Plane(Vector3.up, Vector3.zero);

		if (plane.Raycast(ray, out float distance))
		{
			return ray.GetPoint(distance);
		}

		return Vector3.zero;
	}

	void HandleZoom()
	{
		if (Mouse.current == null) return;

		float scroll = Mouse.current.scroll.ReadValue().y;

		if (Mathf.Abs(scroll) < 0.01f)
			return;

		Vector3 pos = transform.position;

		if (Mathf.Abs(startPosition.y - (pos + transform.forward * scroll * zoomSpeed).y) > maxZoom)
		{
			return;
		}

		pos += transform.forward * scroll * zoomSpeed;

		transform.position = pos;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;

		Vector3 center = new Vector3(
			(minBounds.x + maxBounds.x) / 2f,
			transform.position.y,
			(minBounds.y + maxBounds.y) / 2f
		);

		Vector3 size = new Vector3(
			maxBounds.x - minBounds.x,
			0.1f,
			maxBounds.y - minBounds.y
		);

		Gizmos.DrawWireCube(center, size);
	}
}
