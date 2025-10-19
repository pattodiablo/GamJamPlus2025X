
using UnityEngine;
using System.Collections;


public class SalvarAmigo : MonoBehaviour
{
    public Animator animator;   
    [SerializeField] private float disappearHorizontalSpeed = 0.5f;


    private Rigidbody2D rb;

    public GameObject magicShield;
    private bool rotatedY180;

    public bool isRescued = false;

    public EnergyCounter energyCounter;

    private int energyNumber;
    [Header("Energy lookup")]
    [SerializeField] private string canvasName = "UICanvas"; // opcional: nombre del Canvas
    [SerializeField] private string canvasTag = "";          // opcional: tag del Canvas

    [Header("Audio Amiwi")]
    [SerializeField] private AudioSource AmigoSpawn;
    [SerializeField] private AudioSource gracias;

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

        magicShield.SetActive(false);
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (Random.value < 0.5f)
        {
            transform.Rotate(0f, 180f, 0f, Space.Self);
        }

        float yAng = transform.eulerAngles.y % 360f;
        rotatedY180 = Mathf.Abs(Mathf.DeltaAngle(yAng, 180f)) < 1f;

         Invoke(nameof(PlaySpawnAudio), 3f);
    }

     private void PlaySpawnAudio()
    {
       Debug.LogWarning("Va a sonar audio de spawn amigo");
        AmigoSpawn.Play();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        int energyValue = energyCounter != null ? energyCounter.currentEnergy : 0;
        if (energyCounter == null)
            Debug.LogWarning("EnergyCounter no encontrado. Asigna en la instancia en escena o ajusta canvasName/canvasTag.");
            gracias.Play();

        if (energyValue > 0)
        {
            other.SendMessage("AddPoints", 100, SendMessageOptions.DontRequireReceiver);
            other.SendMessage("UseBattery", 1, SendMessageOptions.DontRequireReceiver);
            animator.SetBool("IsRecued", true);
            isRescued = true;
            desaparecer();
        }
    }
    
    public void AmigoDestroy()
    {
         // Instanciar explosión en la posición actual


        GameObject.Destroy(gameObject, 0f);
    }
    void desaparecer()
    {
        if (magicShield != null) magicShield.SetActive(true);
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        float dir = rotatedY180 ? -1f : 1f;
        rb.linearVelocity = new Vector2(dir * disappearHorizontalSpeed, rb.linearVelocity.y); // FIX: velocity
    }   
}
