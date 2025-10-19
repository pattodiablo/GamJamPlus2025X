using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject platformPrefab;

    [Header("Spawn Settings")]
    public float minSpawnInterval = 0.5f;
    public float maxSpawnInterval = 2f;
    public float spawnY = -5f;
    public float moveSpeed = 2f;
    
    [Header("Platform Scale (Dimensiones)")]
    [Tooltip("Escala mínima de las plataformas")]
    public Vector3 minScale = new Vector3(0.5f, 0.5f, 1f);
    [Tooltip("Escala máxima de las plataformas")]
    public Vector3 maxScale = new Vector3(2f, 1.5f, 1f);
    
    [Header("Debug")]
    public bool showDebugInfo = true;

    private float sceneLimitX;
    private int platformsCreated = 0;

    void Start()
    {
        // Verificar que el prefab esté asignado
        if (platformPrefab == null)
        {
            Debug.LogError("❌ PlatformSpawner: No hay prefab asignado!");
            return;
        }
        
        CalculateSceneLimits();
        StartCoroutine(SpawnRoutine());
        
        if (showDebugInfo)
        {
            Debug.Log($"✅ PlatformSpawner iniciado - Límites X: ±{sceneLimitX:F2}, SpawnY: {spawnY}");
        }
    }

    void CalculateSceneLimits()
    {
        Camera cam = Camera.main;
        if (cam.orthographic)
        {
            float sceneLimitY = cam.orthographicSize;
            sceneLimitX = sceneLimitY * cam.aspect;
        }
        else
        {
            float distance = Mathf.Abs(cam.transform.position.z - transform.position.z);
            Vector3 corner = cam.ViewportToWorldPoint(new Vector3(1, 1, distance));
            sceneLimitX = corner.x;
        }
    }

    System.Collections.IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Verificar que el prefab siga siendo válido
            if (platformPrefab == null)
            {
                Debug.LogError("❌ PlatformSpawner: El prefab se perdió durante la ejecución!");
                yield break;
            }
            
            float randomX = Random.Range(-sceneLimitX, sceneLimitX);
            Vector3 spawnPos = new Vector3(randomX, spawnY, 0f);

            // Crear instancia del prefab (NO modificar el prefab original)
            GameObject platform = Instantiate(platformPrefab, spawnPos, Quaternion.identity);
            
            if (platform != null)
            {
                // Verificar que es una instancia, no el prefab original
                if (platform == platformPrefab)
                {
                    Debug.LogError("❌ Error crítico: Se está modificando el prefab original!");
                    yield break;
                }
                
                // Verificar si ya tiene PlatformMover (por si está en el prefab)
                PlatformMover existingMover = platform.GetComponent<PlatformMover>();
                if (existingMover == null)
                {
                    // Agregar el componente solo si no existe
                    PlatformMover mover = platform.AddComponent<PlatformMover>();
                    mover.speed = moveSpeed;
                }
                else
                {
                    // Si ya existe, configurar la velocidad usando el método SetSpeed
                    existingMover.SetSpeed(moveSpeed);
                }
                
                // Verificar que tiene Rigidbody
                VerifyRigidbody(platform);
                
                // Aplicar escala aleatoria a la plataforma
                ApplyRandomScale(platform);
                
                platformsCreated++;
                
                if (showDebugInfo)
                {
                    Debug.Log($"🟢 Plataforma #{platformsCreated} creada en ({randomX:F2}, {spawnY}) - Velocidad: {moveSpeed} - Escala: {platform.transform.localScale}");
                    Debug.Log($"🔍 Instancia creada: {platform.name} (ID: {platform.GetInstanceID()})");
                }
            }
            else
            {
                Debug.LogError("❌ Error: Instantiate devolvió null!");
            }

            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);
        }
    }
    
    void Update()
    {
        // Mostrar estadísticas cada 5 segundos
        if (showDebugInfo && Time.time % 5f < Time.deltaTime)
        {
            int activePlatforms = FindObjectsOfType<PlatformMover>().Length;
            Debug.Log($"📊 Stats - Creadas: {platformsCreated}, Activas: {activePlatforms}");
        }
    }
    
    void ApplyRandomScale(GameObject platform)
    {
        // Generar escala aleatoria para cada eje
        float randomScaleX = Random.Range(minScale.x, maxScale.x);
        float randomScaleY = Random.Range(minScale.y, maxScale.y);
        float randomScaleZ = Random.Range(minScale.z, maxScale.z);
        
        // Aplicar la escala aleatoria
        Vector3 newScale = new Vector3(randomScaleX, randomScaleY, randomScaleZ);
        platform.transform.localScale = newScale;
        
        if (showDebugInfo)
        {
            Debug.Log($"🎲 Escala aplicada: X={randomScaleX:F2}, Y={randomScaleY:F2}, Z={randomScaleZ:F2}");
        }
    }
    
    void VerifyRigidbody(GameObject platform)
    {
        Rigidbody rb = platform.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning($"⚠️ Plataforma {platform.name} no tiene Rigidbody! Agregándolo automáticamente...");
            rb = platform.AddComponent<Rigidbody>();
            
            // Configuración básica
            rb.useGravity = false;
            rb.linearDamping = 0f;
            rb.angularDamping = 0f;
            rb.freezeRotation = true;
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"🔧 Rigidbody verificado - UseGravity: {rb.useGravity}, Mass: {rb.mass}");
        }
    }
}
