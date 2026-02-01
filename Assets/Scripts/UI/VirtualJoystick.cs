using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [Header("Joystick Components")]
    public RectTransform background;
    public RectTransform stick;
    
    [Header("Settings")]
    public float maxDistance = 50f;
    
    private Vector2 inputVector;
    private Vector2 stickStartPos; // Rinominato per chiarezza
    
    void Start()
    {
        // Salva la posizione iniziale dello stick (0, -5)
        stickStartPos = stick.anchoredPosition;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background, 
            eventData.position, 
            eventData.pressEventCamera, 
            out pos))
        {
            pos.x = (pos.x / background.sizeDelta.x);
            pos.y = (pos.y / background.sizeDelta.y);
            
            inputVector = new Vector2(pos.x * 2, pos.y * 2);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;
            
            // Muovi lo stick partendo dalla sua posizione iniziale
            stick.anchoredPosition = stickStartPos + new Vector2(
                inputVector.x * maxDistance,
                inputVector.y * maxDistance
            );
        }
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        // Ritorna alla posizione iniziale salvata (0, -5)
        stick.anchoredPosition = stickStartPos;
    }
    
    public Vector2 GetInputVector()
    {
        return inputVector;
    }
    
    public float GetHorizontal()
    {
        return inputVector.x;
    }
    
    public float GetVertical()
    {
        return inputVector.y;
    }
}