using UnityEngine;

public class MobileInputManager : MonoBehaviour
{
    public static MobileInputManager Instance;
    
    [Header("Joysticks")]
    public VirtualJoystick movementJoystick;
    public VirtualJoystick shootingJoystick;
    
    [Header("Mobile Controls")]
    public GameObject mobileControlsUI;
    
    [Header("Settings")]
    public bool isMobile = true; // Imposta a true per testare su PC
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Rileva automaticamente se è mobile
        #if UNITY_ANDROID || UNITY_IOS
            isMobile = true;
        #else
            // Su PC puoi testare impostando manualmente isMobile = true nell'Inspector
        #endif
        
        if (mobileControlsUI != null)
        {
            mobileControlsUI.SetActive(isMobile);
        }
    }
    
    public Vector2 GetMovementInput()
    {
        if (isMobile && movementJoystick != null)
        {
            return movementJoystick.GetInputVector();
        }
        return Vector2.zero;
    }
    
    public Vector2 GetShootingInput()
    {
        if (isMobile && shootingJoystick != null)
        {
            return shootingJoystick.GetInputVector();
        }
        return Vector2.zero;
    }
    
    public bool IsUsingMobileControls()
    {
        return isMobile;
    }
}
