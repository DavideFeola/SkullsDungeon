using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DoorExitWin : MonoBehaviour
{
    [Header("Door Sprites")]
    public Sprite doorClosedSprite;
    public Sprite doorOpenSprite;

    [Header("Door Settings")]
    public string nextSceneName = "";   // se vuoto -> vittoria
    public int nextRoomNumber = 4;      // >=4 -> vittoria
    public Color lockedColor = Color.red;
    public Color unlockedColor = Color.white;

    [Header("UI")]
    public Image enemyCountImage;
    public TextMeshProUGUI enemyCountNumberText;
    public TextMeshProUGUI openDoorText;

    [Header("Visual Feedback")]
    public bool showEnemyCount = true;

    [Header("Audio")] // ⬅️ NUOVO
    public AudioClip doorUnlockSound; // ⬅️ NUOVO: Trascina qui il suono della porta che si apre
    private AudioSource audioSource; // ⬅️ NUOVO

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D solidCollider;
    private bool isUnlocked = false;
    private bool canCheckEnemies = false;
    private int totalEnemiesAtStart = -1;

    void Awake()
    {
        // ✅ più robusto (se lo SpriteRenderer è su un child)
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>(); // ⬅️ NUOVO

        // ⬅️ NUOVO: Se non c'è AudioSource, aggiungilo automaticamente
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        foreach (BoxCollider2D col in colliders)
        {
            if (!col.isTrigger)
            {
                solidCollider = col;
                break;
            }
        }

        if (spriteRenderer != null && doorClosedSprite != null)
        {
            spriteRenderer.sprite = doorClosedSprite;
            spriteRenderer.color = lockedColor;
        }

        HideEnemyCounter();

        if (openDoorText != null)
            openDoorText.gameObject.SetActive(false);
    }

    // ⚠️ NIENTE autoActivate qui.
    // Deve essere chiamato quando la combat è pronta (come nelle altre stanze).
    public void ActivateDoor()
    {
        canCheckEnemies = true;
        ShowEnemyCounter();
    }

    void Update()
    {
        if (!isUnlocked && canCheckEnemies)
        {
            CheckEnemies();
        }
    }

    void CheckEnemies()
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        int regularEnemies = 0;

        foreach (GameObject enemy in allEnemies)
        {
            if (enemy != null && enemy.activeInHierarchy && enemy.GetComponent<EnemySniper>() == null)
            {
                regularEnemies++;
            }
        }

        EnemySniper[] snipersArr = FindObjectsByType<EnemySniper>(FindObjectsSortMode.None);
        int snipers = 0;
        foreach (var s in snipersArr)
        {
            if (s != null && s.gameObject.activeInHierarchy)
                snipers++;
        }

        int enemiesLeft = regularEnemies + snipers;

        if (totalEnemiesAtStart == -1)
        {
            totalEnemiesAtStart = enemiesLeft;
        }

        UpdateEnemyCountUI(enemiesLeft);

        if (enemiesLeft == 0 && !isUnlocked)
        {
            UnlockDoor();
        }
    }

    void UpdateEnemyCountUI(int count)
    {
        if (!showEnemyCount) return;

        if (enemyCountNumberText != null)
        {
            enemyCountNumberText.text = count.ToString();

            Color targetColor;

            if (count == 0 || count == 1)
            {
                targetColor = Color.green;
            }
            else if (totalEnemiesAtStart > 0)
            {
                float percentage = (float)count / totalEnemiesAtStart;

                if (percentage <= 0.5f)
                    targetColor = new Color(1f, 0.8f, 0f);
                else
                    targetColor = Color.red;
            }
            else
            {
                targetColor = Color.red;
            }

            enemyCountNumberText.color = targetColor;
            if (enemyCountImage != null)
                enemyCountImage.color = targetColor;
        }
    }

    void ShowEnemyCounter()
    {
        if (!showEnemyCount) return;

        if (enemyCountImage != null)
            enemyCountImage.gameObject.SetActive(true);

        if (enemyCountNumberText != null)
            enemyCountNumberText.gameObject.SetActive(true);
    }

    void HideEnemyCounter()
    {
        if (enemyCountImage != null)
            enemyCountImage.gameObject.SetActive(false);

        if (enemyCountNumberText != null)
            enemyCountNumberText.gameObject.SetActive(false);
    }

    void HideOpenDoorText()
    {
        if (openDoorText != null)
            openDoorText.gameObject.SetActive(false);
    }

    void UnlockDoor()
    {
        isUnlocked = true;


        if (AudioManager.Instance != null)
            AudioManager.Instance.StopMusicImmediately(clearClip: true);

        // ⬅️ NUOVO: Riproduci il suono della porta che si apre
        if (doorUnlockSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(doorUnlockSound);
        }

        if (spriteRenderer != null && doorOpenSprite != null)
        {
            spriteRenderer.sprite = doorOpenSprite;
            spriteRenderer.color = unlockedColor;
        }

        if (solidCollider != null)
            solidCollider.enabled = false;

        HideEnemyCounter();

        if (openDoorText != null)
        {
            openDoorText.gameObject.SetActive(true);
            openDoorText.text = "Door unlocked!";
        }

        Invoke(nameof(HideOpenDoorText), 10f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (!isUnlocked)
        {
            return;
        }

        FreezePlayer(other.gameObject);

        if (AudioManager.Instance != null)
            AudioManager.Instance.StopMusicImmediately(clearClip: true);

        LoadNextSceneOrWin();
    }

    void FreezePlayer(GameObject player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            string scriptName = script.GetType().Name;
            if (scriptName.Contains("Player") && scriptName.Contains("Move"))
            {
                script.enabled = false;
            }
        }
    }

    void LoadNextSceneOrWin()
    {
        // ✅ se è la porta finale, mostra vittoria
        if (string.IsNullOrEmpty(nextSceneName) || nextRoomNumber >= 4)
        {

            VictoryManager victoryManager = FindFirstObjectByType<VictoryManager>();
            if (victoryManager != null) victoryManager.MostraVittoria();
            else Debug.LogError("VictoryManager non trovato nella scena!");

            return;
        }

        // altrimenti vai alla scena dopo
        if (RoomTransition.Instance != null)
            RoomTransition.Instance.TransitionToRoom(nextRoomNumber, nextSceneName);
        else
            SceneManager.LoadScene(nextSceneName);
    }
}