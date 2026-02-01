using UnityEngine;
using TMPro;

public class SignInteraction : MonoBehaviour
{
    [Header("UI References")]
    public GameObject signPanel;
    public TextMeshProUGUI dialogueText;
    
    [Header("Sign Content")]
    [TextArea(3, 10)]
    public string signMessage = "Benvenuto! Questo è un cartello informativo.";
    
    [Header("Typewriter Effect")]
    public bool useTypewriter = true;
    public DialogueTypewriter dialogueTypewriter; // Opzionale
    
    [Header("Auto Settings")]
    public bool autoOpenOnEnter = true;
    public bool autoCloseOnExit = false;
    
    // ← RIMOSSA isPlayerNear perché non serve
    
    void Start()
    {
        if (signPanel != null)
            signPanel.SetActive(false);
    }
    
    void Update()
    {
        // Chiudi con ESC
        if (signPanel != null && signPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseSign();
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (autoOpenOnEnter)
            {
                OpenSign();
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (autoCloseOnExit && signPanel != null && signPanel.activeSelf)
            {
                CloseSign();
            }
        }
    }
    
    void OpenSign()
    {
        if (signPanel != null)
        {
            signPanel.SetActive(true);
            
            // Se usi il typewriter
            if (useTypewriter && dialogueTypewriter != null)
            {
                dialogueTypewriter.StartDialogue();
            }
            // Altrimenti mostra il testo direttamente
            else if (dialogueText != null)
            {
                dialogueText.text = signMessage;
            }
            
            Time.timeScale = 0f; // Pausa il gioco
        }
    }
    
    public void CloseSign()
    {
        if (signPanel != null)
        {
            signPanel.SetActive(false);
            Time.timeScale = 1f; // Riprendi il gioco
        }
    }
}