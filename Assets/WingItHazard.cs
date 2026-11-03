using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Renderer))]
public sealed class WingItHazard : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;

        var color = new MaterialPropertyBlock();
        color.SetColor("_BaseColor", new Color(0.65f, 0.15f, 0.9f));
        color.SetColor("_Color", new Color(0.65f, 0.15f, 0.9f));
        GetComponent<Renderer>().SetPropertyBlock(color);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out WingItPlayerHitFeedback feedback))
            feedback.TakeHit();
    }
}
