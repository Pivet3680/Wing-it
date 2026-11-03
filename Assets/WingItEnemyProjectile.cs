using UnityEngine;

public sealed class WingItEnemyProjectile : MonoBehaviour
{
    public void SetLifetime(float seconds) => Destroy(gameObject, seconds);

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<WingItEnemy>() == null)
            Destroy(gameObject);
    }
}
