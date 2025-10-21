using UnityEngine;
using UnityEngine.SceneManagement;

public class VidaController : MonoBehaviour
{
    public GameObject barra1;
    public GameObject barra2;
    public GameObject barra3;
    public GameObject barra4;
    public GameObject barra5;
    public GameObject barra6;
    public GameObject barra7;
    public GameObject barra8;
    public GameObject barra9;
    public GameObject barra10;

    [Header("Configuración")]
    [Tooltip("Vida actual (0..vidaMaxima)")]
    [SerializeField] private int vidaActual = 10;
    [Tooltip("Vida máxima (no puede exceder el número de barras)")]
    [SerializeField] private int vidaMaxima = 10;

    [Header("Debug")]
    public bool showDebugInfo = false;

    private GameObject[] barras;

    // Evita cargar la pantalla de fin varias veces
    private bool isGameOver = false;

    void Start()
    {
        barras = new GameObject[]
        {
            barra1, barra2, barra3, barra4, barra5,
            barra6, barra7, barra8, barra9, barra10
        };

        // Clamp vidaMaxima a la cantidad de barras disponibles
        vidaMaxima = Mathf.Clamp(vidaMaxima, 0, barras.Length);
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        VerificarBarras();
        ActualizarBarrasVida();

        // Verificar estado inicial (por si empieza en 0)
        CheckGameOver();
    }

    void VerificarBarras()
    {
        for (int i = 0; i < barras.Length; i++)
        {
            if (barras[i] == null)
            {
                if (showDebugInfo) Debug.LogWarning($"VidaController: Barra {i + 1} no asignada.");
            }
        }
    }

    void ActualizarBarrasVida()
    {
        for (int i = 0; i < barras.Length; i++)
        {
            if (barras[i] == null) continue;
            bool activa = (i < vidaActual);
            barras[i].SetActive(activa);
        }

    }

    // Añade vida (1..n). No supera vidaMaxima.
    public void AnadirVida(int cantidad = 1)
    {
        if (cantidad <= 0) return;
        int antes = vidaActual;
        vidaActual = Mathf.Clamp(vidaActual + cantidad, 0, vidaMaxima);
        if (vidaActual != antes) ActualizarBarrasVida();
    }

    // Resta vida (1..n). No baja de 0.
    public void RestarVida(int cantidad = 1)
    {
        if (cantidad <= 0) return;
        int antes = vidaActual;
        vidaActual = Mathf.Clamp(vidaActual - cantidad, 0, vidaMaxima);
        if (vidaActual != antes) ActualizarBarrasVida();

        // Verificar si llegó a 0
        CheckGameOver();
    }

    // Establece vida directamente (clamped)
    public void EstablecerVida(int nuevaVida)
    {
        nuevaVida = Mathf.Clamp(nuevaVida, 0, vidaMaxima);
        if (vidaActual != nuevaVida)
        {
            vidaActual = nuevaVida;
            ActualizarBarrasVida();
            CheckGameOver();
        }
    }

    // Ajusta la vida máxima (y recorta si hace falta)
    public void SetVidaMaxima(int nuevaMaxima)
    {
        nuevaMaxima = Mathf.Clamp(nuevaMaxima, 0, barras.Length);
        vidaMaxima = nuevaMaxima;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);
        ActualizarBarrasVida();
    }

    // Getters
    public int GetVidaActual() => vidaActual;
    public int GetVidaMaxima() => vidaMaxima;

    // Comprueba si la vida llegó a cero y carga la escena "Fin"
    void CheckGameOver()
    {
        if (isGameOver) return;
        if (vidaActual <= 0)
        {
            isGameOver = true;
            if (showDebugInfo) Debug.Log("VidaController: Vida en 0 -> Cargando escena 'Fin'");
            SceneManager.LoadScene("Fin", LoadSceneMode.Single);
        }
    }
}
