using UnityEngine;

public class boomTrigger : MonoBehaviour
{

    public AudioSource boomSource;

    // Se llama una vez al inicio para inicializar
    void Start()
    {
        boomSource = GetComponent<AudioSource>();
    }

    // Se activa cuando otro objeto con un Collider y Rigidbody entra en este trigger
    void OnTriggerEnter(Collider other)
    {
        // Opcional: Puedes usar una etiqueta para verificar si es el objeto correcto
         if (other.CompareTag("Player"))
         {
            
             boomSource.Play();
         }

        // Si no necesitas filtrar por etiqueta, simplemente reproduce el sonido
        
    }
}
