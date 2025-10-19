using UnityEngine;

public class VerticalBackgroundScroll : MonoBehaviour
{
    [Header("Player Connection")]
    [Tooltip("Referencia al PlayerController2D")]
    public PlayerController2D playerController;

    [Tooltip("Referencia al VirtualJoystick")]
    public VirtualJoystick virtualJoystick;

    [Header("Movimiento")]
    [Tooltip("Aceleración por segundo")]
    public float acceleration = 1000f;
    
    [Tooltip("Velocidad máxima que puede alcanzar")]
    public float maxSpeed = 50f;

    [Header("Player Velocity")]
    [Tooltip("Multiplicador para la velocidad actual del player")]
    public float currentVelocityMultiplier = 1f;
    
    [Tooltip("Multiplicador para la linear velocity del player (para uso futuro)")]
    public float linearVelocityMultiplier = 1f;

    [Header("Debug")]
    [Tooltip("Mostrar información de velocidad en consola")]
    public bool showDebugInfo = false;

    // Variables privadas
    private float currentSpeed;
    private Transform bg1;
    private Transform bg2;
    private float backgroundHeight;
    
    // Variables para almacenar velocidades del player
    private float playerCurrentVelocityY;
    private float playerLinearVelocityY;

    void Start()
    {
        InitializeBackground();
    }

    void InitializeBackground()
    {
        // Buscar PlayerController2D automáticamente si no está asignado
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController2D>();
            
            if (playerController == null)
            {
                Debug.LogError("❌ FondoController: No se encontró PlayerController2D en la escena");
                currentSpeed = 1f; // Velocidad por defecto si no hay player
            }
        }


// Buscar VirtualJoystick automáticamente si no está asignado
        if (virtualJoystick == null)
        {
            virtualJoystick = FindObjectOfType<VirtualJoystick>();
            
            if (virtualJoystick == null)
            {
                Debug.LogError("❌ FondoController: No se encontró VirtualJoystick en la escena");
            }
        }
        // Inicializar velocidad con la currentVelocity del player
        UpdatePlayerVelocities();
        currentSpeed = Mathf.Abs(playerLinearVelocityY * currentVelocityMultiplier);
        
        // Si la velocidad inicial es muy baja, usar un mínimo
        if (currentSpeed < 0.5f)
        {
            currentSpeed = 1f;
        }

        // Obtener los dos fondos hijos
        bg1 = transform.GetChild(0);
        bg2 = transform.GetChild(1);

        // Calcular altura real del sprite
        SpriteRenderer sr = bg1.GetComponent<SpriteRenderer>();
        backgroundHeight = sr.bounds.size.y;

        // Posicionar bg2 debajo de bg1 para scroll infinito
        bg2.position = new Vector3(bg1.position.x, bg1.position.y - backgroundHeight, bg1.position.z);

