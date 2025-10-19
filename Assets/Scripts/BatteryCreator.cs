using UnityEngine;

public class BatteryCreator : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject batteryPrefab;

    [Header("Cámara y ubicación")]
    [SerializeField] private Camera targetCamera;     // Si es null usa Camera.main
    [SerializeField] private float bottomOffset = 1f; // Distancia por debajo del borde inferior
    [SerializeField] private float xMargin = 0.2f;    // Margen lateral

    [Header("Velocidad inicial (opcional)")]
    [SerializeField] private float initialUpSpeed = 0f; // Si > 0, aplica velocidad hacia arriba

    [Header("Progresión y tasa (spawns/seg)")]
    [SerializeField] private float startDelay = 0f;
    [SerializeField] private float minRate = 0.2f;   // Al inicio
    [SerializeField] private float maxRate = 2.0f;   // Tope; se capea a 3/seg
    [SerializeField] private float rampUpTime = 60f; // Segs para ir de minRate a maxRate
    [SerializeField] private AnimationCurve rateCurve = AnimationCurve.Linear(0, 0, 1, 1);

    private float startTime;
    private float spawnBudget;

    private void Start()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        startTime = Time.time;
        maxRate = Mathf.Clamp(maxRate, 0.01f, 3f);
    }

    private void Update()
    {
        if (batteryPrefab == null || targetCamera == null || !targetCamera.orthographic) return;

        float elapsed = Time.time - startTime - startDelay;
        if (elapsed < 0f) return;

        float t = rampUpTime > 0f ? Mathf.Clamp01(elapsed / rampUpTime) : 1f;
        float curve = Mathf.Clamp01(rateCurve.Evaluate(t));
        float currentRate = Mathf.Clamp(Mathf.Lerp(minRate, maxRate, curve), 0f, 3f);

        spawnBudget += currentRate * Time.deltaTime;

        while (spawnBudget >= 1f)
        {
            SpawnOne();
            spawnBudget -= 1f;
        }
    }

    private void SpawnOne()
    {
        var cam = targetCamera;
        Vector3 camPos = cam.transform.position;
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        float minX = camPos.x - halfW + xMargin;
        float maxX = camPos.x + halfW - xMargin;

        float x = Random.Range(minX, maxX);
        float y = camPos.y - halfH - bottomOffset; // fuera de cámara por abajo

        var obj = Instantiate(batteryPrefab, new Vector3(x, y, 0f), Quaternion.identity);

        if (initialUpSpeed > 0f && obj.TryGetComponent<Rigidbody2D>(out var rb))
        {
            var v = rb.linearVelocity;
            v.y = Mathf.Abs(initialUpSpeed);
            rb.linearVelocity = v;
        }
    }

    private void OnValidate()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        minRate = Mathf.Max(0f, minRate);
        maxRate = Mathf.Clamp(maxRate, 0.01f, 3f);
        xMargin = Mathf.Max(0f, xMargin);
        bottomOffset = Mathf.Max(0f, bottomOffset);
    }
}
