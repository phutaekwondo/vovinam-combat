using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private InputActionReference moveInput;
    [SerializeField] private InputActionReference sprintInput;
    [SerializeField] private float walkSpeed = 5f; // unit / second
    [SerializeField] private float runVelocity = 10f; // unit / second
    [SerializeField] private float gravity = -5f; // unit / second

    float blendVelocity = 0f;
    private float acceleration = 10f;
    private float sqrRunVelocity = 100f;

    private void Awake()
    {
        sqrRunVelocity = runVelocity * runVelocity;
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
        Vector3 moveVector = new Vector3(moveInputDirection.x, gravity, moveInputDirection.y) * moveDistance;
        characterController.Move(moveVector);
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