using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; // 🔑 AGGIUNGI QUESTO

public class MobileControlsReactive : MonoBehaviour
{
    [SerializeField] private GameObject mobileControls;
    private static MobileControlsReactive instance;
    bool usingTouch;
    
    void Awake()
    {
        // Singleton: mantieni solo una istanza
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 🔑 Questo è fondamentale!
            Apply(false);
        }
        else
        {
            Destroy(gameObject); // Distruggi duplicati
            return;
        }
    }

    // 🔑 AGGIUNGI QUESTI DUE METODI
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        bool touchPressed =
            (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            || (Touchscreen.current != null && Mouse.current != null && Mouse.current.leftButton.isPressed);
        
        if (touchPressed)
        {
            if (!usingTouch)
            {
                usingTouch = true;
                Apply(true);
            }
            return;
        }
        
        bool mouseOrKeyboard =
            (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            || Input.anyKeyDown;
        
        if (mouseOrKeyboard)
        {
            if (usingTouch)
            {
                usingTouch = false;
                Apply(false);
            }
        }
    }
    
    void Apply(bool enable)
    {
        if (!mobileControls) return;
        if (mobileControls.activeSelf == enable) return;
        mobileControls.SetActive(enable);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Se torniamo al menu, distruggi tutto
        if (scene.name == "Menu" || scene.name == "MainMenu")
        {
            instance = null;
            Destroy(gameObject);
        }
    }
}
