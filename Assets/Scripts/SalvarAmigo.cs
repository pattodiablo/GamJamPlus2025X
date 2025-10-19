using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SalvarAmigo : MonoBehaviour
{
    public Animator animator;   
    [SerializeField] private float disappearHorizontalSpeed = 0.5f;

    private Rigidbody2D rb;

    public GameObject magicShield;
    private bool rotatedY180;

    public bool isRescued = false;

    [Header("UI Score System")]
    [Tooltip("Texto UI que muestra la puntuación")]
    public Text puntuacionText; // Para UI Text
    
    [Tooltip("Texto UI que muestra la puntuación (TextMeshPro)")]
    public TextMeshProUGUI puntuacionTMP; // Para TextMeshPro
    
    [Tooltip("Buscar automáticamente el texto 'Puntuacion' en la escena")]
    public bool autoFindPuntuacionText = true;

    // Contador estático para mantener la puntuación entre instancias
    private static int scoreCounter = 0;

    public GameObject Fondo;
    public EnergyCounter energyCounter;

    private int energyNumber;
    [Header("Energy lookup")]
    [SerializeField] private string canvasName = "UICanvas"; // opcional: nombre del Canvas
    [SerializeField] private string canvasTag = "";          // opcional: tag del Canvas

    private VerticalBackgroundScroll speedFondo;
    
    [Header("Movement Sync")]
    [Tooltip("Sincronizar velocidad con el fondo")]
    public bool syncWithBackground = true;

    void Awake()
    {
        if (energyCounter == null)
        {
            GameObject canvasGO = null;
            if (!string.IsNullOrEmpty(canvasTag))
                canvasGO = GameObject.FindWithTag(canvasTag);
            if (canvasGO == null && !string.IsNullOrEmpty(canvasName))
                canvasGO = GameObject.Find(canvasName);
            if (canvasGO != null)
                energyCounter = canvasGO.GetComponentInChildren<EnergyCounter>(true);

            if (energyCounter == null)
                energyCounter = FindObjectOfType<EnergyCounter>(true);
        }
    }

    void Start()
    {
        speedFondo = FindObjectOfType<VerticalBackgroundScroll>();
        magicShield.SetActive(false);
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Configuración inicial del Rigidbody
        if (speedFondo != null && syncWithBackground)
        {
            rb.gravityScale = 0f; // Desactivar gravedad para control total
            rb.linearDamping = 0f; // Sin resistencia
            rb.angularDamping = 0f; // Sin resistencia angular
        }

        // Buscar automáticamente el texto "Puntuacion" en la escena
        if ((puntuacionText == null && puntuacionTMP == null) && autoFindPuntuacionText)
        {
            GameObject puntuacionObj = GameObject.Find("Puntuacion");
            if (puntuacionObj != null)
            {
                // Intentar obtener componente Text
                puntuacionText = puntuacionObj.GetComponent<Text>();
                
                // Si no tiene Text, intentar TextMeshPro
                if (puntuacionText == null)
                {
                    puntuacionTMP = puntuacionObj.GetComponent<TextMeshProUGUI>();
                }
            }
        }
        
        if (Random.value < 0.5f)
        {
            transform.Rotate(0f, 180f, 0f, Space.Self);
        }

        float yAng = transform.eulerAngles.y % 360f;
        rotatedY180 = Mathf.Abs(Mathf.DeltaAngle(yAng, 180f)) < 1f;

        Invoke(nameof(PlaySpawnAudio), 4f);
    }

    void FixedUpdate()
    {
        // Sincronizar velocidad con el fondo continuamente
        if (speedFondo != null && rb != null && syncWithBackground && !isRescued)
        {
            // Igualar la velocidad del Rigidbody a la velocidad del fondo (hacia arriba)
            float fondoSpeed = speedFondo.GetCurrentSpeed();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fondoSpeed*0.1f);
        }
    }

    private void PlaySpawnAudio()
    {
        Debug.LogWarning("Va a sonar audio de spawn amigo");
        // AudioManager.Instance.PlaySound("AmigoSpawn");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        int energyValue = energyCounter != null ? energyCounter.currentEnergy : 0;
        if (energyCounter == null)
            Debug.LogWarning("EnergyCounter no encontrado. Asigna en la instancia en escena o ajusta canvasName/canvasTag.");

        if (energyValue > 0)
        {
            if (!isRescued)
            {
                puntaje();
                
                other.SendMessage("UseBattery", 1, SendMessageOptions.DontRequireReceiver);
                animator.SetBool("IsRecued", true);
                isRescued = true;
                desaparecer();
            }
        }
    }
    
    public void AmigoDestroy()
    {
        // Instanciar explosión en la posición actual
        GameObject.Destroy(gameObject, 0f);
    }

    void desaparecer()
    {
        magicShield.SetActive(true);
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        float dir = rotatedY180 ? -1f : 1f; // 180° → izquierda, 0° → derecha
        rb.linearVelocity = new Vector2(dir * disappearHorizontalSpeed, rb.linearVelocity.y);
    }

    void puntaje()
    {
        // Incrementar contador de 1 en 1
        scoreCounter++;

        // Actualizar el texto UI
        UpdatePuntuacionUI();
    }
    
    void UpdatePuntuacionUI()
    {
        string scoreText = scoreCounter.ToString();

        // Actualizar UI Text si está asignado
        if (puntuacionText != null)
        {
            puntuacionText.text = scoreText;
        }
        // Actualizar TextMeshPro si está asignado
        else if (puntuacionTMP != null)
        {
            puntuacionTMP.text = scoreText;
        }
    }

    // Método público para obtener la velocidad actual sincronizada
    public float GetSyncedSpeed()
    {
        return speedFondo != null ? speedFondo.GetCurrentSpeed() : 0f;
    }

    // Método para activar/desactivar la sincronización
    public void SetSyncWithBackground(bool sync)
    {
        syncWithBackground = sync;
    }
}
