using UnityEngine;
using UnityEngine.InputSystem;

public class Draggable : MonoBehaviour
{
    private Camera mainCamera;
    public static bool isDragging;

    private Plane dragPlane;
    private Vector3 offset;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryStartDrag();
        }

        if (Mouse.current.leftButton.isPressed && isDragging)
        {
            Drag();
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }
    }

    void TryStartDrag()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                isDragging = true;

                // Create a horizontal plane at Y = 0
                dragPlane = new Plane(Vector3.up, Vector3.zero);

                if (dragPlane.Raycast(ray, out float enter))
                {
                    Vector3 hitPoint = ray.GetPoint(enter);
                    offset = transform.position - hitPoint;
                }
            }
        }
    }

    void Drag()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 newPosition = hitPoint + offset;

            newPosition.y = 0f; // 🔒 Lock Y

            transform.position = newPosition;
        }
    }
}