        if (showDebugInfo)
        {
            Debug.Log($"✅ VerticalBackgroundScroll inicializado:");
            Debug.Log($"   Player conectado: {playerController != null}");
            Debug.Log($"   Velocidad inicial (currentVelocity): {currentSpeed}");
            Debug.Log($"   Aceleración: {acceleration}");
            Debug.Log($"   Velocidad máxima: {maxSpeed}");
            Debug.Log($"   Altura del fondo: {backgroundHeight}");
        }
    }

    void Update()
    {
        UpdatePlayerVelocities();
        UpdateSpeed();
        MoveBackground();
        HandleInfiniteScroll();
    }

    void UpdatePlayerVelocities()
    {
        if (playerController == null) return;


        // Obtener linearVelocity.y del Rigidbody2D del player
        Rigidbody2D playerRb = playerController.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerLinearVelocityY = playerRb.linearVelocity.y;
        }

        if (showDebugInfo && Time.frameCount % 120 == 0) // Cada 2 segundos aprox
        {
            Debug.Log($"📊  LinearVelocity.y: {playerLinearVelocityY:F2}");
        }
    }

    void UpdateSpeed()
    {
        // Usar currentVelocity del player como base para la velocidad del fondo
        float playerInfluence = Mathf.Abs(playerLinearVelocityY * currentVelocityMultiplier);

        // Acelerar normalmente
        

        if (virtualJoystick.inputVector.y < -0.1f)
        {
            currentSpeed = 100f ;
        }
        else
        {
            currentSpeed = 10f; //+ (500f * acceleration * Time.deltaTime);
        }

        
        // Debug de velocidad actual sin spam
        if (showDebugInfo && Time.frameCount % 60 == 0) // Cada segundo aprox
        {
            Debug.Log($"🚀 Velocidad actual: {currentSpeed:F2} (Player influence: {playerInfluence:F2})");
        }
        
        // Aplicar límite de velocidad máxima
        if (currentSpeed > playerInfluence + maxSpeed)
        {
            currentSpeed = playerInfluence + maxSpeed;
            
            if (showDebugInfo)
            {
                Debug.Log($"🚀 Velocidad máxima alcanzada: {maxSpeed}");
            }
        }
    }

    void MoveBackground()
    {
        Vector3 movement = Vector3.up * currentSpeed * Time.deltaTime;
        bg1.Translate(movement);
        bg2.Translate(movement);
    }

    void HandleInfiniteScroll()
    {
        // Reposicionar bg1 si sale de pantalla
        if (bg1.position.y >= backgroundHeight)
        {
            bg1.position = new Vector3(bg1.position.x, bg2.position.y - backgroundHeight + 0.01f, bg1.position.z);
        }

        // Reposicionar bg2 si sale de pantalla
        if (bg2.position.y >= backgroundHeight)
        {
            bg2.position = new Vector3(bg2.position.x, bg1.position.y - backgroundHeight + 0.01f, bg2.position.z);
        }
    }

    // Métodos públicos para control externo
    public void SetSpeed(float newSpeed)
    {
        currentSpeed = Mathf.Clamp(newSpeed, 0f, maxSpeed);
        
        if (showDebugInfo)
        {
            Debug.Log($"⚡ Velocidad cambiada manualmente a: {currentSpeed}");
        }
    }

    public void ResetSpeed()
    {
        UpdatePlayerVelocities();
        currentSpeed = Mathf.Abs(playerCurrentVelocityY * currentVelocityMultiplier);
        
        if (currentSpeed < 0.5f)
        {
            currentSpeed = 1f;
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"🔄 Velocidad reiniciada a: {currentSpeed}");
        }
    }

    public void SetMaxSpeed(float newMaxSpeed)
    {
        maxSpeed = newMaxSpeed;
        
        // Si la velocidad actual excede el nuevo máximo, ajustarla
        if (currentSpeed > maxSpeed)
        {
            currentSpeed = maxSpeed;
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"🎯 Nueva velocidad máxima: {maxSpeed}");
        }
    }

    // Getters para información del estado
    public float GetCurrentSpeed() => currentSpeed;
    public float GetMaxSpeed() => maxSpeed;
    public bool IsAtMaxSpeed() => Mathf.Approximately(currentSpeed, maxSpeed);
    
    // Getters para las velocidades del player
    public float GetPlayerCurrentVelocityY() => playerCurrentVelocityY;
    public float GetPlayerLinearVelocityY() => playerLinearVelocityY;
    
    // Progreso hacia la velocidad máxima (0-1)
    public float GetSpeedProgress() => currentSpeed / maxSpeed;
    
    // Método para usar linearVelocity en el futuro
    public void UseLinearVelocityInfluence(bool enable)
    {
        if (enable)
        {
            // Aquí puedes agregar lógica para usar linearVelocity
            float linearInfluence = Mathf.Abs(playerLinearVelocityY * linearVelocityMultiplier);
            currentSpeed = Mathf.Max(currentSpeed, linearInfluence);
            
            if (showDebugInfo)
            {
                Debug.Log($"🔄 Usando LinearVelocity influence: {linearInfluence:F2}");
            }
        }
    }
}