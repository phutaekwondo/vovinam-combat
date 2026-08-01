using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private InputActionReference moveInput;
    [SerializeField] private InputActionReference sprintInput;
    [SerializeField] private InputActionReference jumpInput;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float walkSpeed = 5f; // unit / second
    [SerializeField] private float runVelocity = 10f; // unit / second
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -15f; // unit / second^2

    float blendVelocity = 0f;
    float blendRotation = 0f;
    float verticalVelocity = 0f;
    float acceleration = 10f;
    GameObject playerModel;
    InputAction jumpAction;


    private void Awake()
    {
        playerModel = animator.gameObject;
        blendRotation = playerModel.transform.eulerAngles.y;

        if (jumpInput != null)
        {
            jumpAction = jumpInput.action;
        }
        else if (TryGetComponent(out PlayerInput playerInput) && playerInput.actions != null)
        {
            jumpAction = playerInput.actions.FindAction("Jump");
        }
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

        applyJumpAndGravity();

        float moveDistance = Time.deltaTime * blendVelocity;
        Vector3 moveDirection = getCameraRelativeDirection(moveInputDirection);
        Vector3 moveVector = new Vector3(moveDirection.x, 0f, moveDirection.z) * moveDistance;
        moveVector.y = verticalVelocity * Time.deltaTime;
        characterController.Move(moveVector);

        applyRotation(moveDirection);
    }

    private void applyJumpAndGravity()
    {
        if (characterController.isGrounded)
        {
            animator.SetBool("Jump", false);
            animator.SetBool("FreeFall", false);

            if (verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            if (wasJumpPressed())
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                animator.SetBool("Jump", true);
            }
        }
        else if (verticalVelocity < 0f)
        {
            animator.SetBool("FreeFall", true);
        }

        verticalVelocity += gravity * Time.deltaTime;
    }

    private bool wasJumpPressed()
    {
        return jumpAction != null && jumpAction.WasPressedThisFrame();
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

    private void applyAnimation()
    {
        animator.SetFloat("velocity", blendVelocity / runVelocity);
        animator.SetBool("Grounded", characterController.isGrounded);
    }
}
