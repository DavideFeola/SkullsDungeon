using UnityEngine;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager I { get; private set; }

    [Header("Audio")]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip closeClip;

    void Awake()
    {
        // Singleton vero (a prova di scene)
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);

        // Safety
        if (!source)
            source = GetComponent<AudioSource>();
    }

    public void PlayClose()
    {
        if (source && closeClip)
            source.PlayOneShot(closeClip);
    }
}
