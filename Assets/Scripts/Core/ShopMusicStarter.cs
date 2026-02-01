using UnityEngine;

public class ShopMusicStarter : MonoBehaviour
{
    void Start()
    {
        // Aspetta un frame per essere sicuri che tutto sia caricato
        Invoke("StartShopMusic", 0.1f);
    }

    void StartShopMusic()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayShopMusic();
        }
    }
}
