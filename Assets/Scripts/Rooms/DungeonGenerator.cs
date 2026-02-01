using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    // ✅ NUOVO: Flag statico per sapere se siamo nella Combat Room
    public static bool IsInCombatRoom { get; private set; } = false;

    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject enemyPrefab;          // Nemico melee (corpo a corpo)
    public GameObject enemySniperPrefab;    // ✅ NUOVO: Nemico sniper (a distanza)
    public GameObject enemyRoom3Prefab;     // ✅ NUOVO: Nemico melee potenziato Stanza 3
    
    [Header("Player Spawn")]
    public Vector3 playerSpawnPosition = new Vector3(0f, 0f, 0f);
    
    [Header("Enemy Spawn")]
    public int numberOfEnemies = 3;
    public float minX = -8f;
    public float maxX = 8f;
    public float minY = -6f;
    public float maxY = 6f;
    
    [Header("Spawn Settings")]
    public bool spawnPlayerOnStart = true;
    public bool spawnEnemiesOnStart = false;
    
    [Header("Rooms (legacy)")]
    public int roomsCount = 5;
    public float spacing = 20f;
    
    private GameObject playerInstance;
    private bool enemiesSpawned = false;
    private MinimapCameraFollow minimapCamera;
    
    void Start()
    {
        // ✅ NUOVO: Resetta lo sparo all'inizio di ogni scena (importante per scene multiple!)
        IsInCombatRoom = false;

        // Trova la minimap camera
        minimapCamera = FindFirstObjectByType<MinimapCameraFollow>();
        
        // Controlla se Player esiste già
        if (CoinCollector.Instance != null)
        {
            // Riposiziona il Player esistente
            CoinCollector.Instance.transform.position = playerSpawnPosition;
            playerInstance = CoinCollector.Instance.gameObject;
            
            // Ricollega GameOverManager
            ConnectGameOverManager(playerInstance);
        }
        else if (spawnPlayerOnStart)
        {
            // Spawna nuovo Player solo se non esiste
            if (playerPrefab == null)
            {
                Debug.LogError("[DG] Player Prefab NON assegnato!");
            }
            else
            {
                SpawnPlayer();
            }
        }
        
        if (spawnEnemiesOnStart && enemyPrefab != null)
        {
            SpawnEnemies();
        }
    }
    
    void SpawnPlayer()
    {
        playerInstance = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
        playerInstance.name = "Player";
        
        ConnectGameOverManager(playerInstance);
    }
    
    // Collega GameOverManager
    void ConnectGameOverManager(GameObject player)
    {
        var ph = player.GetComponent<PlayerHealth>();
        var gom = FindFirstObjectByType<GameOverManager>();
        
        if (ph != null && gom != null)
        {
            ph.OnDeath.RemoveAllListeners();
            ph.OnDeath.AddListener(gom.MostraGameOver);
        }
        else
        {
            Debug.LogWarning("[DG] Impossibile collegare GameOverManager: " +
                             (ph == null ? "PlayerHealth mancante. " : "") +
                             (gom == null ? "GameOverManager non trovato." : ""));
        }
    }
    
    // Attiva la minimap quando spawni i nemici
    public void SpawnEnemiesInCombatRoom()
    {
        if (enemiesSpawned)
        {
            Debug.Log("[DG] Nemici già spawnati!");
            return;
        }
        
        if (enemyPrefab == null)
        {
            Debug.LogError("[DG] Enemy Prefab NON assegnato!");
            return;
        }
        
        enemiesSpawned = true;
        SpawnEnemies();

        // ✅ NUOVO: Abilita lo sparo nella Combat Room
        IsInCombatRoom = true;
        
        // Attiva la minimap nella combat room
        if (minimapCamera != null)
        {
            Vector3 combatRoomCenter = new Vector3(0f, 0f, 0f);
            minimapCamera.SetMinimapActive(true);
            minimapCamera.CenterOnRoom(combatRoomCenter);
        }
        else
        {
            Debug.LogWarning("[DG] MinimapCamera non trovata!");
        }

        // ✅ NUOVO: Mostra l'UI delle monete nella Combat Room
        if (CoinCollector.Instance != null)
        {
            CoinCollector.Instance.ShowCoinUI();
        }
    }
    
    void SpawnEnemies()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 playerPos = player != null ? player.transform.position : playerSpawnPosition;

        // ✅ NUOVO: Determina il tipo di nemici da spawnare in base alla scena
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        bool isStanza2 = sceneName.Contains("Stanza2") || sceneName.Contains("Room2");
        bool isStanza3 = sceneName.Contains("Stanza3") || sceneName.Contains("Room3");

        // ✅ NUOVO: Contatori per gli sniper e per il nemico potenziato
        int snipersSpawned = 0;
        int maxSnipers = 2; // Numero fisso di sniper (20% su 10 nemici)

        for (int i = 0; i < numberOfEnemies; i++)
        {
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);
            Vector3 pos = new Vector3(randomX, randomY, 0f);

            float distanceFromPlayer = Vector3.Distance(pos, playerPos);
            if (distanceFromPlayer < 3f)
            {
                i--;
                continue;
            }

            GameObject enemyToSpawn = null;

            // ✅ STANZA 2: 2 sniper + 8 melee normali
            if (isStanza2 && enemySniperPrefab != null)
            {
                if (snipersSpawned < maxSnipers)
                {
                    enemyToSpawn = enemySniperPrefab;
                    snipersSpawned++;
                }
                else
                {
                    enemyToSpawn = enemyPrefab; // Resto sono melee normali
                }
            }
            // ✅ STANZA 3: 4 sniper + 10 melee normali
