using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public sealed class WingItEnemy : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField, Min(0f)] private float activationDistance = 14f;
    [SerializeField, Min(0f)] private float approachSpeed = 1.5f;
    [SerializeField, Min(0f)] private float verticalFollowSpeed = 1f;
    [SerializeField, Min(0f)] private float attackRange = 10f;
    [SerializeField, Min(0.1f)] private float attackInterval = 2f;
    [SerializeField, Min(0f)] private float projectileSpeed = 8f;
    [SerializeField, Min(0f)] private float offscreenDespawnSeconds = 5f;

    private Rigidbody body;
    private Renderer enemyRenderer;
    private Camera viewCamera;
    private bool defeated;
    private bool hasAppeared;
    private bool isOnScreen;
    private float offscreenSince = -1f;
    private float nextAttackAt;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;
        body.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        GetComponent<Collider>().isTrigger = true;

        enemyRenderer = GetComponent<Renderer>();
        viewCamera = Camera.main;

        if (target == null || enemyRenderer == null || viewCamera == null)
        {
            Debug.LogError("WingItEnemy requires a player target, renderer, and main camera.", this);
            enabled = false;
            return;
        }

        var color = new MaterialPropertyBlock();
        color.SetColor("_BaseColor", new Color(1f, 0.25f, 0.2f));
        color.SetColor("_Color", new Color(1f, 0.25f, 0.2f));
        enemyRenderer.SetPropertyBlock(color);
    }

    private void Update()
    {
        isOnScreen = GeometryUtility.TestPlanesAABB(
            GeometryUtility.CalculateFrustumPlanes(viewCamera), enemyRenderer.bounds);

        if (isOnScreen)
        {
            hasAppeared = true;
            offscreenSince = -1f;
            float distanceAhead = transform.position.x - target.position.x;
            if (distanceAhead > 0f && distanceAhead <= attackRange && Time.time >= nextAttackAt)
            {
                FireAtPlayer();
                nextAttackAt = Time.time + attackInterval;
            }
        }
        else if (hasAppeared)
        {
            if (offscreenSince < 0f)
                offscreenSince = Time.time;
            else if (Time.time - offscreenSince >= offscreenDespawnSeconds)
                Destroy(gameObject);
        }
    }

    private void FireAtPlayer()
    {
        Vector3 direction = target.position - transform.position;
        direction.z = 0f;
        direction.Normalize();

        GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectile.name = "Enemy projectile";
        projectile.transform.position = transform.position + direction * 0.7f;
        projectile.transform.localScale = Vector3.one * 0.24f;

        var color = new MaterialPropertyBlock();
        color.SetColor("_BaseColor", new Color(1f, 0.45f, 0.1f));
        color.SetColor("_Color", new Color(1f, 0.45f, 0.1f));
        projectile.GetComponent<Renderer>().SetPropertyBlock(color);

        Collider projectileCollider = projectile.GetComponent<Collider>();
        projectileCollider.isTrigger = true;
        Physics.IgnoreCollision(projectileCollider, GetComponent<Collider>());

        Rigidbody projectileBody = projectile.AddComponent<Rigidbody>();
        projectileBody.useGravity = false;
        projectileBody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        projectileBody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        projectileBody.linearVelocity = direction * projectileSpeed;
        projectile.AddComponent<WingItEnemyProjectile>().SetLifetime(4f);
    }

    private void FixedUpdate()
    {
        if (!isOnScreen)
            return;

        float distanceAhead = body.position.x - target.position.x;
        if (distanceAhead < 0f || distanceAhead > activationDistance)
            return;

        Vector3 next = body.position;
        if (distanceAhead > 1.5f)
            next.x = Mathf.MoveTowards(next.x, target.position.x + 1.5f, approachSpeed * Time.fixedDeltaTime);

        next.y = Mathf.MoveTowards(next.y, target.position.y, verticalFollowSpeed * Time.fixedDeltaTime);
        body.MovePosition(next);
    }

    public void TakeHit()
    {
        if (defeated)
            return;

        defeated = true;
        Destroy(gameObject);
    }
}
