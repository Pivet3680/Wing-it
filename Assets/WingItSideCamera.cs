using UnityEngine;

[RequireComponent(typeof(Camera))]
public sealed class WingItSideCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField, Range(0f, 1f)] private float targetViewportX = 0.275f;
    [SerializeField] private float cameraHeight = 1.5f;
    [SerializeField] private float cameraDepth = -15f;

    private Camera cameraComponent;

    public void SetTarget(Transform newTarget) => target = newTarget;

    private void Awake() => cameraComponent = GetComponent<Camera>();

    private void LateUpdate()
    {
        if (target == null)
            return;

        float visibleWidth = 2f * cameraComponent.orthographicSize * cameraComponent.aspect;
        float cameraX = target.position.x + (0.5f - targetViewportX) * visibleWidth;
        transform.position = new Vector3(cameraX, cameraHeight, cameraDepth);
    }
}
