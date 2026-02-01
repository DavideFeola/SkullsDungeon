using UnityEngine;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    public static AmmoUI Instance;
    
    [Header("UI References")]
    public TextMeshProUGUI ammoText;
    public GameObject ammoPanel; // Opzionale: panel per nascondere quando non serve
    
    [Header("Settings")]
    public bool showOnlyInCombat = true; // Se true, mostra solo nelle combat room
    
    private Weapon playerWeapon;
    
    void Awake()
    {
        // Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    void Start()
    {
        // Trova l'arma del player
        FindPlayerWeapon();
        
        // Nascondi all'inizio se necessario
        if (showOnlyInCombat && ammoPanel != null)
            ammoPanel.SetActive(false);
    }
    
    void Update()
    {
        // Aggiorna il contatore ogni frame
        UpdateAmmoDisplay();
        
        // Mostra/nascondi in base alla combat room
        if (showOnlyInCombat && ammoPanel != null)
        {
            ammoPanel.SetActive(DungeonGenerator.IsInCombatRoom);
        }
    }
    
    void FindPlayerWeapon()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerWeapon = player.GetComponentInChildren<Weapon>();
            
            if (playerWeapon == null)
            {
                Debug.LogWarning("[AmmoUI] Weapon non trovato sul Player!");
            }
        }
        else
        {
            Debug.LogError("[AmmoUI] Player non trovato!");
        }
    }
    
    void UpdateAmmoDisplay()
    {
        if (playerWeapon == null)
        {
            FindPlayerWeapon(); // Riprova a trovarlo
            return;
        }
        
        if (ammoText != null)
        {
            int current = playerWeapon.GetCurrentAmmo();
            int max = playerWeapon.GetMaxAmmo();
            
            ammoText.text = $"{current}/{max}";
            
            // ✅ OPZIONALE: Cambia colore se munizioni basse
            if (current <= 10)
                ammoText.color = Color.red;
            else if (current <= 25)
                ammoText.color = Color.yellow;
            else
                ammoText.color = Color.white;
        }
    }
    
    // Metodo pubblico per forzare l'update (chiamalo dopo acquisto shop)
    public void ForceUpdate()
    {
        UpdateAmmoDisplay();
    }
}