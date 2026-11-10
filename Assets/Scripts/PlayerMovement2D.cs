using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement2D : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpPower = 10f;

    // 바닥 검사 설정
    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Vector2 _checkSize = new Vector2(0.5f, 0.15f);
    [SerializeField] private LayerMask _groundLayer;
    private Animator _animator;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private bool _isGrounded;

    public int FacingDirection { get; private set; } = 1;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // 바닥 검사 
        _isGrounded = Physics2D.OverlapBox(
                      _groundCheck.position,
                      _checkSize,
                      0f,
                      _groundLayer
            );

        if (Keyboard.current == null)
            return;

        float horizontal = 0f;

        if (Keyboard.current.aKey.isPressed)
            horizontal = -1f;

        if (Keyboard.current.dKey.isPressed)
            horizontal = 1f;

        bool isRunning = horizontal != 0f;

        if (_animator != null)
            _animator.SetBool("isRunning", isRunning);

        if(horizontal != 0f)
        {
            FacingDirection = horizontal > 0f ? 1 : -1;

            if(_spriteRenderer != null)
            {
                _spriteRenderer.flipX = FacingDirection == -1;
            }
        }

        _rb.linearVelocityX = horizontal * _moveSpeed;

        if (Keyboard.current.wKey.wasPressedThisFrame && _isGrounded)
        {
            _rb.linearVelocityY = _jumpPower;
        }

        // Scene 뷰에서 검사 범위 표시 
    }
    private void OnDrawGizmosSelected()
    {
        if (_groundCheck == null) return;

        Gizmos.color = _isGrounded ? Color.green : Color.red;

        Gizmos.DrawWireCube(
            _groundCheck.position,
            _checkSize
            );
    }
}
