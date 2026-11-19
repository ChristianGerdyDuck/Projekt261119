using UnityEngine;
using UnityEngine.InputSystem;

public enum Controls
{
    MouseRotation, FollowPlayer
}
public class ThirdPersonMotor : MonoBehaviour
{
    public Controls controls = Controls.MouseRotation;
    public ThirdPersonCamera orbitCamera;
    public float Speed = 6f;
    public float TurnSpeed = 640f;
    public float FollowTurnSpeed = 240f;
    public float JumpHeight = 1.6f;
    public float Gravity = -20f;
    public NpcDialogue ActiveDialogue;
    public bool IsInDialogue => ActiveDialogue != null;
    public GoalZone Goal;
    private float verticalSpeed; 
    private CharacterController controller;
    public bool TryBeginDialogue(NpcDialogue dialogue)
    {
        if (dialogue == null || IsInDialogue)
            return false;

        ActiveDialogue = dialogue;
        return true;
    }

    public void EndDialogue(NpcDialogue dialogue)
    {
        if (ActiveDialogue == dialogue)
            ActiveDialogue = null;
    }
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (IsInDialogue || Goal.HasWon)
            return;

        Keyboard keyboard = Keyboard.current;
        Vector2 input = ReadMovement(keyboard);
        bool jumpPressed = keyboard?.spaceKey.wasPressedThisFrame == true;
        float dt = Time.deltaTime;
        Vector3 direction = GetMoveDirection(input, dt);

        UpdateVerticalSpeed(jumpPressed, dt);
        Move(direction, dt);
    }

    private Vector2 ReadMovement(Keyboard keys)
    {
        if (keys == null)
            return Vector2.zero;

        float x = (keys.dKey.isPressed ? 1f : 0f) - (keys.aKey.isPressed ? 1f : 0f);
        float z = (keys.wKey.isPressed ? 1f : 0f) - (keys.sKey.isPressed ? 1f : 0f);
        return new Vector2(x, z);
    }

    private void Jump()
    {
        verticalSpeed = Mathf.Sqrt(-2f * Gravity * JumpHeight);
    }

    private void Move(Vector3 direction, float dt)
    {
        Vector3 velocity = direction * Speed + Vector3.up * verticalSpeed;
        CollisionFlags collisions = controller.Move(velocity * dt);

        if ((collisions & CollisionFlags.Above) != 0 && verticalSpeed > 0f)
        {
            verticalSpeed = 0;
        }
    }

    private Vector3 GetMoveDirection(Vector2 input, float dt)
    {
        if(controls == Controls.FollowPlayer)
        {
            transform.Rotate(0f, input.x * FollowTurnSpeed * dt, 0f, Space.World);
            return transform.forward * input.y;
        }

        Vector2 moveInput = Vector2.ClampMagnitude(input, 1f);
        Vector3 direction = orbitCamera.Heading * new Vector3(moveInput.x, 0f, moveInput.y);
        TurnTowards(direction, dt);
        return direction;
    }
    
    private void TurnTowards(Vector3 direction, float dt)
    {
        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion target = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target, TurnSpeed * dt);
    }
    
    private void UpdateVerticalSpeed(bool jumpPressed, float dt)
    {
        if(controller.isGrounded && verticalSpeed <= 0f)
        {
            verticalSpeed = -2f;

            if (jumpPressed)
                Jump();
        }

        verticalSpeed += Gravity * dt;
    }
}
