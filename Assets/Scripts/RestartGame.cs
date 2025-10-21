using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RestartGame : MonoBehaviour
{

    // Reinicia inmediatamente la escena "MainGame"
    public void RestartMainGame()
    {
        SceneManager.LoadScene("MainGame", LoadSceneMode.Single);
    }

    // Reinicia la escena después de un retardo (segundos)
    public void RestartMainGameDelayed(float delay)
    {
        StartCoroutine(RestartAfterDelay(delay));
    }

    private IEnumerator RestartAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        RestartMainGame();
    }
}
