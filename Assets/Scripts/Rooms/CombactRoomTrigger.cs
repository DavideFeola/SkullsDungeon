using UnityEngine;

public class CombatRoomTrigger : MonoBehaviour
{
    [Header("References")]
    public DungeonGenerator dungeonGenerator; // riferimento al tuo DungeonGenerator

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;

        if (dungeonGenerator != null)
        {
            dungeonGenerator.SpawnEnemiesInCombatRoom();
        }
        else
        {
            Debug.LogWarning("⚠️ DungeonGenerator non assegnato!");
        }
    }
}

