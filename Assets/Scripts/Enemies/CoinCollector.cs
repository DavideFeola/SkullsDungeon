using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro; // ✅ AGGIUNTO per TextMesh Pro

public class CoinCollector : MonoBehaviour
{
    public int totalCoins = 10;

    [Header("UI")]
    public TextMeshProUGUI coinText; // ✅ MODIFICATO: Text → TextMeshProUGUI
    public GameObject coinUIContainer; // Container dell'UI monete (da nascondere/mostrare)

    [Header("Audio")]
    public AudioClip coinSound;
    private AudioSource audioSource;

    public static CoinCollector Instance;

    // HashSet per tracciare le monete già raccolte (evita doppie raccolte)
    private HashSet<int> collectedCoins = new HashSet<int>();

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
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        FindCoinText();
        UpdateCoinUI();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UnfreezePlayer();

        FindCoinText();
        UpdateCoinUI();
    }

    void UnfreezePlayer()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }

        Weapon weapon = GetComponentInChildren<Weapon>();
        if (weapon != null)
        {
            weapon.ResetAimDirection();
        }

        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.RefreshCamera();
        }

        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            string scriptName = script.GetType().Name;

            if (scriptName.Contains("Player") &&
                (scriptName.Contains("Move") || scriptName.Contains("Controller")))
            {
                script.enabled = true;
            }
        }
    }

    void FindCoinText()
    {
        coinText = null;
        coinUIContainer = null;

        GameObject coinTextObj = GameObject.FindGameObjectWithTag("CoinUI");
        if (coinTextObj != null)
        {
            coinText = coinTextObj.GetComponent<TextMeshProUGUI>(); // ✅ MODIFICATO: Text → TextMeshProUGUI

            coinUIContainer = coinTextObj.transform.parent != null
                ? coinTextObj.transform.parent.gameObject
                : coinTextObj;

        }
        else
        {
            TextMeshProUGUI[] allTexts = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None); // ✅ MODIFICATO: Text → TextMeshProUGUI
            foreach (TextMeshProUGUI t in allTexts) // ✅ MODIFICATO: Text → TextMeshProUGUI
            {
                if (t.gameObject.name.Contains("Coin"))
                {
                    coinText = t;

                    coinUIContainer = t.transform.parent != null
                        ? t.transform.parent.gameObject
                        : t.gameObject;

                    break;
                }
            }
        }

        if (coinText == null)
        {
            Debug.LogWarning("[CoinCollector] CoinText non trovato nella scena!");
        }

        HideCoinUI();
    }

    public void ShowCoinUI()
    {
        if (coinUIContainer != null)
        {
            coinUIContainer.SetActive(true);
        }
        else if (coinText != null)
        {
            coinText.gameObject.SetActive(true);
        }
    }

    public void HideCoinUI()
    {
        if (coinUIContainer != null)
        {
            coinUIContainer.SetActive(false);        }
        else if (coinText != null)
        {
            coinText.gameObject.SetActive(false);
        }
    }

    public void CollectCoin(int amount)
    {
        totalCoins += amount;
        UpdateCoinUI();

        if (audioSource != null && coinSound != null)
        {
            audioSource.PlayOneShot(coinSound);
        }

    }

    public void CollectCoinSafe(GameObject coinObject, int amount)
    {
        if (coinObject == null) return;

        int coinID = coinObject.GetInstanceID();

        if (collectedCoins.Contains(coinID))
        {
            Debug.LogWarning("[CoinCollector] DUPLICATO RILEVATO! Moneta " + coinID + " già raccolta!");
            return;
        }

        collectedCoins.Add(coinID);

        totalCoins += amount;
        UpdateCoinUI();

        if (audioSource != null && coinSound != null)
        {
            audioSource.PlayOneShot(coinSound);
        }

    }

    public int GetCoins()
    {
        return totalCoins;
    }

    public void SpendCoins(int amount)
    {
        if (totalCoins >= amount)
        {
            totalCoins -= amount;
            UpdateCoinUI();
            Debug.Log("Spese " + amount + " monete. Rimanenti: " + totalCoins);
        }
        else
        {
            Debug.LogWarning("Monete insufficienti! Hai: " + totalCoins + ", servono: " + amount);
        }
    }

    void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = totalCoins.ToString();
        }
        else
        {
            Debug.LogWarning("[CoinCollector] CoinText è null, non posso aggiornare UI");
        }
    }
}
