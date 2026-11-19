using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class ThirdPersonCamera : MonoBehaviour
{
    public ThirdPersonMotor PlayerMotor;
    public GoalZone Goal;
    public Transform Target;
    public Transform FollowAnchor;
    public bool avoidObstacles = true;
    public LayerMask ObstacleMask;
    #region Floats
    [Min(0f)] public float Clearance = 0.05f;
    [Min(0f)] public float ReturnSpeed = 6f;
    [Min(0.01f)] public float FollowTurnSmoothTime = 0.25f;
    [Min(0.01f)] public float Radius = 0.2f;
    [Min(0.1f)] public float OrbitDistance = 6f;
    public float Sensitivity = 0.2f;
    private float yaw;
    private float pitch = 20f;
    private float CurrentDistance = float.PositiveInfinity;
    private float recenterYaw, yawVelocity;
    #endregion
    private bool recentering, wasOrbiting;
    public Quaternion Heading => Quaternion.Euler(0f, yaw, 0f);
    private bool WasInDialogue;
    private Quaternion RotationBeforeDialogue;

    private void Awake()
    {
        yaw = PlayerMotor.transform.eulerAngles.y;
    }

    private void Update()
    {
        if (PlayerMotor.IsInDialogue || Goal.HasWon)
            return;
        bool orbiting = PlayerMotor.controls == Controls.MouseRotation;

        if(orbiting != wasOrbiting)
        {
            wasOrbiting = orbiting;
            recentering = false;
            yawVelocity = 0f;

            if (orbiting)
                yaw = PlayerMotor.transform.eulerAngles.y;

            SetCursorLock(orbiting);
            return;
        }

        if (!orbiting)
            return;

        if (Keyboard.current?.qKey.wasPressedThisFrame == true)
        {
            recenterYaw = PlayerMotor.transform.eulerAngles.y;
            recentering = true;
            yawVelocity = 0f;
        }

        ReadOrbitInput();
        UpdateRecenter();
    }

    private void LateUpdate()
    {
        NpcDialogue dia = PlayerMotor.ActiveDialogue;
        if(dia != null)
        {
            if(!WasInDialogue)
            {
                RotationBeforeDialogue = transform.rotation;
                WasInDialogue = true;
            }

            Vector3 direction = dia.CameraLookPosition - transform.position;

            if(direction.sqrMagnitude > 0.0001f)
            {
                Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 240f * Time.deltaTime);
            }

            return;
        }

        if(WasInDialogue)
        {
            transform.rotation = RotationBeforeDialogue;
            WasInDialogue = false;
        }

        Vector3 pivot = Target.position;
        Vector3 offset = GetCameraOffset();
        float desiredDistance = offset.magnitude;

        if (desiredDistance < 0.001f)
        {
            CurrentDistance = 0f;
            transform.position = pivot;
            return;
        }

        Vector3 backwards = offset / desiredDistance;
        CurrentDistance = avoidObstacles ? ResolveDistance(pivot, backwards, desiredDistance) : desiredDistance;
        Vector3 cameraPosition = pivot + backwards * CurrentDistance;
        Vector3 lookDirection = pivot - cameraPosition;

        if (lookDirection.sqrMagnitude < 0.00001f)
            lookDirection = -backwards;

        transform.SetPositionAndRotation(cameraPosition, Quaternion.LookRotation(lookDirection, Vector3.up));
    }

    private Vector3 GetCameraOffset()
    {
        if (PlayerMotor.controls == Controls.FollowPlayer)
            return FollowAnchor.position - Target.position;

        Quaternion orbit = Quaternion.Euler(pitch, yaw, 0f);
        return orbit * Vector3.back * OrbitDistance;
    }

    private void ReadOrbitInput()
    {
        Mouse mouse = Mouse.current;

        if (mouse == null)
            return;

        if (Keyboard.current?.escapeKey.wasPressedThisFrame == true)
        {
            SetCursorLock(false);
            return;
        }

        if (Cursor.lockState != CursorLockMode.Locked)
        {
            if (mouse.leftButton.wasPressedThisFrame)
                SetCursorLock(true);

            return;
        }

        Vector2 delta = mouse.delta.ReadValue();

        if (delta.sqrMagnitude >= 1f)
        {
            recentering = false;
            yawVelocity = 0f;
        }

        yaw += delta.x * Sensitivity;
        pitch = Mathf.Clamp(pitch - delta.y * Sensitivity, -80f, 80f);
    }

    private void UpdateRecenter()
    {
        if (recentering && Time.deltaTime > 0f)
        {
            yaw = Mathf.SmoothDampAngle(yaw, recenterYaw, ref yawVelocity, FollowTurnSmoothTime);
        }
        if (Mathf.Abs(Mathf.DeltaAngle(yaw, recenterYaw)) < 0.1f)
        {
            yaw = recenterYaw;
            recentering = false;
            yawVelocity = 0f;
        }
    }

    private void OnDisable()
    {
        SetCursorLock(false);
        wasOrbiting = false;
    }

    private void SetCursorLock(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    private float ResolveDistance(Vector3 pivot, Vector3 backwards, float desiredDistance)
    {
        float allowed = GetAllowedDistance(pivot, backwards, desiredDistance);
        float nextDistance = CurrentDistance + ReturnSpeed * Time.deltaTime;
        return Mathf.Min(allowed, nextDistance);
    }

    private float GetAllowedDistance(Vector3 Pivot, Vector3 backwards, float desiredDistance)
    {
        if (Physics.CheckSphere(Pivot, Radius, ObstacleMask, QueryTriggerInteraction.Ignore))
            return 0f;

        if (Physics.SphereCast(Pivot, Radius, backwards, out RaycastHit hit, desiredDistance, ObstacleMask, QueryTriggerInteraction.Ignore))
            return Mathf.Max(0f, hit.distance - Clearance);
        return desiredDistance;
    }
}
