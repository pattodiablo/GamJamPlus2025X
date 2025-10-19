using UnityEngine;

public class DoExplosion : MonoBehaviour
{
    [Header("Audio Interacción Meteoro")]
    [SerializeField] private AudioSource sfxBoom;

    public bool destroySelf = true;
    [SerializeField] private GameObject explodePrefab;
    public void Explode(Transform at)
    {
        if (explodePrefab == null || at == null) return;
        Instantiate(explodePrefab, at.position, Quaternion.identity);
        sfxBoom.Play();

        if (destroySelf)
        {
            Destroy(gameObject);
        }   
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
