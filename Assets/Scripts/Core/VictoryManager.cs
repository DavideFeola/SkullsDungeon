using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    public GameObject victoryPanel;
    
    void Start()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
    }
    
    public void MostraVittoria()
    {
        
        if (victoryPanel != null)
        {
            
            // ⬅️ ATTIVA TUTTI I PARENT (Canvas, VictoryPanel)
            Transform current = victoryPanel.transform;
            while (current != null)
            {
                if (!current.gameObject.activeSelf)
                {
                    current.gameObject.SetActive(true);
                }
                current = current.parent;
            }
            
            // Assicurati che VictoryPanel sia attivo
            victoryPanel.SetActive(true);            
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogError("❌ [VictoryManager] victoryPanel è NULL!");
        }
        
        // Disabilita il player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var controller = player.GetComponent<PlayerController>();
            if (controller != null) controller.enabled = false;
            
            var weapon = player.GetComponentInChildren<Weapon>();
            if (weapon != null) weapon.enabled = false;
        }
        
    }
    
    public void Restart()
    {
        Time.timeScale = 1f;
        
        // ⬅️ NUOVO: Ferma la musica prima di riavviare
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusicImmediately(clearClip: true);
        }
        
        // Distruggi il player persistente
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Destroy(player);
        }
        
        // Distruggi il CoinCollector
        if (CoinCollector.Instance != null)
        {
            Destroy(CoinCollector.Instance.gameObject);
        }
        
        // Distruggi RoomTransition se esiste
        if (RoomTransition.Instance != null)
        {
            Destroy(RoomTransition.Instance.gameObject);
        }
        
        // Torna alla Stanza 1
        SceneManager.LoadScene("Stanza1");
    }
    
    public void TornaAlMenu()
    {
        Time.timeScale = 1f;
        
        // ⬅️ NUOVO: Ferma e distruggi AudioManager PRIMA di tutto
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusicImmediately(clearClip: true);
            Destroy(AudioManager.Instance.gameObject);
        }
        
        // Distruggi tutti i singleton
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) Destroy(player);
        
        if (CoinCollector.Instance != null)
            Destroy(CoinCollector.Instance.gameObject);
        
        if (RoomTransition.Instance != null)
            Destroy(RoomTransition.Instance.gameObject);
        
        if (GameManager.I != null)
            Destroy(GameManager.I.gameObject);
        
        // Carica il menu
        SceneManager.LoadScene("MainMenu");
    }
}