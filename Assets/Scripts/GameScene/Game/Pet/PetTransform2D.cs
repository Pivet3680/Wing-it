
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PetTransform2D : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private GameObject _defaultVisual;
    [SerializeField] private GameObject _blockVisual;

    [Header("Follow")]
    [SerializeField] private MonoBehaviour _petFollow;

    [Header("Block Physics")]
    [SerializeField] private float _blockGravity = 3f;

    private Rigidbody2D _rb;
    private BoxCollider2D _collider;
    private BoxCollider2D _blockVisualCollider;
    private PetLauncher2D _launcher;
    private Camera _mainCamera;
    private bool _isBlock;
    public bool IsBlock => _isBlock;
    public BoxCollider2D BlockCollider => _blockVisualCollider != null &&
        _blockVisualCollider.isActiveAndEnabled ? _blockVisualCollider : _collider;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _blockVisualCollider = _blockVisual.GetComponent<BoxCollider2D>();
        _launcher = GetComponent<PetLauncher2D>();
        _mainCamera = Camera.main;

        SetDefaultMode();
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
            return;

        if (Mouse.current == null)
            return;

        if (!Mouse.current.middleButton.wasPressedThisFrame)
            return;

        if (_isBlock)
        {
            SetDefaultMode();
        }
        else
        {
            Vector2 mousePosition =
                Mouse.current.position.ReadValue();

            Vector3 worldPosition =
                _mainCamera.ScreenToWorldPoint(
                    new Vector3(
                        mousePosition.x,
                        mousePosition.y,
                        -_mainCamera.transform.position.z
                    )
                );

            SetBlockMode(worldPosition);
        }
    }


    private void SetBlockMode(Vector2 targetPosition)
    {
        _isBlock = true;

        // 기존 따라오기 중지
        if (_petFollow != null)
            _petFollow.enabled = false;

        // 기존 물리 상태 초기화
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.constraints = RigidbodyConstraints2D.None;
        _rb.gravityScale = 0f;
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;

        // 마우스로 지정한 위치에 이동
        _rb.position = targetPosition;
        _rb.SetRotation(0f);

        // 스프라이트 전환
        _defaultVisual.SetActive(false);
        _blockVisual.SetActive(true);

        // 블록 물리 설정
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _rb.gravityScale = _blockGravity;

        // X축 고정, 회전 방지
        _rb.constraints = RigidbodyConstraints2D.FreezePositionX |
            RigidbodyConstraints2D.FreezeRotation;

        _collider.enabled = true;
        
    }


    private void SetDefaultMode()
    {
        _launcher?.StopLaunch();
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _isBlock = false;

        // 블록 물리 해제
        _collider.enabled = false;

        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.gravityScale = 0f;
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;

        // 기본형 스프라이트 복귀
        _blockVisual.SetActive(false);
        _defaultVisual.SetActive(true);

        // 따라오기 재개
        if (_petFollow != null)
            _petFollow.enabled = true;
    }
}
