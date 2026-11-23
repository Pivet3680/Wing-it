using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStomp : MonoBehaviour
{
    [Header("Enemy References")]
    [SerializeField] private GameObject _enemyRoot;
    [SerializeField] private Animator _enemyAnimator;
    [SerializeField] private Collider2D _bodyCollider;

    [Header("Stomp Settings")]
    [SerializeField] private float _bouncePower = 7f;
    [SerializeField] private float _minimumFallSpeed = 0.1f;
    [SerializeField] private float _hitAnimationDuration = 0.5f;

    private bool _isDead;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isDead) return;

        PlayerMovement2D player =
            other.GetComponent<PlayerMovement2D>();

        if (player == null) return;

        Rigidbody2D playerRb =
            player.GetComponent<Rigidbody2D>();

        if (playerRb == null) return;

        // 내려오는 중일 때만 밟기 가능
        if (playerRb.linearVelocityY >= -_minimumFallSpeed) 
            return;

        // 플레이어의 발이 몬스터 몸체 위쪽에 있는 경우에만 밟기 성공
        if(_bodyCollider != null)
        {
            float playerFeet = other.bounds.min.y;
            float enemyTop = _bodyCollider.bounds.max.y;

            if (playerFeet < enemyTop - 0.15f) return;
        }

        // 밟기 성공
        _isDead = true;

        // 플레이어를 위로 튕겨 올림
        playerRb.linearVelocityY = _bouncePower;

        StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        // 몬스터의 모든 충돌 판정 비활성화
        Collider2D[] colliders =
            _enemyRoot.GetComponentsInChildren<Collider2D>();

        foreach(Collider2D col in colliders)
        {
            col.enabled = false;
        }

        // 사망 후 물리 이동 중단
        Rigidbody2D enemyRb =
            _enemyRoot.GetComponent<Rigidbody2D>();

        if(enemyRb != null)
        {
            enemyRb.linearVelocity = Vector2.zero;
            enemyRb.simulated = false;
        }

        // 피격 애니메이션 재생
        if(_enemyAnimator != null)
        {
            _enemyAnimator.SetTrigger("Hit");
        }

        // 애니메이션이 끝날 때까지 대기
        yield return new WaitForSeconds(
            _hitAnimationDuration);

        // 몬스터 제거
        Destroy(_enemyRoot);
    }


}
