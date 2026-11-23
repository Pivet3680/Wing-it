
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpPower = 10f;

    [Header("Hit Setting")]
    [SerializeField] private float _knockbackX = 6f;
    [SerializeField] private float _knockbackY = 3f;
    [SerializeField] private float _knockbackDuration = 0.2f;
    [SerializeField] private float _invincibleDuration = 0.5f;

    private bool _isknockedBack;
    private float _knockbackEndTime;
    private float _invincibleEndTime;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Vector2 _checkSize = new Vector2(0.5f, 0.15f);
    [SerializeField] private LayerMask _groundLayer;

    [Header("Crouch Setting")]
    [SerializeField] private float _crouchHeightMultiplier = 0.5f;
    private BoxCollider2D _playerCollider;
    private Vector2 _originalColliderSize;
    private Vector2 _originalColliderOffset;
    private bool _isCrouching;

    [Header("Dash Settings")]
    [SerializeField] private float _dashSpeed = 15f;
    [SerializeField] private float _dashDistance = 3f;
    [SerializeField] private float _dashCooldown = 1f;
    [SerializeField] private float _dashMaxDuration = 0.5f;

    private Animator _animator;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;

    private bool _isGrounded;
    private bool _isDashing;

    private float _originalGravity;
    private float _nextDashTime;
    private float _dashStartX;
    private float _dashEndTime;
    private int _dashDirection;

    [Header("Pet Launch")]
    [SerializeField] private PetLauncher2D _petLauncher;
    [SerializeField] private float _launchRange = 0.5f;

    public int FacingDirection { get; private set; } = 1;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _originalGravity = _rb.gravityScale;

        _playerCollider = GetComponent<BoxCollider2D>();

        _originalColliderSize = _playerCollider.size;
        _originalColliderOffset = _playerCollider.offset;
    }

    private void Update()
    {
        // 넉백 중에는 일반 이동 및 점프 입력 차단
        if (_isknockedBack)
        {
            if (Time.time < _knockbackEndTime) return;

            _isknockedBack = false;
        }

        // 대시 중 일반 이동과 점프 차단
        if (_isDashing)
            return;

        if (Time.timeScale == 0f) return;

        // 바닥 감지
        _isGrounded = Physics2D.OverlapBox(
            _groundCheck.position,
            _checkSize,
            0f,
            _groundLayer
        );

        if(_animator != null)
        {
            _animator.SetBool("isGrounded", _isGrounded);
        }

        if (Keyboard.current == null)
            return;       

        float horizontal = 0f;

        if (Keyboard.current.aKey.isPressed)
            horizontal = -1f;

        if (Keyboard.current.dKey.isPressed)
            horizontal = 1f;

        bool isRunning = horizontal != 0f;

        bool wantsCrouch = Keyboard.current.sKey.isPressed && _isGrounded;

        SetCrouch(wantsCrouch);

        if (_animator != null)
            _animator.SetBool("isRunning", isRunning);

        // 방향 전환
        if (horizontal != 0f)
        {
            FacingDirection = horizontal > 0f ? 1 : -1;

            if (_spriteRenderer != null)
                _spriteRenderer.flipX = FacingDirection == -1;
        }

        // 일반 이동
        _rb.linearVelocityX = horizontal * _moveSpeed;

        // 점프
        if (Keyboard.current.wKey.wasPressedThisFrame && _isGrounded
            && !_isCrouching)
        {
            _rb.linearVelocityY = _jumpPower;

            _animator.SetTrigger("isJumping");
        }
        
        // 가까운 블록을 차고, 발사할 수 없으면 기존 대시를 실행한다.
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (_petLauncher != null &&
                _petLauncher.TryLaunchFrom(_playerCollider, FacingDirection, _launchRange))
            {
                if (_animator != null)
                    _animator.SetTrigger("Kick");
            }
            else if (Time.time >= _nextDashTime)
            {

                StartDash();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!_isDashing)
            return;

        // 대시 시작 위치에서 얼마나 이동했는지 계산
        float movedDistance =
            Mathf.Abs(_rb.position.x - _dashStartX);

        float remainingDistance = _dashDistance - movedDistance;

        // 목표 거리 도달 또는 제한 시간 초과
        if (remainingDistance <= 0f ||
            Time.time >= _dashEndTime)
        {
            StopDash();
            return;
        }

        // 마지막 순간 목표 거리를 지나치지 않도록 속도 조절
        float speed = Mathf.Min(
            _dashSpeed,
            remainingDistance / Time.fixedDeltaTime
        );

        // 대시 중 수평 속도 유지, 수직 속도 0
        _rb.linearVelocity = new Vector2(
            _dashDirection * speed,
            0f
        );
    }

    public void TakeHit(Vector2 enemyPosition)
    {
        if (_isDashing)
        {
            _isDashing = false;
            _rb.gravityScale = _originalGravity;
        }
        // 무적 상태라면, 피격 무시
        if (Time.time < _invincibleEndTime) return;

        _invincibleEndTime = Time.time + _invincibleDuration;
        _knockbackEndTime = Time.time + _knockbackDuration;
        _isknockedBack = true;

        // 적의 반대 방향으로 넉백
        float direction =
            transform.position.x < enemyPosition.x ? -1f : 1f;
        
        _rb.linearVelocity = new Vector2(
            direction * _knockbackX,
            _knockbackY);

        // 패격 애니메이션 (Anlmator에 Hit Trigger가 있을때)
        _animator.SetTrigger("Hit");
    }
    private void StartDash()
    {
        _isDashing = true;

        _dashDirection = FacingDirection;
        _dashStartX = _rb.position.x;
        _dashEndTime = Time.time + _dashMaxDuration;
        _nextDashTime = Time.time + _dashCooldown;

        // 중력 제거
        _rb.gravityScale = 0f;

        // 시작 순간에도 대시 속도 적용
        _rb.linearVelocity = new Vector2(
            _dashDirection * _dashSpeed,
            0f
        );

        // 대시 애니메이션
        if (_animator != null)
        {
            _animator.SetBool("isRunning", false);
            _animator.SetTrigger("Dash");
        }
    }

    private void StopDash()
    {
        _isDashing = false;

        // 중력 복원
        _rb.gravityScale = _originalGravity;

        // 대시 속도 제거
        _rb.linearVelocity = Vector2.zero;
    }

    private void SetCrouch(bool crouch)
    {
        if (_isCrouching == crouch) return;

        _isCrouching = crouch;

        if (_isCrouching)
        {
            float newHeight =
                _originalColliderSize.y * _crouchHeightMultiplier;

            float heightDifference =
                _originalColliderSize.y - newHeight;

            _playerCollider.size = new Vector2(
                _originalColliderSize.x,
                newHeight);

            _playerCollider.offset = new Vector2(
                _originalColliderOffset.x,
                _originalColliderOffset.y - heightDifference / 2f);
        }
        else
        {
            _playerCollider.size = _originalColliderSize;
            _playerCollider.offset = _originalColliderOffset;
        }

        if(_animator != null)
        {
            _animator.SetBool("isCrouching", _isCrouching);
        }
    }

    private void OnDisable()
    {
        if (_rb != null)
            _rb.gravityScale = _originalGravity;

        _isDashing = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (_groundCheck == null)
            return;

        Gizmos.color = _isGrounded ? Color.green : Color.red;

        Gizmos.DrawWireCube(
            _groundCheck.position,
            _checkSize
        );
    }
}
