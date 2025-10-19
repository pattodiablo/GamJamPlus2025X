using UnityEngine;

public class BigMetheorController : MonoBehaviour
{
    [Header("Movimiento vertical lento")]
    [SerializeField] private float amplitude = 0.25f;   // qué tanto sube/baja
    [SerializeField] private float frequency = 0.5f;    // ciclos por segundo
    [SerializeField] private bool randomizePhase = true;

    private float phase = 0f;
    private float prevOffset = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (randomizePhase) phase = Random.Range(0f, Mathf.PI * 2f);
    }

    // Update is called once per frame
    void Update()
    {
        float newOffset = Mathf.Sin((Time.time * Mathf.PI * 2f * frequency) + phase) * amplitude;
        float delta = newOffset - prevOffset;
        prevOffset = newOffset;

        // Superpone el bobbing sin interferir con otros movimientos
        transform.position += new Vector3(0f, delta, 0f);
    }
}
