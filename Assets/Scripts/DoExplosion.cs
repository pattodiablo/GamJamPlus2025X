using UnityEngine;

public class DoExplosion : MonoBehaviour
{
    [SerializeField] private GameObject explodePrefab;
    public void Explode(Transform at)
    {
        if (explodePrefab == null || at == null) return;
        Instantiate(explodePrefab, at.position, Quaternion.identity);
    }


    public class AutoDestroyOnAnimationEvent : MonoBehaviour
    {
        // Llama este método desde un Animation Event al final del clip
        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }

}
