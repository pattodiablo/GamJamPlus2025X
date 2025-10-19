using UnityEngine;

public class BatteryController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 20f; // grados/segundo (suave)
    [SerializeField] private bool randomizeDirection = true;

    [SerializeField] private GameObject rayAnimationsPrefab; // <- Prefab de rayos a instanciar
    [SerializeField] private AudioSource playerPWRup;

    // 1 = horario (derecha), -1 = antihorario (izquierda)
    private int direction = 1;
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (randomizeDirection)
            direction = Random.value < 0.5f ? -1 : 1;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, 0f, direction * rotationSpeed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
   if (!other.CompareTag("Player")) return;

            
        var energy = other.GetComponentInParent<PlayerController2D>(); // o tu script real
    

        if (energy != null)
        {
            energy.AddBattery(1); // ajusta al método real
            playerPWRup.Play();
        }

        // Instanciar rayos antes de destruir
        if (rayAnimationsPrefab != null)
        {
            Instantiate(rayAnimationsPrefab, transform.position, other.transform.rotation);
        }
        playerPWRup.Play();

        GetComponent<Collider2D>().enabled = false;

        Invoke(nameof(DestroyDelayed), 0.5f);

        


    }
    private void DestroyDelayed()
    {
          
        Destroy(gameObject);
    }
}
