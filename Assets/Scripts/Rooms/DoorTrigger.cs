using UnityEngine;
using Unity.Cinemachine;

public class DoorTrigger : MonoBehaviour
{
    [Header("Cameras")]
    public CinemachineCamera shopCamera;
    public CinemachineCamera combatCamera;

    [Header("Door Sprites")]
    public Sprite doorOpenSprite;
    public Sprite doorClosedSprite;

    [Header("Colliders")]
    public BoxCollider2D triggerCollider;
    public BoxCollider2D solidCollider;

    private SpriteRenderer spriteRenderer;
    private bool cameraChanged = false;
    private bool playerHasCrossed = false;
    private Vector3 playerLastPosition;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer && doorOpenSprite)
            spriteRenderer.sprite = doorOpenSprite;

        if (triggerCollider)
        {
            triggerCollider.isTrigger = true;
            triggerCollider.enabled = true;
        }

        if (solidCollider)
        {
            solidCollider.isTrigger = false;
            solidCollider.enabled = false;
        }

        gameObject.tag = "Door";
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (cameraChanged) return;
        if (!other.CompareTag("Player")) return;

        cameraChanged = true;
        playerLastPosition = other.transform.position;

        if (shopCamera && combatCamera)
        {
            shopCamera.Priority = 5;
            combatCamera.Priority = 10;
        }

        // 🎵 CAMBIA MUSICA A COMBAT
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCombatMusic();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (playerHasCrossed) return;
        if (!other.CompareTag("Player")) return;

        Vector3 playerCurrentPosition = other.transform.position;
        float deltaY = playerCurrentPosition.y - playerLastPosition.y;

        if (deltaY > 0)
        {
            Debug.Log("⚠️ Player tornato indietro (verso l'alto). Porta resta aperta.");
            
            // 🎵 SE TORNA INDIETRO, RIPRISTINA MUSICA SHOP
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayShopMusic();
            }
            
            return;
        }

        playerHasCrossed = true;

        CloseDoor();
        SpawnEnemiesAndActivateMinimap();
    }

    void CloseDoor()
    {
        if (spriteRenderer && doorClosedSprite)
            spriteRenderer.sprite = doorClosedSprite;

        if (solidCollider) 
            solidCollider.enabled = true;

        if (triggerCollider) 
            triggerCollider.enabled = false;

    }

    void SpawnEnemiesAndActivateMinimap()
    {
        DungeonGenerator dg = FindFirstObjectByType<DungeonGenerator>();
        if (dg != null)
        {
            dg.SpawnEnemiesInCombatRoom();
        }
        else
        {
            Debug.LogError("❌ [DoorTrigger] DungeonGenerator NON trovato!");
        }
    }
}