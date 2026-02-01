using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music Tracks")]
    public AudioClip shopMusic;
    public AudioClip combatMusic;
    public AudioClip gameOverMusic;

    [Header("Audio Sources")]
    public AudioSource musicSource;

    [Header("Fade Settings")]
    [Range(0.1f, 3f)]
    public float fadeDuration = 1f;

    [Header("Volume Settings")] // 🔑 NUOVO
    [Range(0f, 1f)]
    public float shopMusicVolume = 0.3f; // 🔑 Volume hotel_2 (30%)
    [Range(0f, 1f)]
    public float combatMusicVolume = 0.3f; // 🔑 Volume opening_theme (30%)
    [Range(0f, 1f)]
    public float gameOverMusicVolume = 1f; // 🔑 Volume game_over (100%)

    private Coroutine fadeCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.ignoreListenerPause = true;
            musicSource.volume = 1f;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayShopMusic();
    }

    void Start()
    {
        PlayShopMusic();
    }

    public void PlayShopMusic()
    {
        musicSource.loop = true;
        SwitchMusic(shopMusic, shopMusicVolume); // 🔑 MODIFICATO
    }

    public void PlayCombatMusic()
    {
        musicSource.loop = true;
        SwitchMusic(combatMusic, combatMusicVolume); // 🔑 MODIFICATO
    }

    public void PlayGameOverMusic()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        musicSource.Stop();
        musicSource.loop = false;
        musicSource.clip = gameOverMusic;
        musicSource.volume = gameOverMusicVolume; // 🔑 MODIFICATO
        musicSource.Play();
    }

    public void StopMusicImmediately(bool clearClip = false)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        musicSource.Stop();
        musicSource.volume = 1f;

        if (clearClip) musicSource.clip = null;
    }

    public void ResetToShopMusic()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        musicSource.Stop();
        musicSource.loop = true;
        musicSource.clip = shopMusic;
        musicSource.volume = shopMusicVolume; // 🔑 MODIFICATO
        musicSource.Play();
    }

    public void OnPlayerRespawn()
    {
        ResetToShopMusic();
    }

    void SwitchMusic(AudioClip newClip, float targetVolume) // 🔑 MODIFICATO - aggiunto parametro
    {
        if (newClip == null)
        {
            Debug.LogWarning("⚠️ AudioClip è null!");
            return;
        }

        if (musicSource.clip == newClip && musicSource.isPlaying)
            return;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        fadeCoroutine = StartCoroutine(FadeToNewTrack(newClip, targetVolume)); // 🔑 MODIFICATO
    }

    IEnumerator FadeToNewTrack(AudioClip newClip, float targetVolume) // 🔑 MODIFICATO - aggiunto parametro
    {
        float startVolume = musicSource.volume;

        while (musicSource.volume > 0f)
        {
            musicSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        musicSource.Stop();
        musicSource.loop = true;
        musicSource.clip = newClip;
        musicSource.Play();

        while (musicSource.volume < targetVolume) // 🔑 MODIFICATO
        {
            musicSource.volume += targetVolume * Time.deltaTime / fadeDuration; // 🔑 MODIFICATO
            yield return null;
        }

        musicSource.volume = targetVolume; // 🔑 MODIFICATO
        fadeCoroutine = null;
    }

    public void StopMusic()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        fadeCoroutine = StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float startVolume = musicSource.volume;

        while (musicSource.volume > 0f)
        {
            musicSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume;
        fadeCoroutine = null;
    }
}
