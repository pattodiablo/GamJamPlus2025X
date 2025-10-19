using UnityEngine;
using System.Collections;


public class AmigoController : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject amigoPrefab;

    [Header("Cámara y ubicación")]
    [SerializeField] private Camera targetCamera;   // Si es null usa Camera.main
    [SerializeField] private float bottomOffset = 1.0f; // Qué tan abajo del borde inferior aparece
    [SerializeField] private float xMargin = 0.2f;      // Margen lateral para evitar cortar

    [Header("Progresión y tasa (spawns/seg)")]
    [SerializeField] private float startDelay = 0f;     // Retraso inicial antes de empezar
    [SerializeField] private float minRate = 0.2f;      // Al inicio (spawns/seg)
    [SerializeField] private float maxRate = 3f;        // Límite duro (máx. 3/seg)
    [SerializeField] private float rampUpTime = 60f;    // Segundos para llegar de minRate a maxRate
    [SerializeField] private AnimationCurve rateCurve = AnimationCurve.Linear(0, 0, 1, 1);
    // La curva define cómo progresa entre 0..1 de tiempo normalizado

    [Header("Audio Amigo")]
    [SerializeField] private AudioSource spawnSoundAmigo;
    //[SerializeField] private float delayPlay;

    private float startTime;
    private float spawnBudget; // Acumula “spawns fraccionarios” (método estable por frame)

    void Start()
    {
        startTime = Time.time;
        if (targetCamera == null) targetCamera = Camera.main;
        maxRate = Mathf.Clamp(maxRate, 0.01f, 3f); // nunca superar 3/seg

        //StartCoroutine(ExecuteAfterDelay());

    }

    void Update()
    {
        if (amigoPrefab == null || targetCamera == null || !targetCamera.orthographic) return;

        float elapsed = Time.time - startTime - startDelay;
        if (elapsed < 0f) return;

        // 0..1 de progreso según el tiempo transcurrido
        float t = rampUpTime > 0f ? Mathf.Clamp01(elapsed / rampUpTime) : 1f;
        float curveFactor = Mathf.Clamp01(rateCurve.Evaluate(t));

        // Tasa actual (spawns/seg), limitada a máx. 3
        float currentRate = Mathf.Clamp(Mathf.Lerp(minRate, maxRate, curveFactor), 0f, 3f);

        // Acumular presupuesto de spawns según deltaTime
        spawnBudget += currentRate * Time.deltaTime;

        // Instanciar mientras haya “presupuesto” entero
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

        float x = Random.Range(minX, maxX);
        float y = camPos.y - halfHeight - bottomOffset;
        Vector3 spawnPos = new Vector3(x, y, 0f);

        Instantiate(amigoPrefab, spawnPos, Quaternion.identity);

        spawnSoundAmigo.Play();


    }

    /*IEnumerator ExecuteAfterDelay()
    {
        yield return new WaitForSeconds(delayPlay);
        Debug.Log("suena");
        spawnSoundAmigo.Play();
    }*/

    private void OnValidate()
    {
        if (minRate < 0f) minRate = 0f;
        if (maxRate < 0.01f) maxRate = 0.01f;
        if (xMargin < 0f) xMargin = 0f;
        if (bottomOffset < 0f) bottomOffset = 0f;
    }
}
