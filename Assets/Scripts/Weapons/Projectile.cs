using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    Rigidbody2D rb;
    float damage;
    int pierce;
    int hitCount;
    bool isEnemyProjectile;  // Flag per distinguere proiettili nemici

    public void Init(Vector2 dir, float dmg, float speed, float lifeTime, int pierce, bool isFromEnemy = false)
    {
        rb = GetComponent<Rigidbody2D>();

        damage = dmg;
        this.pierce = pierce;
        this.isEnemyProjectile = isFromEnemy;

        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.linearVelocity = dir.normalized * speed;

        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Player
        if (col.CompareTag("Player"))
        {
            if (isEnemyProjectile)
            {
                var playerHealth = col.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    Destroy(gameObject);
                }
                return;
            }
            else
            {
                return;
            }
        }

        // Porta
        if (col.CompareTag("Door") && col.isTrigger)
        {
            Destroy(gameObject);
            return;
        }

        // Altri trigger
        if (col.isTrigger)
        {
            return;
        }

        // Nemici (solo proiettili del player)
        if (!isEnemyProjectile)
        {
            var e = col.GetComponent<Enemy>();
            if (e != null)
            {
                e.TakeDamage(damage);
                hitCount++;
                if (hitCount > pierce)
                    Destroy(gameObject);
                return;
            }

            var sniper = col.GetComponent<EnemySniper>();
            if (sniper != null)
            {
                sniper.TakeDamage(damage);
                hitCount++;
                if (hitCount > pierce)
                    Destroy(gameObject);
                return;
            }
        }

        // Qualsiasi altro collider solido
        Destroy(gameObject);
    }
}
