using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerRespawn : MonoBehaviour
{
    [Header("Death Setting")]
    [SerializeField] private float _deathY = -10f;

    [Header("Respawn Setting")]
    [SerializeField] private Transform _spawnPoint;

    private Rigidbody2D _rb;
    private Vector3 _initlalPosition;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        // 게임 시작 위치 저장
        _initlalPosition = transform.position;
    }

    private void Update()
    {
        // 일정 Y좌표 아래로 떨어질 시 사망
        if(transform.position.y < _deathY)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        // 물리 속도 초기화
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;

        // 리스폰 위치 결정
        Vector3 targetPosition = _spawnPoint != null
            ? _spawnPoint.position : _initlalPosition;

        // 시작 위치로 이동
        _rb.position = new Vector2(
            targetPosition.x,
            targetPosition.y);
    }
}
