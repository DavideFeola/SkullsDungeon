using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueTypewriter : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI dialogueText;
    
    [Header("Settings")]
    public string message = "Hi, I'm Leo. Welcome to my Shop!What do you need?";
    [Range(0.01f, 0.1f)]
    public float typingSpeed = 0.05f;
    
    private Coroutine typeCoroutine;
    
    public void StartDialogue()
    {
        if (typeCoroutine != null)
        {
            StopCoroutine(typeCoroutine);
        }
        
        typeCoroutine = StartCoroutine(TypeText());
    }
    
    private IEnumerator TypeText()
    {
        dialogueText.text = "";
        
        foreach (char letter in message)
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed); // ← MODIFICATO!
        }
    }
}