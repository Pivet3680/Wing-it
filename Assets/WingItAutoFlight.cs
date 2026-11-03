using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public sealed class WingItAutoFlight : MonoBehaviour
{
    [SerializeField, Min(0f)] private float forwardSpeed = 5f;
    [SerializeField, Min(0f)] private float normalJumpHeight = 2.5f;
    [SerializeField, Min(0f)] private float holdJumpHeight = 0.8f;
    [SerializeField, Min(0f)] private float holdToGlideSeconds = 0.35f;
    [SerializeField, Min(0f)] private float glideFallSpeed = 2f;
    [SerializeField, Min(1f)] private float glideForwardSpeedMultiplier = 1.5f;

    private Rigidbody body;
    private InputAction flapAction;
    private float queuedJumpHeight = -1f;
    private bool spaceHeld;
    private bool glideStarted;
    private float spacePressedAt;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        body.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void OnEnable()
    {
        flapAction = new InputAction("Flap", InputActionType.Button, "<Keyboard>/space");
        flapAction.performed += OnFlap;
        flapAction.canceled += OnFlapReleased;
        flapAction.Enable();
    }

    private void OnDisable()
    {
        if (flapAction != null)
        {
            flapAction.performed -= OnFlap;
            flapAction.canceled -= OnFlapReleased;
            flapAction.Disable();
            flapAction.Dispose();
            flapAction = null;
        }

        queuedJumpHeight = -1f;
        spaceHeld = false;
        glideStarted = false;
        if (body != null)
            body.useGravity = true;
    }

    private void OnFlap(InputAction.CallbackContext context)
    {
        spaceHeld = true;
        glideStarted = false;
        spacePressedAt = Time.time;
    }

    private void OnFlapReleased(InputAction.CallbackContext context)
    {
        if (!spaceHeld)
            return;

        if (!glideStarted)
            queuedJumpHeight = Time.time - spacePressedAt < holdToGlideSeconds
                ? normalJumpHeight
                : holdJumpHeight;

        spaceHeld = false;
        glideStarted = false;
    }

    private void FixedUpdate()
    {
        Vector3 velocity = body.linearVelocity;
        if (spaceHeld && !glideStarted && Time.time - spacePressedAt >= holdToGlideSeconds)
        {
            glideStarted = true;
            queuedJumpHeight = holdJumpHeight;
        }

        bool gliding = spaceHeld && glideStarted;
        body.useGravity = !gliding;
        velocity.x = forwardSpeed * (gliding ? glideForwardSpeedMultiplier : 1f);

        if (queuedJumpHeight >= 0f)
        {
            velocity.y = Mathf.Sqrt(2f * Mathf.Max(0f, -Physics.gravity.y) * queuedJumpHeight);
            queuedJumpHeight = -1f;
        }

        if (gliding)
            velocity.y = Mathf.Max(velocity.y + Physics.gravity.y * Time.fixedDeltaTime, -glideFallSpeed);

        body.linearVelocity = velocity;
    }
}
