using UnityEngine;

[RequireComponent(typeof(Renderer))]
public sealed class WingItPlayerHitFeedback : MonoBehaviour
{
    [SerializeField, Min(0f)] private float flashSeconds = 0.4f;
    [SerializeField] private Color hitColor = new Color(1f, 0.15f, 0.15f);

    private Renderer playerRenderer;
    private MaterialPropertyBlock originalColors;
    private MaterialPropertyBlock hitColors;
    private float flashUntil;
    private bool flashing;

    public int HitCount { get; private set; }

    private void Awake()
    {
        playerRenderer = GetComponent<Renderer>();
        originalColors = new MaterialPropertyBlock();
        playerRenderer.GetPropertyBlock(originalColors);

        hitColors = new MaterialPropertyBlock();
        hitColors.SetColor("_BaseColor", hitColor);
        hitColors.SetColor("_Color", hitColor);
    }

    public void TakeHit()
    {
        HitCount++;
        flashUntil = Time.time + flashSeconds;
        flashing = true;
        playerRenderer.SetPropertyBlock(hitColors);
    }

    private void Update()
    {
        if (flashing && Time.time >= flashUntil)
            RestoreColor();
    }

    private void OnDisable()
    {
        if (flashing)
            RestoreColor();
    }

    private void RestoreColor()
    {
        playerRenderer.SetPropertyBlock(originalColors);
        flashing = false;
    }
}
