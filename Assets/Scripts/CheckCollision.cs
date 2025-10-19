using System.Diagnostics;
using UnityEngine;

public class CheckCollision : MonoBehaviour
{
    [Header("Audio Interacción Meteoro")]
    [SerializeField] private AudioSource sfxBoom;
    [SerializeField] private AudioSource playerHurt;
    [SerializeField] private AudioSource npcFail;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool doExplosionOnCollision = false;
    public bool doExplosionOnTrigger = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
       // UnityEngine.Debug.Log("Colisión detectada en CheckCollision.cs");

        if (!collision.collider.CompareTag("Player")) return;

        collision.collider.SendMessage("PlayerHurt", SendMessageOptions.DontRequireReceiver);
        
    }

      private void OnTriggerEnter2D(Collider2D other)
    {
        //  UnityEngine.Debug.Log("Trigger detectado en CheckCollision.cs");


        if (other.CompareTag("Player"))
        {
            sfxBoom.Play();
            playerHurt.Play();

            other.SendMessage("PlayerHurt", SendMessageOptions.DontRequireReceiver);
            if (doExplosionOnTrigger)
            {
             
                GetComponent<DoExplosion>()?.Explode(transform);
                
            }
        }

        if (other.CompareTag("Amigo"))
        {

          var amigo = other.GetComponent<SalvarAmigo>();
            bool isRescued = amigo != null && amigo.isRescued;

            if (isRescued)
            {
                // Ya rescatado: no destruir ni explotar
                return;
            }

            // No rescatado: destruir y (si quieres) explotar
            amigo.SendMessage("AmigoDestroy", SendMessageOptions.DontRequireReceiver);
            GetComponent<DoExplosion>()?.Explode(transform);
            npcFail.Play();
        }
    }

}
