
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
    private Camera _mainCamera;

    private bool _isBlock;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
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

        // 따라오기 중지
        if (_petFollow != null)
            _petFollow.enabled = false;

        // 스프라이트 전환
        _defaultVisual.SetActive(false);
        _blockVisual.SetActive(true);

        // 기존 속도 제거
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;

        // 클릭한 위치로 이동
        _rb.position = targetPosition;

        // 물리 활성화
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _rb.gravityScale = _blockGravity;

        // 블록 충돌 활성화
        _collider.enabled = true;
    }

    private void SetDefaultMode()
    {
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
