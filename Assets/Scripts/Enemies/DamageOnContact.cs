using UnityEngine;
 
[RequireComponent(typeof(Collider2D))]

public class DamageOnContact : MonoBehaviour

{

    public float damage = 1f;

    public float hitCooldown = 0.5f;   // quanto tempo aspettare tra un colpo e l'altro

    float nextHitTime;
 
    void Reset()

    {

        // assicura trigger per “attraversare” il player

        var col = GetComponent<Collider2D>();

        if (col) col.isTrigger = true;

    }
 
    void OnTriggerEnter2D(Collider2D other)

    {

        TryHit(other);

    }
 
    void OnTriggerStay2D(Collider2D other)

    {

        TryHit(other);

    }
 
    void TryHit(Collider2D other)

    {

        if (Time.time < nextHitTime) return;
 
        var ph = other.GetComponent<PlayerHealth>();

        if (ph != null && other.gameObject.activeInHierarchy)

        {

            ph.TakeDamage(damage);

            nextHitTime = Time.time + hitCooldown;

        }

    }

}