else if (isStanza3 && enemySniperPrefab != null)
{
    int maxSnipersRoom3 = 4;

    if (snipersSpawned < maxSnipersRoom3)
    {
        // Primi 4 sono sniper
        enemyToSpawn = enemySniperPrefab;
        snipersSpawned++;
    }
    else
    {
        // Resto sono melee normali
        enemyToSpawn = enemyPrefab;
    }
}

            else
            {
                // Stanza 1 o altre: solo melee normali
                enemyToSpawn = enemyPrefab;
            }

            if (enemyToSpawn == null)
            {
                Debug.LogError($"[DG] Prefab nemico non assegnato! Scena: {sceneName}");
                continue;
            }

            GameObject enemy = Instantiate(enemyToSpawn, pos, Quaternion.identity);
            string enemyType = (enemyToSpawn == enemySniperPrefab) ? "Sniper" : "Melee";

            enemy.name = $"Enemy_{enemyType}_{i}";

        }

        // Attiva ExitDoor o DoorExitWin
        ExitDoor exitDoor = FindFirstObjectByType<ExitDoor>();
        if (exitDoor != null)
        {
            exitDoor.ActivateDoor();
        }

        // Controlla anche DoorExitWin (per la stanza finale)
        DoorExitWin doorExitWin = FindFirstObjectByType<DoorExitWin>();
        if (doorExitWin != null)
        {
            doorExitWin.ActivateDoor();
        }

        // ✅ NUOVO: Log migliorato con conteggio per ogni stanza
        if (isStanza2)
        {
            Debug.Log($"[DG] Stanza 2 - Spawnati {numberOfEnemies} nemici: Sniper: {snipersSpawned}, Melee: {numberOfEnemies - snipersSpawned}");
        }
        else if (isStanza3)
{
    int normalMelee = numberOfEnemies - snipersSpawned;
}

        else
        {
            Debug.Log($"[DG] Spawnati {numberOfEnemies} nemici nella Combat Room (Scena: {sceneName})!");
        }
    }
    
    // Metodo per disattivare la minimap quando esci dalla combat room
    public void ExitCombatRoom()
    {
        // ✅ NUOVO: Disabilita lo sparo fuori dalla Combat Room
        IsInCombatRoom = false;

        if (minimapCamera != null)
        {
            minimapCamera.SetMinimapActive(false);
        }

        // ✅ NUOVO: Nascondi l'UI delle monete fuori dalla Combat Room
        if (CoinCollector.Instance != null)
        {
            CoinCollector.Instance.HideCoinUI();
        }
    }
}