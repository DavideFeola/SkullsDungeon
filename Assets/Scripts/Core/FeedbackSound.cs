using UnityEngine;

public class FeedbackSound : MonoBehaviour
{
    private AudioSource audioSource;
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    void OnEnable()
    {
        // Riproduci il suono ogni volta che il feedback appare
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}
