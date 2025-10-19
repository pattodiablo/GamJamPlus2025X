using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    public float speed = 2f;
    
    private float destroyHeight;  // Altura donde se destruye la plataforma
    private bool initialized = false;
    private Rigidbody rb;

    void Start()
    {
        // Verificar que este objeto es una instancia, no un prefab
        if (gameObject.scene.name == null)
        {
            Debug.LogError("❌ PlatformMover: Este componente está en un prefab, no en una instancia!");
            return;
        }
        
        // Obtener o agregar Rigidbody
        SetupRigidbody();
        
        CalculateDestroyHeight();
        
        Debug.Log($"🔧 PlatformMover iniciado en instancia: {gameObject.name} (ID: {gameObject.GetInstanceID()})");
    }

    void SetupRigidbody()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("❌ PlatformMover: No se encontró Rigidbody en el prefab!");
            return;
        }
        
        // Configurar Rigidbody para movimiento controlado
        rb.useGravity = false;  // No usar gravedad
        rb.linearDamping = 0f;           // Sin resistencia al aire
        rb.angularDamping = 0f;    // Sin resistencia angular
        
        // Congelar rotaciones si es necesario (para juego 2D)
        rb.freezeRotation = true;
        
        // Aplicar velocidad inicial hacia arriba
        rb.linearVelocity = Vector3.up * speed;
        
        Debug.Log($"🔧 Rigidbody configurado - Velocidad inicial: {rb.linearVelocity}");
    }

    void CalculateDestroyHeight()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            if (cam.orthographic)
            {
                // Para cámara ortográfica
                destroyHeight = cam.transform.position.y + cam.orthographicSize + 2f;
            }
            else
            {
                // Para cámara perspectiva
                float distance = Mathf.Abs(cam.transform.position.z - transform.position.z);
                Vector3 topCenter = cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, distance));
                destroyHeight = topCenter.y + 2f;
            }
            
            initialized = true;
            Debug.Log($"🔧 PlatformMover: Altura de destrucción calculada = {destroyHeight:F2}");
        }
        else
        {
            Debug.LogError("❌ PlatformMover: No se encontró Camera.main!");
        }
    }

    void Update()
    {
        // Solo ejecutar si está inicializado correctamente
        if (!initialized || rb == null) return;
        
        // Mantener velocidad constante (por si algo la cambia)
        if (rb.linearVelocity.y != speed)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, speed, rb.linearVelocity.z);
        }
        
        // Verificar si debe destruirse
        if (transform.position.y > destroyHeight)
        {
            Debug.Log($"🗑️ Plataforma destruida - Salió de escena en Y = {transform.position.y:F2} (Límite: {destroyHeight:F2})");
            Destroy(gameObject);
        }
    }
    
    // Método para cambiar la velocidad dinámicamente
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, speed, rb.linearVelocity.z);
            Debug.Log($"🔧 Velocidad cambiada a: {speed}");
        }
    }
}
