using UnityEngine;

public class SpikeMovement2D : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveDistance = 2f;
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private bool _startMovingUp = true;

    private Rigidbody2D _rb;
    private Vector2 _startPosition;
    private Vector2 _targetPosition;
    private bool _movingUp;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // 시작 위치 저장
        _startPosition = _rb.position;

        // 처음 이동 방향 설정
        _movingUp = _startMovingUp;

        UpdateTarget();
    }

    private void FixedUpdate()
    {
        // 현재 위치에서 목표 위치까지 이동
        Vector2 newPosition = Vector2.MoveTowards(
            _rb.position,
            _targetPosition,
            _moveSpeed * Time.fixedDeltaTime);

        _rb.MovePosition(newPosition);

        // 목표 위치 도착
        if(Vector2.Distance(
            newPosition,
            _targetPosition) < 0.001f)
        {
            _movingUp = !_movingUp;
            UpdateTarget();
        }
    }

    private void UpdateTarget()
    {
        if (_movingUp)
        {
            _targetPosition = 
                _startPosition + Vector2.up * _moveDistance;
        }
        else
        {
            _targetPosition =
                _startPosition + Vector2.down * _moveDistance;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = Application.isPlaying
            ? (Vector3)_startPosition
              : transform.position;
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(
            center + Vector3.up * _moveDistance,
            center + Vector3.down * _moveDistance);

        Gizmos.DrawWireSphere(
            center + Vector3.up * _moveDistance,
            0.15f);
        Gizmos.DrawWireSphere(
            center + Vector3.down * _moveDistance,
            0.15f);
    }
}
