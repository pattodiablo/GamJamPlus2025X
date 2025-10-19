using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EsperarHistoria : MonoBehaviour
{
    [SerializeField] private float delay = 4f;
    [SerializeField] private string sceneName = "MainGame";

    void Start()
    {
        StartCoroutine(LoadAfterDelay());
    }

    private IEnumerator LoadAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
