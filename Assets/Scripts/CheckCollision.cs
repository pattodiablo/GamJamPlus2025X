using System.Diagnostics;
using UnityEngine;

public class CheckCollision : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool doExplosionOnCollision = false;
    public bool doExplosionOnTrigger = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        UnityEngine.Debug.Log("Colisión detectada en CheckCollision.cs");

        if (!collision.collider.CompareTag("Player")) return;

        collision.collider.SendMessage("PlayerHurt", SendMessageOptions.DontRequireReceiver);
    }

      private void OnTriggerEnter2D(Collider2D other)
    {
         UnityEngine.Debug.Log("Trigger detectado en CheckCollision.cs");


        if (other.CompareTag("Player"))
        {
             other.SendMessage("PlayerHurt", SendMessageOptions.DontRequireReceiver);
            if (doExplosionOnTrigger)
            {
                UnityEngine.Debug.Log("Tratando de explotar");
                GetComponent<DoExplosion>()?.Explode(transform);
            }
        }
           

       
    }

}
