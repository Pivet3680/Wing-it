using UnityEngine;

public sealed class WingItProjectile : MonoBehaviour
{
    public void SetLifetime(float seconds) => Destroy(gameObject, seconds);

    private void OnCollisionEnter(Collision collision) => Destroy(gameObject);

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out WingItEnemy enemy))
        {
            enemy.TakeHit();
            Destroy(gameObject);
        }
    }
}
