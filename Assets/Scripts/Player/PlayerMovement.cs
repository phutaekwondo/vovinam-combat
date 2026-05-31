using UnityEngine;
using UnityEngine.InputSystem;

enum PlayerAnimationIndex
{
    Idle = 0,
    Walk = 1,
    WalkBack = 2,
    WalkRight = 3,
    WalkLeft = 4,
}

public class PlayerMovement : MonoBehaviour
{
    static readonly int AnimIndexHash = Animator.StringToHash("animIndex");

    [SerializeField] float moveSpeed = 5f;
    [SerializeField] Animator animator;

    PlayerAnimationIndex currentAnimation = PlayerAnimationIndex.Idle;

    void Update()
    {
        Vector2 input = ReadInput();
        Vector3 direction = new Vector3(input.x, 0f, input.y);
        if (direction.sqrMagnitude > 1f)
            direction.Normalize();

        transform.position += direction * (moveSpeed * Time.deltaTime);
        UpdateAnimation(input);
    }

    void UpdateAnimation(Vector2 input)
    {
        if (animator == null)
            return;

        int animation = (int)GetAnimation(input);
        int currentAnimation = (int)this.currentAnimation;
        if (currentAnimation == animation)
        {
            return;
        }

        this.currentAnimation = (PlayerAnimationIndex)animation;
        animator.SetInteger(AnimIndexHash, animation);
    }

    static PlayerAnimationIndex GetAnimation(Vector2 input)
    {
        if (input.sqrMagnitude < 0.01f)
            return PlayerAnimationIndex.Idle;

        if (Mathf.Abs(input.y) >= Mathf.Abs(input.x))
            return input.y > 0f ? PlayerAnimationIndex.Walk : PlayerAnimationIndex.WalkBack;

        return input.x > 0f ? PlayerAnimationIndex.WalkRight : PlayerAnimationIndex.WalkLeft;
    }

    Vector2 ReadInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
            return Vector2.zero;

        float horizontal = 0f;
        float vertical = 0f;

        if (keyboard.aKey.isPressed) horizontal -= 1f;
        if (keyboard.dKey.isPressed) horizontal += 1f;
        if (keyboard.sKey.isPressed) vertical -= 1f;
        if (keyboard.wKey.isPressed) vertical += 1f;

        return new Vector2(horizontal, vertical);
    }
}
