using UnityEngine;

public class PanelOpenSound : MonoBehaviour
{
    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        audioSource.Play();
    }
}
