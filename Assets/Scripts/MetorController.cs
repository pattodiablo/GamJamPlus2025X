using UnityEngine;

public class MetorController : MonoBehaviour
{

   private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        other.SendMessage("PlayerHurt", SendMessageOptions.DontRequireReceiver);
    }

}
