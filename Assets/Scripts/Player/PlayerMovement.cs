using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private InputActionReference moveInput;
    [SerializeField] private InputActionReference sprintInput;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float walkSpeed = 5f; // unit / second
    [SerializeField] private float runVelocity = 10f; // unit / second
    [SerializeField] private float gravity = -5f; // unit / second

    float blendVelocity = 0f;
    float blendRotation = 0f;
    float acceleration = 10f;
    GameObject playerModel;


    private void Awake()
    {
        playerModel = animator.gameObject;
        blendRotation = playerModel.transform.eulerAngles.y;
    }

    private void Update()
    {
        applyMovement();
        applyAnimation();
    }

    private void applyMovement()
    {
        Vector2 moveInputDirection = moveInput.action.ReadValue<Vector2>();
        float targetVelocity = getTargetVelocity(moveInputDirection);
        blendVelocity = Mathf.Lerp(blendVelocity, targetVelocity, Time.deltaTime * acceleration);

        float moveDistance = Time.deltaTime * blendVelocity;
        float gravity = getGravity();
        Vector3 moveDirection = getCameraRelativeDirection(moveInputDirection);
        Vector3 moveVector = new Vector3(moveDirection.x, gravity, moveDirection.z) * moveDistance;
        characterController.Move(moveVector);

        applyRotation(moveDirection);
    }

    private Vector3 getCameraRelativeDirection(Vector2 moveInputDirection)
    {
        if (cameraTransform == null)
        {
            return new Vector3(moveInputDirection.x, 0f, moveInputDirection.y);
        }

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        return forward * moveInputDirection.y + right * moveInputDirection.x;
    }

    private void applyRotation(Vector3 moveDirection)
    {
        if (moveDirection.sqrMagnitude > 0.1f)
        {
            float targetRotation = getTargetRotation(moveDirection);

            blendRotation = Mathf.Lerp(blendRotation, targetRotation, Time.deltaTime * acceleration);
            playerModel.transform.rotation = Quaternion.Euler(0f, blendRotation, 0f);
        }
    }

    private float getTargetRotation(Vector3 moveDirection)
    {
        float currentRotation = blendRotation;
        float targetRotation = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

        while (Mathf.Abs(targetRotation - currentRotation) > 180f)
        {
            if (targetRotation - currentRotation > 180f)
            {
                targetRotation -= 360f;
            }
            else
            {
                targetRotation += 360f;
            }
        }

        return targetRotation;
    }

    private float getTargetVelocity(Vector2 moveInput)
    {
        if (moveInput.sqrMagnitude > 0.1f)
        {
            bool isSprinting = sprintInput.action.ReadValue<float>() > 0.5f;
            float velocity = isSprinting ? runVelocity : walkSpeed;
            return velocity;
        }

        return 0f;
    }

    private float getGravity()
    {
        return characterController.isGrounded ? 0f : gravity;
    }

    private void applyAnimation()
    {
        animator.SetFloat("velocity", blendVelocity / runVelocity);
    }
}
