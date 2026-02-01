using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class RoomTransition : MonoBehaviour
{
    public static RoomTransition Instance;
    
    [Header("UI")]
    public GameObject transitionPanel;
    public TextMeshProUGUI roomText;
    
    [Header("Settings")]
    public float fadeDuration = 0.5f;
    public float displayDuration = 1.5f;
    
    private CanvasGroup canvasGroup;
    
    void Awake()
    {
        // Singleton persistente
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Aggiungi CanvasGroup se non c'è
        canvasGroup = transitionPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = transitionPanel.AddComponent<CanvasGroup>();
        }
        
        transitionPanel.SetActive(false);
    }
    
    public void TransitionToRoom(int roomNumber, string sceneName)
    {
        StartCoroutine(TransitionCoroutine(roomNumber, sceneName));
    }
    
    IEnumerator TransitionCoroutine(int roomNumber, string sceneName)
    {
        // Mostra il pannello
        transitionPanel.SetActive(true);
        roomText.text = "Room " + roomNumber + "...";
        
        // Fade in (da trasparente a nero)
        yield return StartCoroutine(Fade(0f, 1f));
        
        // Carica la nuova scena
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        // Aspetta che finisca di caricare
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        // Aspetta un po' per mostrare "Stanza X..."
        yield return new WaitForSeconds(displayDuration);
        
        // Fade out (da nero a trasparente)
        yield return StartCoroutine(Fade(1f, 0f));
        
        // Nascondi il pannello
        transitionPanel.SetActive(false);
    }
    
    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            yield return null;
        }
        
        canvasGroup.alpha = endAlpha;
    }
}
