using System.Collections;
using UnityEngine;

public class MetthorCreator : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private GameObject meteorPrefab;

    [Header("Placement")]
    [SerializeField] private float topOffset = 1.0f;     // Distancia por encima del borde superior
    [SerializeField] private float xMargin = 0.3f;       // Margen lateral
    [SerializeField] private Camera targetCamera;        // Si es null, usa Camera.main

    [Header("Velocity (opcional)")]
    [SerializeField] private float initialDownSpeed = 0f; // Si > 0, aplica velocidad hacia abajo

    [Header("Progresión y tasa (spawns/seg)")]
    [SerializeField] private float startDelay = 0f;     // Retraso inicial
    [SerializeField] private float minRate = 0.2f;      // Al inicio (spawns/seg)
    [SerializeField] private float maxRate = 3f;        // Máximo (cap 3/seg)
    [SerializeField] private float rampUpTime = 60f;    // Segundos para ir de minRate a maxRate
    [SerializeField] private AnimationCurve rateCurve = AnimationCurve.Linear(0, 0, 1, 1);

    private float startTime;
    private float spawnBudget;

    private void Start()
    {
        startTime = Time.time;
        if (targetCamera == null) targetCamera = Camera.main;
        maxRate = Mathf.Clamp(maxRate, 0.01f, 3f);
    }

    private void Update()
    {
        if (meteorPrefab == null || targetCamera == null || !targetCamera.orthographic) return;

        float elapsed = Time.time - startTime - startDelay;
        if (elapsed < 0f) return;

        float t = rampUpTime > 0f ? Mathf.Clamp01(elapsed / rampUpTime) : 1f;
        float curveFactor = Mathf.Clamp01(rateCurve.Evaluate(t));

        float currentRate = Mathf.Clamp(Mathf.Lerp(minRate, maxRate, curveFactor), 0f, 3f);

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
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        float minX = camPos.x - halfWidth + xMargin;
        float maxX = camPos.x + halfWidth - xMargin;

        float spawnX = Random.Range(minX, maxX);
        float spawnY = camPos.y + halfHeight + topOffset;

        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);
        GameObject meteor = Instantiate(meteorPrefab, spawnPos, Quaternion.identity);

        // Hasta 10% más pequeñas por instancia
        var baseScale = meteorPrefab.transform.localScale;
        float scaleFactor = Random.Range(0.9f, 1f);
        meteor.transform.localScale = baseScale * scaleFactor;

        if (initialDownSpeed > 0f && meteor.TryGetComponent<Rigidbody2D>(out var rb))
        {
            var v = rb.linearVelocity;
            v.y = -Mathf.Abs(initialDownSpeed);
            rb.linearVelocity = v;
        }
    }

    private void OnValidate()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        minRate = Mathf.Max(0f, minRate);
        maxRate = Mathf.Clamp(maxRate, 0.01f, 3f);
        xMargin = Mathf.Max(0f, xMargin);
        topOffset = Mathf.Max(0f, topOffset);
    }
}
