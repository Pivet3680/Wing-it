using UnityEngine;

public class PetFollow : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [Header("Follow settings")]
    [SerializeField] private float _moveSpeed = 4f;
    [SerializeField] private Vector2 _follwOffset = new Vector2(-2f, 1f);
    [SerializeField] private float _smoothTime = 0.15f;

    [Header("Floating Settings")]
    [SerializeField] private float _floatHeight = 0.15f;
    [SerializeField] private float _floatSpeed = 2f;
    [SerializeField] private SpriteRenderer _petSpriteRenderer;
    private Vector2 _velocity;
    private int _facingDirection = 1;
    private PlayerMovement2D _playerMovement;

    private void Awake()
    {
        _playerMovement = _playerTransform.GetComponent<PlayerMovement2D>();
    }
    private void LateUpdate()
    {
        if (_playerTransform == null) return;        

        _facingDirection = _playerMovement.FacingDirection;

        Vector2 offset = new Vector2(
            -_facingDirection * _follwOffset.x,
            _follwOffset.y
            );

        Vector2 targetPosition = (Vector2)_playerTransform.position + offset;

        targetPosition.y += Mathf.Sin(Time.time * _floatSpeed) * _floatHeight;

        Vector2 nextPosition = Vector2.SmoothDamp(
            transform.position,
            targetPosition,
            ref _velocity,
            _smoothTime,
            _moveSpeed,
            Time.deltaTime
            );

        _petSpriteRenderer.flipX = _facingDirection == -1;

        transform.position = new Vector3(
            nextPosition.x,
            nextPosition.y,
            transform.position.z
            );
    }

}
