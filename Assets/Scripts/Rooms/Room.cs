using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Spawns & Doors")]
    public Transform[] enemySpawnPoints; // Gli spawn points per i nemici
    
    [Header("Doors")]
    // Lascia vuoto per ora
    
    [Header("Safety")]
    public float minDistanceFromPlayer = 2f;
    
    [Header("Enemy Settings")]
    public GameObject enemyPrefab; // Trascina il prefab del nemico
    public int enemiesToSpawn = 5; // Quanti nemici far spawnare
    
    private bool hasSpawned = false;
    
    void Start()
    {
        // NON spawnare automaticamente
        // Aspettiamo la chiamata da DoorTrigger
    }
    
    // Metodo chiamato dalla porta
    public void StartSpawning()
    {
        if (hasSpawned) return; // Spawn solo una volta
        
        hasSpawned = true;
        
        SpawnEnemies();
    }
    
    void SpawnEnemies()
    {
        if (enemyPrefab == null || enemySpawnPoints.Length == 0)
        {
            Debug.LogWarning("Manca il prefab nemico o gli spawn points!");
            return;
        }
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            // Scegli uno spawn point casuale
            Transform spawnPoint = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Length)];
            
            // Controlla distanza dal player
            if (player != null)
            {
                float distance = Vector2.Distance(spawnPoint.position, player.transform.position);
                if (distance < minDistanceFromPlayer)
                {
                    continue; // Troppo vicino, salta questo spawn
                }
            }
            
            // Spawna il nemico
            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity, transform);
        }
    }
}