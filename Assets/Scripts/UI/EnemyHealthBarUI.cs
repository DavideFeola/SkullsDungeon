using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarUI : MonoBehaviour
{
    [Header("References")]
    public Image fillImage;          // Trascina la Image del Fill

    [Header("Auto-Detected (non toccare)")]
    [HideInInspector] public Enemy enemy;              // Auto-rilevato dal parent
    [HideInInspector] public EnemySniper enemySniper;  // Auto-rilevato dal parent
    
    [Header("Colors")]
    public Color fullColor = Color.green;
    public Color midColor = Color.yellow;
    public Color lowColor = Color.red;
    
    void Awake()
    {

        if (fillImage != null)
        {

            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;

            // Imposta colore iniziale verde (vita piena)
            fillImage.color = fullColor;
            fillImage.fillAmount = 1f;

        }
        else
        {
            Debug.LogError("[EnemyHealthBarUI] ❌ fillImage è NULL su " + gameObject.name + "! Assegna la Fill Image nell'Inspector.");
        }
    }
    
    void Start()
    {

        // Se non hai assegnato enemy, prova a trovarlo nel parent
        if (enemy == null && enemySniper == null)
        {
            enemy = GetComponentInParent<Enemy>();

            // ✅ NUOVO: Cerca anche EnemySniper
            if (enemy == null)
            {
                enemySniper = GetComponentInParent<EnemySniper>();
            }
        }

        // Debug per verificare configurazione
        if (enemy == null && enemySniper == null)
        {
            Debug.LogError("[EnemyHealthBarUI] ❌ Né Enemy né EnemySniper trovati nel parent di " + gameObject.name);
            Debug.LogError("[EnemyHealthBarUI] ❌ Parent hierarchy: " + GetParentHierarchy());
        }
        else if (enemySniper != null)
        {
            Debug.Log("[EnemyHealthBarUI] ✅ Sniper rilevato: " + enemySniper.gameObject.name + " | HP: " + enemySniper.currentHP + "/" + enemySniper.maxHP);
        }
        else if (enemy != null)
        {
            Debug.Log("[EnemyHealthBarUI] ✅ Enemy rilevato: " + enemy.gameObject.name + " | HP: " + enemy.currentHP + "/" + enemy.maxHP);
        }

        // Verifica il colore della fillImage dopo lo Start
        if (fillImage != null)
        {
            Debug.Log("[EnemyHealthBarUI] Colore fillImage dopo Start: " + fillImage.color + " | enabled: " + fillImage.enabled + " | gameObject.activeSelf: " + fillImage.gameObject.activeSelf);
        }
    }

    string GetParentHierarchy()
    {
        string hierarchy = gameObject.name;
        Transform current = transform.parent;
        while (current != null)
        {
            hierarchy = current.name + " > " + hierarchy;
            current = current.parent;
        }
        return hierarchy;
    }
    
    bool hasLoggedColors = false;

    void LateUpdate()
    {
        if (!fillImage) return;

        // ✅ NUOVO: Supporta sia Enemy che EnemySniper
        float maxHP = 0f;
        float currentHP = 0f;

        if (enemy != null)
        {
            maxHP = enemy.maxHP;
            currentHP = enemy.currentHP;
        }
        else if (enemySniper != null)
        {
            maxHP = enemySniper.maxHP;
            currentHP = enemySniper.currentHP;

            // Debug: log colori e HP solo una volta per sniper
            if (!hasLoggedColors)
            {
                Debug.Log("[EnemyHealthBarUI] 🎨 COLORI SNIPER - fullColor: " + fullColor + " | midColor: " + midColor + " | lowColor: " + lowColor);
                Debug.Log("[EnemyHealthBarUI] 🎨 HP SNIPER - currentHP: " + currentHP + " | maxHP: " + maxHP);
                hasLoggedColors = true;
            }
        }
        else
        {
            return; // Nessun nemico trovato
        }

        // Calcola percentuale vita (0..1)
        float max = Mathf.Max(0.0001f, maxHP);
        float t = Mathf.Clamp01(currentHP / max);

        // Aggiorna lunghezza
        fillImage.fillAmount = t;

        // Aggiorna colore (verde→giallo→rosso)
        Color color;
        if (t > 0.5f)
        {
            float k = (t - 0.5f) * 2f;
            color = Color.Lerp(midColor, fullColor, k);
        }
        else
        {
            float k = t * 2f;
            color = Color.Lerp(lowColor, midColor, k);
        }
        color.a = 1f;
        fillImage.color = color;

        // Debug: log colore finale e calcolo t solo una volta
        if (enemySniper != null && !hasLoggedColors)
        {
            Debug.Log("[EnemyHealthBarUI] 🎨 Percentuale vita (t): " + t + " | Colore finale: " + color);
        }
    }
}

