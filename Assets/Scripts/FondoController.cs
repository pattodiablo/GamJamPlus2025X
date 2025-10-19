using System;
using UnityEngine;

public class VerticalBackgroundScroll : MonoBehaviour
{
    [Header("Movimiento")]
    public float initialSpeed = 1f;       // velocidad inicial
    public float acceleration = 1f;     // aceleración normal

    [Header("Player Reference")]
    [SerializeField] private PlayerController playerController;

    private float currentSpeed;
    private Transform bg1;
public float reverseMultiplier = -0.5f; // factor de velocidad invertida (puedes usar -1 para invertir totalmente)

    public float lastSpeed;
    private Transform bg2;
    private float currentAcceleration;
private float savedSpeedBeforeReverse;
    private bool isReversing = false;
    private float backgroundHeight;
    private float reverseTimer = 0f;
    public float reverseDuration = 1f; // duración del efecto de reverso en segundos

    void Start()
    {
        //Debug.Log(acceleration);
        currentSpeed = initialSpeed;
        currentAcceleration = acceleration;

        bg1 = transform.GetChild(0);
        bg2 = transform.GetChild(1);

        // Calcula altura real del sprite
        SpriteRenderer sr = bg1.GetComponent<SpriteRenderer>();
        backgroundHeight = sr.bounds.size.y;

        // Posiciona bg2 debajo de bg1
        bg2.position = new Vector3(bg1.position.x, bg1.position.y - backgroundHeight, bg1.position.z);

        // Suscribirse al evento de colisión del player
        if (playerController != null)
        {
            playerController.OnPlayerCollision.AddListener(OnPlayerCollisionTriggered);
        
        }
        else
        {
      
            // Intentar encontrar el PlayerController automáticamente
            playerController = FindFirstObjectByType<PlayerController>();
            if (playerController != null)
            {
                playerController.OnPlayerCollision.AddListener(OnPlayerCollisionTriggered);
            
            }
            else
            {
                Debug.LogError("❌ No se encontró ningún PlayerController en la escena");
            }
        }
    }

    private void OnPlayerCollisionTriggered()
    {
        
        if (!isReversing)
        {
            isReversing = true;
            reverseTimer = 0f; // reinicia el timer
            savedSpeedBeforeReverse = currentSpeed;     // guarda la velocidad antes del cambio
          
            currentSpeed *= reverseMultiplier;          // invierte la velocidad (por ejemplo, la hace negativa)
        
        }
        else
        {
          //  Debug.Log("Ya estaba en reverso, ignorando evento");
        }
    }

    void Update()
    {
    
        lastSpeed = currentSpeed;

        // Lógica de finalización del reverso y aceleración normal
        if (!isReversing)
        {
            // sigue acelerando normalmente hacia arriba
            currentSpeed += acceleration * Time.deltaTime;
        }
        else
        {
            // Contar tiempo de reverso
            reverseTimer += Time.deltaTime;
            
            // Terminar reverso después de la duración especificada
            if (reverseTimer >= reverseDuration)
            {
                isReversing = false;
                reverseTimer = 0f;
           
                // recupera la velocidad donde se quedó antes de la reversa
                currentSpeed = savedSpeedBeforeReverse * 0.25f; // Reducción más drástica del 75%
             
            }
        }

        

        // Movimiento
        Vector3 movement = Vector3.up * currentSpeed * Time.deltaTime;
        bg1.Translate(movement);
        bg2.Translate(movement);

        // Reacomodo infinito
        if (bg1.position.y >= backgroundHeight)
            bg1.position = new Vector3(bg1.position.x, bg2.position.y - backgroundHeight + 0.01f, bg1.position.z);

        if (bg2.position.y >= backgroundHeight)
            bg2.position = new Vector3(bg2.position.x, bg1.position.y - backgroundHeight + 0.01f, bg2.position.z);
    }

    void OnDestroy()
    {
        // Desuscribirse del evento para evitar memory leaks
        if (playerController != null)
        {
            playerController.OnPlayerCollision.RemoveListener(OnPlayerCollisionTriggered);
        }
    }
}
