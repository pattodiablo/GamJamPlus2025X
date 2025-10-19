using UnityEngine;

public class SalvarAmigo : MonoBehaviour
{
    public Animator animator;   
    [SerializeField] private float disappearHorizontalSpeed = 0.5f;


    private Rigidbody2D rb;

    public GameObject magicShield;
    private bool rotatedY180;

    public bool isRescued = false;

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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.SendMessage("AddPoints", 100, SendMessageOptions.DontRequireReceiver);
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
               magicShield.SetActive(true);
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        float dir = rotatedY180 ? -1f : 1f; // 180° → izquierda, 0° → derecha
        rb.linearVelocity = new Vector2(dir * disappearHorizontalSpeed, rb.linearVelocity.y);
    }   
}
