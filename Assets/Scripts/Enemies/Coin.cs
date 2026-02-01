using UnityEngine;
public class Coin : MonoBehaviour
{
    public int value = 1; // Valore della moneta
    public float attractSpeed = 5f; // Velocità di attrazione verso il player
    public float attractRange = 3f; // Distanza a cui inizia ad essere attratta
    private Transform player;
    private Rigidbody2D rb;
    private bool isBeingAttracted = false;
    private bool isCollected = false; // Flag per evitare doppie raccolte
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Trova il player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }
    void Update()
    {
        if (player == null) return;
        float distance = Vector2.Distance(transform.position, player.position);
        // Se il player è vicino, attira la moneta
        if (distance <= attractRange)
        {
            isBeingAttracted = true;
        }
        if (isBeingAttracted)
        {
            // Muovi verso il player
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * attractSpeed;
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        // Evita doppie raccolte
        if (isCollected) return;
        if (!gameObject.activeInHierarchy) return; // Doppio controllo

        if (other.CompareTag("Player"))
        {
            isCollected = true; // Marca come raccolta immediatamente

            // Disabilita IMMEDIATAMENTE il gameObject per bloccare qualsiasi altro trigger
            gameObject.SetActive(false);

            // Raccogli la moneta usando il metodo sicuro
            var coinManager = other.GetComponent<CoinCollector>();
            if (coinManager != null)
            {
                coinManager.CollectCoinSafe(gameObject, value);
            }

            // Distruggi la moneta
            Destroy(gameObject, 0.1f); // Piccolo delay per permettere al suono di partire
        }
    }
    // Visualizza il range di attrazione
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attractRange);
    }
}