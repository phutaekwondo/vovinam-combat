using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private InputActionReference moveInput;
    [SerializeField] private InputActionReference sprintInput;
    [SerializeField] private float walkSpeed = 5f; // unit / second
    [SerializeField] private float runSpeed = 10f; // unit / second

    private void Update()
    {
        Vector2 moveInputDirection = moveInput.action.ReadValue<Vector2>();
        bool isSprinting = sprintInput.action.ReadValue<float>() > 0.5f;
        float speed = isSprinting ? this.runSpeed : this.walkSpeed;
        Vector3 moveVector = new Vector3(moveInputDirection.x, 0, moveInputDirection.y) * Time.deltaTime * speed;
        this.characterController.Move(moveVector);
    }
}