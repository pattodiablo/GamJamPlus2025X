using UnityEngine;

public class MetheorLifeTime : MonoBehaviour
{
    [SerializeField] private float lifetime = 8f; // segundos

    private void Start()
    {
        Invoke(nameof(DestroySelf), Mathf.Max(0.01f, lifetime));
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
