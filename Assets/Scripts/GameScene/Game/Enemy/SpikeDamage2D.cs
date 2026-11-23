using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SpikeDamage2D : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement2D player =
            other.GetComponentInParent<PlayerMovement2D>();

        if (player == null) return;

        player.TakeHit(transform.position);
    }
}
