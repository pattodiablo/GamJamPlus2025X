using UnityEngine;

public class WorldBounds : MonoBehaviour
{
    [Header("Bounds Settings")]
    public bool useWorldBounds = true;        // Activar límites de mundo
    public Rect worldRect = new Rect(-10, -5, 20, 10); // x, y, width, height
    
    [Header("Behavior")]
    public bool bounceOnBounds = false;       // Rebotar en los límites
    public float bounceForce = 0.5f;          // Fuerza del rebote (0-1)
    public bool stopOnBounds = true;          // Parar movimiento en los límites
    public bool wrapAround = false;           // Teletransportar al lado opuesto
    
    [Header("Visual Debug")]
    public bool showBounds = true;            // Mostrar límites en Scene view
    public Color boundsColor = Color.yellow;  // Color de los límites
    
    private Rigidbody2D rb;
    private Vector3 lastValidPosition;
    
    // Propiedades para acceso fácil
    public float WorldLeft => worldRect.xMin;
    public float WorldRight => worldRect.xMax;
    public float WorldBottom => worldRect.yMin;
    public float WorldTop => worldRect.yMax;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastValidPosition = transform.position;
    }
    
    void FixedUpdate()
    {
        if (useWorldBounds)
        {
            ApplyWorldBounds();
        }
    }
    
    void ApplyWorldBounds()
    {
        Vector3 position = transform.position;
        Vector2 velocity = rb != null ? rb.linearVelocity : Vector2.zero;
        bool hitBounds = false;
        
        // Verificar límites horizontales
        if (position.x < WorldLeft)
        {
            if (wrapAround)
            {
                position.x = WorldRight;
            }
            else
            {
                position.x = WorldLeft;
                if (bounceOnBounds && rb != null)
                    velocity.x = -velocity.x * bounceForce;
                else if (stopOnBounds)
                    velocity.x = Mathf.Max(0, velocity.x);
            }
            hitBounds = true;
        }
        else if (position.x > WorldRight)
        {
            if (wrapAround)
            {
                position.x = WorldLeft;
            }
            else
            {
                position.x = WorldRight;
                if (bounceOnBounds && rb != null)
                    velocity.x = -velocity.x * bounceForce;
                else if (stopOnBounds)
                    velocity.x = Mathf.Min(0, velocity.x);
            }
            hitBounds = true;
        }
        
        // Verificar límites verticales
        if (position.y < WorldBottom)
        {
            if (wrapAround)
            {
                position.y = WorldTop;
            }
            else
            {
                position.y = WorldBottom;
                if (bounceOnBounds && rb != null)
                    velocity.y = -velocity.y * bounceForce;
                else if (stopOnBounds)
                    velocity.y = Mathf.Max(0, velocity.y);
            }
            hitBounds = true;
        }
        else if (position.y > WorldTop)
        {
            if (wrapAround)
            {
                position.y = WorldBottom;
            }
            else
            {
                position.y = WorldTop;
                if (bounceOnBounds && rb != null)
                    velocity.y = -velocity.y * bounceForce;
                else if (stopOnBounds)
                    velocity.y = Mathf.Min(0, velocity.y);
            }
            hitBounds = true;
        }
        
        // Aplicar cambios
        if (hitBounds)
        {
            transform.position = position;
            if (rb != null)
            {
                rb.linearVelocity = velocity;
            }
            
            // Llamar evento si el objeto tiene uno
            SendMessage("OnWorldBoundsHit", SendMessageOptions.DontRequireReceiver);
        }
        
        lastValidPosition = position;
    }
    
    // Métodos públicos para configuración dinámica
    public void SetBounds(float left, float right, float bottom, float top)
    {
        worldRect = new Rect(left, bottom, right - left, top - bottom);
    }
    
    public void SetBounds(Rect rect)
    {
        worldRect = rect;
    }
    
    public bool IsInsideBounds(Vector3 position)
    {
        return worldRect.Contains(new Vector2(position.x, position.y));
    }
    
    public Vector3 ClampToBounds(Vector3 position)
    {
        return new Vector3(
            Mathf.Clamp(position.x, WorldLeft, WorldRight),
            Mathf.Clamp(position.y, WorldBottom, WorldTop),
            position.z
        );
    }
    
    // Visualización en el editor
    void OnDrawGizmos()
    {
        if (showBounds)
        {
            Gizmos.color = boundsColor;
            Gizmos.DrawWireCube(worldRect.center, worldRect.size);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (useWorldBounds)
        {
            // Dibujar límites más prominentes cuando está seleccionado
            Gizmos.color = boundsColor;
            Gizmos.DrawWireCube(worldRect.center, worldRect.size);
            
            // Dibujar líneas más gruesas
            Vector3 bottomLeft = new Vector3(WorldLeft, WorldBottom, 0);
            Vector3 bottomRight = new Vector3(WorldRight, WorldBottom, 0);
            Vector3 topLeft = new Vector3(WorldLeft, WorldTop, 0);
            Vector3 topRight = new Vector3(WorldRight, WorldTop, 0);
            
            Gizmos.DrawLine(bottomLeft, bottomRight);
            Gizmos.DrawLine(bottomRight, topRight);
            Gizmos.DrawLine(topRight, topLeft);
            Gizmos.DrawLine(topLeft, bottomLeft);
        }
    }
}