using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform target;
    [SerializeField] private InputActionReference lookInput;
    [SerializeField] private float distance = 3f;
    [SerializeField] private float sensitivity = 0.15f;
    [SerializeField] private float minPitch = -20f;
    [SerializeField] private float maxPitch = 70f;
    [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1.5f, 0f);

    private InputAction lookAction;
    private float yaw;
    private float pitch;

    private void Awake()
    {
        if (lookInput != null)
        {
            lookAction = lookInput.action;
            return;
        }

        if (TryGetComponent(out PlayerInput playerInput) && playerInput.actions != null)
        {
            lookAction = playerInput.actions.FindAction("Look");
        }
    }

    private void Start()
    {
        if (target == null || _camera == null)
        {
            return;
        }

        Vector3 focusPoint = target.position + targetOffset;
        Vector3 offset = _camera.transform.position - focusPoint;
        if (offset.sqrMagnitude > 0.001f)
        {
            distance = offset.magnitude;
            yaw = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;
            pitch = Mathf.Asin(Mathf.Clamp(offset.y / distance, -1f, 1f)) * Mathf.Rad2Deg;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (target == null || _camera == null)
        {
            return;
        }

        Vector2 look = lookAction != null ? lookAction.ReadValue<Vector2>() : Vector2.zero;
        yaw += look.x * sensitivity;
        pitch -= look.y * sensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Vector3 focusPoint = target.position + targetOffset;
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        _camera.transform.position = focusPoint - rotation * Vector3.forward * distance;
        _camera.transform.LookAt(focusPoint);
    }
}
