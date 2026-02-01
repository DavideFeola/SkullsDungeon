using UnityEngine;
using UnityEngine.Events;
 
[RequireComponent(typeof(Collider2D))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHP = 5f;
    public float currentHP;
 
    [Header("Invulnerability")]
    public float invulnTime = 0.6f;
    float invulnUntil;
 
    [Header("Feedback")]
    public SpriteRenderer sprite;
    public Color hitFlashColor = new Color(1f, 0.3f, 0.3f, 1f);
    public float hitFlashDuration = 0.08f;
 
    [Header("Events")]
    public UnityEvent OnDeath;
    
    [Header("Game Over")]
    public GameOverManager gameOverManager;
    
    // 🔥 FLAG PER EVITARE MORTE MULTIPLA
    private bool isDead = false;
 
    Color _originalColor;
 
    void Awake()
    {
        ResetPlayer();
        
        if (!sprite) sprite = GetComponentInChildren<SpriteRenderer>();
        if (sprite) _originalColor = sprite.color;
    }
    
    // 🔄 METODO PUBBLICO PER RESETTARE COMPLETAMENTE IL PLAYER
    public void ResetPlayer()
    {
        currentHP = maxHP;
        isDead = false;
        invulnUntil = 0f;
        gameOverManager = null; // ⭐ AZZERA IL RIFERIMENTO!
    }
 
    public void TakeDamage(float dmg)
    {
        if (isDead) return;
        
        if (Time.time < invulnUntil) return;
        if (dmg <= 0f) return;
 
        currentHP = Mathf.Max(0f, currentHP - dmg);
        invulnUntil = Time.time + invulnTime;
 
        if (sprite) StartCoroutine(HitFlash());
 
        if (currentHP <= 0f && !isDead)
        {
            Die();
        }
    }
 
    System.Collections.IEnumerator HitFlash()
    {
        if (sprite)
        {
            sprite.color = hitFlashColor;
            yield return new WaitForSeconds(hitFlashDuration);
            sprite.color = _originalColor;
        }
    }
 
    public void Heal(float amount)
    {
        if (amount <= 0f) return;
        currentHP = Mathf.Min(maxHP, currentHP + amount);
    }
 
    void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        OnDeath?.Invoke();
        
        // 🎵 FERMA MUSICA E FAI PARTIRE GAME OVER
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusicImmediately();
            StartCoroutine(PlayGameOverMusicDelayed());
        }
        
        gameOverManager = FindAnyObjectByType<GameOverManager>();
        
        if (gameOverManager != null)
        {
            gameOverManager.MostraGameOver();
        }
    }
    
    System.Collections.IEnumerator PlayGameOverMusicDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameOverMusic();
        }
    }
}