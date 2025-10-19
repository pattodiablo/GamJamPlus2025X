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
        
            return;
        }
        
        // Obtener o agregar Rigidbody
        SetupRigidbody();
        
        CalculateDestroyHeight();
        
   
    }

    void SetupRigidbody()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
  
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
       
        }
        else
        {
         
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
         
        }
    }
}
