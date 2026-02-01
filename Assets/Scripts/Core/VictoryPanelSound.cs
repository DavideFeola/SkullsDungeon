using UnityEngine;

public class VictoryPanelSound : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}
