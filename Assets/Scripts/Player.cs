using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;
    public float margin = 0.01f; // margen adicional alrededor de la vista

    [Header("Efecto de giro")]
    public float spinSpeed = 1000f; // velocidad de rotación
    public int spinCount = 5;       // cantidad de giros completos

    private bool isSpinning = false;
    private Quaternion originalRotation;
    
    [Header("Tilt horizontal")]
    [Tooltip("Ángulo máximo (grados) que inclina el jugador al moverse horizontalmente")]
    public float maxTilt = 15f;
    [Tooltip("Suavizado para interpolar la rotación hacia el tilt objetivo (mayor = más rápido)")]
    public float tiltSmooth = 10f;

    [Header("Rebote")]
    [Tooltip("Fuerza del rebote cuando colisiona con los límites")]
    public float bounceForce = 10f;
    [Tooltip("Duración del efecto de rebote en segundos")]
    public float bounceDuration = 0.1f;

    // Límites dinámicos según la cámara
    private float sceneLimitX;
    private float sceneLimitY;

    // Variables para el rebote
    private Vector3 bounceVelocity = Vector3.zero;
    private float bounceTimer = 0f;
    private bool isBouncing = false;

    public UnityEvent OnPlayerCollision;

    void Start()
    {
        originalRotation = transform.rotation;
        CalculateSceneLimits();
   
    }

    void Update()
    {
        MovePlayer();
        CheckSceneLimits();
    }

    void CalculateSceneLimits()
    {
        // Si la cámara principal es ortográfica, usamos su tamaño visible
        Camera cam = Camera.main;
        if (cam.orthographic)
        {
            sceneLimitY = cam.orthographicSize;
            sceneLimitX = sceneLimitY * cam.aspect;
        }
        else
        {
            // Para cámaras en perspectiva, usamos un valor de referencia
            float distance = Mathf.Abs(cam.transform.position.z - transform.position.z);
            Vector3 corner = cam.ViewportToWorldPoint(new Vector3(1, 1, distance));
            sceneLimitX = corner.x;
            sceneLimitY = corner.y;
        }
    }

    void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Vector3 inputMovement = new Vector3(moveX, moveY, 0f) * speed * Time.deltaTime;
        
        // Aplicar rebote si está activo
        if (isBouncing)
        {
            bounceTimer -= Time.deltaTime;
            
            // Reducir la velocidad de rebote gradualmente
            bounceVelocity = Vector3.Lerp(bounceVelocity, Vector3.zero, Time.deltaTime / bounceDuration);
            
            // Si el rebote terminó, detenerlo
            if (bounceTimer <= 0f)
            {
                isBouncing = false;
                bounceVelocity = Vector3.zero;
                bounceTimer = 0f;
            }
        }
        
        // Combinar movimiento del input con el rebote
        Vector3 totalMovement = inputMovement + bounceVelocity * Time.deltaTime;
        transform.position += totalMovement;
        
        // Aplicar tilt (rotación en Z) basado en la entrada horizontal.
        // No aplicamos tilt si estamos en el efecto de spin completo.
        if (!isSpinning)
        
        {
            float targetAngle = -moveX * maxTilt; // negativo para que gire hacia el lado del movimiento
            Quaternion targetRot = Quaternion.Euler(0f, 0f, targetAngle);
            // Interpolamos suavemente la rotación actual hacia la rotación objetivo
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Mathf.Clamp01(tiltSmooth * Time.deltaTime));
        }
    }

    void CheckSceneLimits()
    {
        float limitX = sceneLimitX + margin;
        float limitY = sceneLimitY + margin;

        bool collided = false;
        float direction = 0f;
        Vector3 bounceDirection = Vector3.zero;

        // Comprobación horizontal
        if (transform.position.x > limitX)
        {
            transform.position = new Vector3(limitX, transform.position.y, transform.position.z);
            collided = true;
            direction = 1f; // choca a la derecha → gira a la izquierda
            bounceDirection = Vector3.left; // rebota hacia la izquierda
           
        }
        else if (transform.position.x < -limitX)
        {
            transform.position = new Vector3(-limitX, transform.position.y, transform.position.z);
            collided = true;
            direction = -1f; // choca a la izquierda → gira a la derecha
            bounceDirection = Vector3.right; // rebota hacia la derecha
       
        }

        // Comprobación vertical
        if (transform.position.y > limitY)
        {
            transform.position = new Vector3(transform.position.x, limitY, transform.position.z);
            collided = true;
            direction = 1f; // para efectos visuales
            bounceDirection = Vector3.down; // rebota hacia abajo
        
        }
        else if (transform.position.y < -limitY)
        {
            transform.position = new Vector3(transform.position.x, -limitY, transform.position.z);
            collided = true;
            direction = -1f; // para efectos visuales
            bounceDirection = Vector3.up; // rebota hacia arriba
          
        }

        if (collided && !isSpinning)
        {
            // Aplicar rebote
            ApplyBounce(bounceDirection);
            
          
            OnPlayerCollision?.Invoke(); // dispara el evento
          //  StartCoroutine(SpinEffect(direction));
        }
    }

    private void ApplyBounce(Vector3 direction)
    {
        bounceVelocity = direction * bounceForce;
        bounceTimer = bounceDuration;
        isBouncing = true;
       
    }

    private System.Collections.IEnumerator SpinEffect(float direction)
    {
        isSpinning = true;
        float totalRotation = 360f * spinCount;
        float rotated = 0f;

        while (rotated < totalRotation)
        {
            float rotationThisFrame = spinSpeed * Time.deltaTime;
            transform.Rotate(0f, 0f, rotationThisFrame * direction);
            rotated += rotationThisFrame;
            yield return null;
        }

        transform.rotation = originalRotation;
        isSpinning = false;
    }
}
