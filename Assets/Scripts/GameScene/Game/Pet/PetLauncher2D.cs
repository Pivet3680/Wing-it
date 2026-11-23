using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PetTransform2D))]
public class PetLauncher2D : MonoBehaviour
{
    [Header("Launch Settings")]
    [SerializeField] private float _launchSpeed = 15f;
    [SerializeField] private float _launchDistance = 5f;

    private Rigidbody2D _rb;
    private PetTransform2D _petTransform;

    private bool _isLaunching;
    private float _launchStartX;
    private int _launchDirection;
    private float _launchEndTime;

    private float _originalGravity;
    private RigidbodyConstraints2D _originalConstraints;
    private CollisionDetectionMode2D _originalCollisionDetection;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _petTransform = GetComponent<PetTransform2D>();
    }

    public bool TryLaunchFrom(Collider2D playerCollider, int direction, float range)
    {
        if (!isActiveAndEnabled || !_petTransform.IsBlock || _isLaunching ||
            _launchSpeed <= 0f || _launchDistance <= 0f)
            return false;

        Collider2D blockCollider = _petTransform.BlockCollider;
        if ((blockCollider.bounds.center.x - playerCollider.bounds.center.x) * direction <= 0f)
            return false;

        ColliderDistance2D distance = playerCollider.Distance(blockCollider);
        if (!distance.isValid || distance.distance > range)
            return false;

        _isLaunching = true;
        _launchDirection = direction >= 0 ? 1 : -1;
        _launchStartX = _rb.position.x;
        _launchEndTime = Time.time + _launchDistance / _launchSpeed + 0.25f;

        // 현재 물리 설정 저장
        _originalGravity = _rb.gravityScale;
        _originalConstraints = _rb.constraints;
        _originalCollisionDetection = _rb.collisionDetectionMode;

        // X축, Y축 이동 허용 / Z 축 회전만 고정
        _rb.constraints =
            RigidbodyConstraints2D.FreezeRotation;

        // 발사 중 중력 해제
        _rb.gravityScale = 0f;
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        _rb.linearVelocity = Vector2.zero;

        return true;
    }

    private void FixedUpdate()
    {
        if (!_isLaunching) return;

        float traveled = (_rb.position.x - _launchStartX) * _launchDirection;
        float remainingDistance = _launchDistance - traveled;

        if (remainingDistance <= 0.001f || Time.time >= _launchEndTime)
        {
            StopLaunch();
            return;
        }

        float speed = Mathf.Min(_launchSpeed, remainingDistance / Time.fixedDeltaTime);
        _rb.linearVelocity = new Vector2(_launchDirection * speed, 0f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StopAtBlockingContact(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        StopAtBlockingContact(collision);
    }

    private void StopAtBlockingContact(Collision2D collision)
    {
        if (!_isLaunching) return;

        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.x * _launchDirection < -0.75f)
            {
                StopLaunch();
                return;
            }
        }
    }

    private void OnDisable()
    {
        StopLaunch();
    }

    public void StopLaunch()
    {
        if (!_isLaunching) return;

        _isLaunching = false;

        _rb.linearVelocity = Vector2.zero;
        _rb.gravityScale = _originalGravity;
        _rb.constraints = _originalConstraints;
        _rb.collisionDetectionMode = _originalCollisionDetection;
    }
}
