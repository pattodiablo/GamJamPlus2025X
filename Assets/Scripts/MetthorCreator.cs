using System.Collections;
using UnityEngine;

public class MetthorCreator : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private GameObject meteorPrefab;
    [SerializeField] private float intervalMin = 1.0f;
    [SerializeField] private float intervalMax = 2.5f;

    [Header("Placement")]
    [SerializeField] private float topOffset = 1.0f;     // Aparece esta distancia por encima del borde superior
    [SerializeField] private float xMargin = 0.3f;       // Margen lateral para no salir cortado
    [SerializeField] private Camera targetCamera;        // Si es null, usa Camera.main

    [Header("Velocity (opcional)")]
    [SerializeField] private float initialDownSpeed = 0f; // Si > 0 y el meteor tiene Rigidbody2D, le da velocidad hacia abajo

    private Coroutine spawnRoutine;

    private void OnEnable()
    {
        if (spawnRoutine == null)
            spawnRoutine = StartCoroutine(SpawnLoop());
    }

    private void OnDisable()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnOne();

            float wait = (intervalMax > intervalMin)
                ? Random.Range(intervalMin, intervalMax)
                : intervalMin;

            yield return new WaitForSeconds(wait);
        }
    }

    private void SpawnOne()
    {
        if (meteorPrefab == null)
        {
            Debug.LogWarning($"{name}: Asigna un prefab de Meteor en MetthorCreator.");
            return;
        }

        var cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null || !cam.orthographic)
        {
            Debug.LogWarning($"{name}: Se requiere una cámara ortográfica (targetCamera o Camera.main).");
            return;
        }

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
            var v = rb.linearVelocity;            // corregido: velocity (no linearVelocity)
            v.y = -Mathf.Abs(initialDownSpeed);
            rb.linearVelocity = v;
        }
    }

    private void OnValidate()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
        intervalMin = Mathf.Max(0.01f, intervalMin);
        intervalMax = Mathf.Max(intervalMin, intervalMax);
        xMargin = Mathf.Max(0f, xMargin);
        topOffset = Mathf.Max(0f, topOffset);
    }
}
