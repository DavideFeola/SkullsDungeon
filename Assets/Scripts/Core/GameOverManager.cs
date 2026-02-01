using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    
    void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }
    
    public void MostraGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            // ⭐ CHIAMA LA MUSICA DI GAME OVER
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayGameOverMusic();
            }
            
            Time.timeScale = 0f;
        }
    }
    
    public void ReloadScene()
    {
        Time.timeScale = 1f;
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ResetToShopMusic();
        }
        
        // Nascondi il pannello
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        // Distruggi il CoinCollector singleton
        if (CoinCollector.Instance != null)
        {
            Destroy(CoinCollector.Instance.gameObject);
        }
        
        // 🔄 RESETTA IL PLAYER PRIMA DI RICARICARE
        PlayerHealth playerHealth = FindAnyObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.ResetPlayer();
        }
        
        // Carica la prima scena
        SceneManager.LoadScene(1);
    }
    
    public void TornaAlMenu()
    {
        Time.timeScale = 1f;
        
        // ⬅️ Ferma e distruggi AudioManager PRIMA di tutto
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusicImmediately(clearClip: true);
            Destroy(AudioManager.Instance.gameObject);
        }
        
        // Distruggi il Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) 
            Destroy(player);
        
        // Distruggi CoinCollector
        if (CoinCollector.Instance != null)
            Destroy(CoinCollector.Instance.gameObject);
        
        // Distruggi RoomTransition
        if (RoomTransition.Instance != null)
            Destroy(RoomTransition.Instance.gameObject);
        
        // Distruggi GameManager
        if (GameManager.I != null)
            Destroy(GameManager.I.gameObject);
        
        // 🔹 Nascondi il pannello prima di cambiare scena
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        
        // Carica il menu (scena 0 o "MainMenu")
        SceneManager.LoadScene("MainMenu"); // oppure SceneManager.LoadScene(0);
    }
}
   