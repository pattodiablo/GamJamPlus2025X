using UnityEngine;

public class DestroyAmigo : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;         // Si es null, usa Camera.main
    [SerializeField] private float topMarginViewport = 0.05f; // Margen extra por arriba
    private bool scheduled;

    void Update()
    {
        var cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null || scheduled) return;

        Vector3 vp = cam.WorldToViewportPoint(transform.position);
        if (vp.y > 1f + topMarginViewport) // fuera de cámara por la parte superior
        {
            scheduled = true;
            Destroy(gameObject);
        }
    }

    private void OnValidate()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        if (topMarginViewport < 0f) topMarginViewport = 0f;
    }
}
