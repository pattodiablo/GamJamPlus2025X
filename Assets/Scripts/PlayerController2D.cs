using System.Collections;
using System.Diagnostics;
using System.Security.Claims;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;              // Velocidad base de movimiento
    public float acceleration = 10f;          // Qué tan rápido acelera
    public float deceleration = 15f;          // Qué tan rápido desacelera
    public float maxSpeed = 8f;               // Velocidad máxima
    
    [Header("Floating Effect")]
    public float floatAmplitude = 0.5f;       // Amplitud del efecto flotante
    public float floatFrequency = 2f;         // Frecuencia del efecto flotante
    public bool enableFloatingEffect = true;  // Activar/desactivar efecto flotante
    
    [Header("Gravity Simulation")]
    public float gravityScale = 0.3f;         // Simulación de gravedad reducida
    public Vector2 gravityDirection = Vector2.down; // Dirección de la "gravedad"
    
    [Header("Input")]
    public VirtualJoystick virtualJoystick;   // Referencia al joystick virtual
    
    [Header("Animation")]
    public Animator animator;                 // Referencia al Animator del jugador
    public string joyPowerParam = "JoyPower"; // Nombre del parámetro float en el Animator
    [Range(0f, 1f)] public float joyThresholdDown = 0.5f; // Umbral de activación hacia abajo
    
    [Header("Hurt Settings")]
    [SerializeField] private string hitBoolParam = "GotHit";
    [SerializeField] private float hitFlagTime = 0.6f;
    private Coroutine hitRoutine;

    private Rigidbody2D rb;
    private Vector2 currentVelocity = Vector2.zero;
    private Vector2 inputVector = Vector2.zero;
    private Vector3 initialPosition;
    private float floatTimer = 0f;

    public EnergyCounter energyCounter;

    [Header("Movement Restriction")]
    public bool restrictYMovement = true;    // Si true, bloquea movimiento en Y


    void Start()
    {
        // Obtener componentes
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        // Configurar Rigidbody2D para movimiento espacial
        rb.gravityScale = 0f; // Sin gravedad de Unity, usaremos la nuestra
        rb.linearDamping = 2f;         // Un poco de resistencia para hacer el movimiento más suave
        rb.angularDamping = 5f;  // Resistencia angular

        // Guardar posición inicial para el efecto flotante
        initialPosition = transform.position;

        // Si no se asignó joystick, intentar encontrarlo
        if (virtualJoystick == null)
        {
            virtualJoystick = FindObjectOfType<VirtualJoystick>();
        }

        // Buscar Animator si no está asignado
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }
        }
    }

    void AddPoints(int points)
    {
        // Aquí puedes agregar la lógica para sumar puntos al puntaje del jugador
      //  UnityEngine.Debug.Log($"Puntos añadidos: {points}");
    }
    
    void PlayerHurt()
    {
        animator = GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        if (hitRoutine != null) StopCoroutine(hitRoutine);

        animator.SetBool(hitBoolParam, true);
        hitRoutine = StartCoroutine(ClearHitFlagAfter(hitFlagTime));

    }

    private IEnumerator ClearHitFlagAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (animator != null) animator.SetBool(hitBoolParam, false); // Cambia a true si así lo necesitas
        hitRoutine = null;
    }

    void Update()
    {
        // Obtener input del joystick virtual
        if (virtualJoystick != null)
        {
            inputVector = virtualJoystick.GetInputVector();
        }
        
        // Actualizar JoyPower en Animator cuando se empuja hacia abajo > umbral
        // Nota: Por convención, en UI/joysticks Y hacia arriba es positivo, hacia abajo es negativo.
        if (animator != null && !string.IsNullOrEmpty(joyPowerParam))
        {
            // downStrength = 0 cuando y >= -umbral; 1 cuando y <= -1
            float downStrength = Mathf.Clamp01((-inputVector.y - joyThresholdDown) / (1f - joyThresholdDown));
            animator.SetFloat(joyPowerParam, downStrength);
        }
        
        // Aplicar efecto flotante si está habilitado
        if (enableFloatingEffect)
        {
        //    ApplyFloatingEffect();
        }
    }

    void FixedUpdate()
    {
        // Aplicar movimiento basado en input
        ApplyMovement();

        // Aplicar gravedad reducida
        //ApplySpaceGravity();

        // Limitar velocidad máxima
        LimitMaxSpeed();

        ClampUpAndDown();

    }
    
    void ClampUpAndDown()
    {
    
        // Limitar la posición vertical para evitar que se salga demasiado
        float limitY = 4.5f; // Ajustar según el tamaño del mundo
        if (transform.position.y > limitY)
        {
            transform.position = new Vector3(transform.position.x, limitY, transform.position.z);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }
        else if (transform.position.y < -limitY)
        {
            transform.position = new Vector3(transform.position.x, -limitY, transform.position.z);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }
    }
    
    void ApplyMovement()
    {
        // Calcular velocidad objetivo basada en el input
        Vector2 targetVelocity = inputVector * moveSpeed;
        
        // Acelerar o desacelerar hacia la velocidad objetivo
        if (inputVector.magnitude > 0.1f)
        {
            // Hay input, acelerar hacia la velocidad objetivo
            currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            // No hay input, desacelerar
            currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }
        
         // Aplicar la velocidad al Rigidbody
        if (restrictYMovement)
        {
            rb.linearVelocity = new Vector2(currentVelocity.x, 0f); // bloquea Y
        }
        else
        {
            rb.linearVelocity = new Vector2(currentVelocity.x, rb.linearVelocity.y * 0.5f + currentVelocity.y); // antes: linearVelocity
        }
    }
    
    void ApplySpaceGravity()
    {
        // Aplicar una gravedad reducida para simular el espacio
        Vector2 gravity = gravityDirection * gravityScale;
        rb.AddForce(gravity, ForceMode2D.Force);
    }
    
    void ApplyFloatingEffect()
    {
        // Crear un sutil efecto de flotación
        floatTimer += Time.deltaTime * floatFrequency;
        float floatOffset = Mathf.Sin(floatTimer) * floatAmplitude;
        
        // Aplicar solo si no hay mucho input vertical
        if (Mathf.Abs(inputVector.y) < 0.3f)
        {
            Vector3 floatPosition = new Vector3(
                transform.position.x,
                initialPosition.y + floatOffset,
                transform.position.z
            );
            
            // Suavizar el movimiento hacia la posición flotante
            Vector3 targetPosition = Vector3.Lerp(transform.position, floatPosition, Time.deltaTime * 2f);
            transform.position = new Vector3(transform.position.x, targetPosition.y, transform.position.z);
        }
        else
        {
            // Actualizar posición base cuando hay input vertical
            initialPosition = new Vector3(initialPosition.x, transform.position.y, initialPosition.z);
        }
    }
    
    void LimitMaxSpeed()
    {
        // Limitar la velocidad máxima
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }
    
    // Método para establecer la referencia del joystick desde el inspector o código
    public void SetVirtualJoystick(VirtualJoystick joystick)
    {
        virtualJoystick = joystick;
    }
    
    // Método para obtener la velocidad actual (útil para animaciones)
    public Vector2 GetCurrentVelocity()
    {
        return currentVelocity;
    }

    // Método para verificar si el player se está moviendo
    public bool IsMoving()
    {
        return currentVelocity.magnitude > 0.1f;
    }

    public void AddBattery(int amount)
    {
        // Aquí puedes agregar la lógica para sumar baterías al jugador
        UnityEngine.Debug.Log($"Baterías añadidas: {amount}");
        if (energyCounter != null)
        {
            energyCounter.AddEnergy(amount);

        }
    }

    public void UseBattery(int amount)
    {
        // Aquí puedes agregar la lógica para usar baterías del jugador
        
        if (energyCounter != null)
        {
            energyCounter.RemoveEnergy(amount);

        }
    }
}