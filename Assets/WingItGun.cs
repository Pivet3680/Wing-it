using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(100)]
public sealed class WingItGun : MonoBehaviour
{
    [SerializeField] private Camera aimCamera;
    [SerializeField] private Transform barrel;
    [SerializeField, Min(0f)] private float muzzleDistance = 0.9f;
    [SerializeField, Min(0f)] private float projectileSpeed = 18f;
    [SerializeField, Min(0f)] private float projectileLifetime = 3f;
    [SerializeField, Min(0.05f)] private float fireInterval = 0.25f;

    private InputAction shootAction;
    private bool shotQueued;
    private bool holdingFire;
    private float nextShotTime;

    public Vector2 AimDirection { get; private set; } = Vector2.right;

    private void Awake()
    {
        if (aimCamera == null)
            aimCamera = Camera.main;

        if (aimCamera == null)
        {
            Debug.LogError("WingItGun requires a camera for mouse aiming.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        shootAction = new InputAction("Shoot", InputActionType.Button, "<Mouse>/leftButton");
        shootAction.performed += OnShoot;
        shootAction.canceled += OnShootReleased;
        shootAction.Enable();
        nextShotTime = 0f;
    }

    private void OnDisable()
    {
        if (shootAction != null)
        {
            shootAction.performed -= OnShoot;
            shootAction.canceled -= OnShootReleased;
            shootAction.Disable();
            shootAction.Dispose();
            shootAction = null;
        }

        shotQueued = false;
        holdingFire = false;
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        shotQueued = true;
        holdingFire = true;
    }

    private void OnShootReleased(InputAction.CallbackContext context) => holdingFire = false;

    private void LateUpdate()
    {
        if (Mouse.current == null || aimCamera == null)
            return;

        Ray ray = aimCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane movementPlane = new Plane(Vector3.forward, transform.position);
        if (movementPlane.Raycast(ray, out float distance))
        {
            Vector3 aimPoint = ray.GetPoint(distance);
            Vector2 direction = aimPoint - transform.position;
            direction.x = Mathf.Max(0f, direction.x);
            if (direction.sqrMagnitude > 0.0001f)
                AimDirection = direction.normalized;
            else
                AimDirection = Vector2.right;
        }

        if (barrel != null)
        {
            barrel.up = AimDirection;
            barrel.position = transform.position + (Vector3)AimDirection * 0.55f;
        }

        if ((shotQueued || holdingFire) && Time.time >= nextShotTime)
        {
            Fire();
            nextShotTime = Time.time + fireInterval;
        }

        shotQueued = false;
    }

    private void Fire()
    {
        GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectile.name = "Prototype projectile";
        projectile.transform.position = transform.position + (Vector3)AimDirection * muzzleDistance;
        projectile.transform.localScale = Vector3.one * 0.22f;

        Collider projectileCollider = projectile.GetComponent<Collider>();
        foreach (Collider playerCollider in GetComponentsInChildren<Collider>())
            Physics.IgnoreCollision(projectileCollider, playerCollider);

        Renderer renderer = projectile.GetComponent<Renderer>();
        var color = new MaterialPropertyBlock();
        color.SetColor("_BaseColor", Color.yellow);
        color.SetColor("_Color", Color.yellow);
        renderer.SetPropertyBlock(color);

        Rigidbody projectileBody = projectile.AddComponent<Rigidbody>();
        projectileBody.useGravity = false;
        projectileBody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        projectileBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        projectileBody.linearVelocity = (Vector3)AimDirection * projectileSpeed;
        projectile.AddComponent<WingItProjectile>().SetLifetime(projectileLifetime);
    }
}
