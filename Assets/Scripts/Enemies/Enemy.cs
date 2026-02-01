using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    public EnemyData data;
    [Header("Health")]
    public float maxHP = 100f;
    public float currentHP;
    
    [Header("Audio")]
    public AudioClip deathSound;
    private AudioSource audioSource;
    
    [Header("Coin Drop")]
    public GameObject coinPrefab; 
    public int minCoins = 1;
    public int maxCoins = 3;
    public float dropForce = 2f;
    
    Transform target;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    MeleeEnemyAnimator enemyAnimator;
    
    private bool isDead = false;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyAnimator = GetComponent<MeleeEnemyAnimator>();
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        if (data != null)
            maxHP = data.maxHP;
        currentHP = maxHP;
        
        var go = GameObject.FindGameObjectWithTag("Player");
        if (go != null) target = go.transform;
        else Debug.LogWarning("[Enemy] Player NON trovato! Assicurati che il Player abbia il tag 'Player'.");
    }
    
    void FixedUpdate()
    {
        if (isDead)
            return;
            
        if (target == null || data == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        
        Vector2 toPlayer = (target.position - transform.position);
        Vector2 dir = toPlayer.normalized;
        rb.linearVelocity = dir * data.moveSpeed;
        
        if (enemyAnimator != null)
            enemyAnimator.UpdateAnimation(dir);
    }
    
    public void TakeDamage(float dmg)
    {
        if (isDead) return;
        
        currentHP -= dmg;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        
        if (currentHP <= 0)
            Die();
    }
    
    void Die()
    {
        if (isDead) return;
        isDead = true;
        
        
        rb.linearVelocity = Vector2.zero;
        
        var collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;
        
        if (deathSound != null && audioSource != null)
            audioSource.PlayOneShot(deathSound);
        
        DropCoins();
        
        Destroy(gameObject, 0.15f);
    }
    
    void DropCoins()
    {
        if (coinPrefab == null) return;
        
        int coinsToDrop = Random.Range(minCoins, maxCoins + 1);
        for (int i = 0; i < coinsToDrop; i++)
        {
            GameObject coin = Instantiate(coinPrefab, transform.position, Quaternion.identity);
            Rigidbody2D coinRb = coin.GetComponent<Rigidbody2D>();
            if (coinRb != null)
            {
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                coinRb.AddForce(randomDirection * dropForce, ForceMode2D.Impulse);
            }
        }
    }
}
