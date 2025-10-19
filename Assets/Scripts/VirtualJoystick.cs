using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Joystick Components")]
    public RectTransform joystickBackground; // El fondo del joystick
    public RectTransform joystickHandle;     // El handle/knob del joystick
    
    [Header("Settings")]
    public float handleRange = 50f;          // Distancia máxima que puede moverse el handle
    public float deadZone = 0.1f;            // Zona muerta para evitar micro-movimientos
    
    [Header("Output")]
    public Vector2 inputVector = Vector2.zero; // Vector de entrada (-1 a 1 en X e Y)

    [Header("Touch Area")]
    [SerializeField] private RectTransform touchArea; // Panel full-screen que recibe los toques
    
    private bool isDragging = false;
    private Vector2 centerPosition;
    
    void Start()
    {
        // Guardar la posición central del joystick
        centerPosition = joystickBackground.anchoredPosition;
        
        // Asegurar que el handle esté en el centro al inicio
        joystickHandle.anchoredPosition = Vector2.zero;
    }
    
    void Update()
    {
        // Si no se está arrastrando, el input es cero
        if (!isDragging)
        {
            inputVector = Vector2.zero;
            joystickHandle.anchoredPosition = Vector2.Lerp(joystickHandle.anchoredPosition, Vector2.zero, Time.deltaTime * 5f);
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;

        // Usa el touchArea si está asignado; si no, el padre del background
        var refRect = touchArea != null ? touchArea : (joystickBackground != null ? joystickBackground.parent as RectTransform : null);
        if (joystickBackground != null && refRect != null &&
            RectTransformUtility.ScreenPointToLocalPointInRectangle(refRect, eventData.position, eventData.pressEventCamera, out var localTouch))
        {
            // Convertir al espacio del padre del background
            if (joystickBackground.parent == refRect)
            {
                joystickBackground.anchoredPosition = localTouch;
            }
            else
            {
                // Convertir desde refRect al espacio del padre real del background
                Vector2 bgLocal;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBackground.parent as RectTransform, eventData.position, eventData.pressEventCamera, out bgLocal);
                joystickBackground.anchoredPosition = bgLocal;
            }
        }

        if (joystickHandle != null)
            joystickHandle.anchoredPosition = Vector2.zero;

        // Genera input inmediato desde el primer toque
        OnDrag(eventData);
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        inputVector = Vector2.zero;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBackground, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            // Limitar la distancia del handle
            localPoint = Vector2.ClampMagnitude(localPoint, handleRange);
            joystickHandle.anchoredPosition = localPoint;
            
            // Calcular el vector de entrada normalizado
            inputVector = localPoint / handleRange;
            
            // Aplicar zona muerta
            if (inputVector.magnitude < deadZone)
            {
                inputVector = Vector2.zero;
            }
        }
    }
    
    // Método público para obtener el input desde otros scripts
    public Vector2 GetInputVector()
    {
        return inputVector;
    }
}
