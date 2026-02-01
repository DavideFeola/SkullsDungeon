using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemySniper : MonoBehaviour
{
    public EnemyData data;

    [Header("Health")]
    public float maxHP = 100f;
    public float currentHP;

    [Header("Sniper Behavior")]
    public float attackRange = 10f;
    public float minDistance = 7f;
    public float fireRate = 1f;
    public float projectileSpeed = 8f;
    public float projectileDamage = 10f;
    public Projectile projectilePrefab;
    public Transform firePoint;

    [Header("Audio")]
    public AudioClip shootSound;
    public AudioClip deathSound;
    private AudioSource audioSource;

    [Header("Obstacle Avoidance (2D)")]
    public float detectionDistance = 3f;
    public float rayOriginOffset = 0.2f;
    public float sideAngle = 45f;
    public float avoidanceStrength = 3f;
    public float steerSmoothing = 10f;
    public LayerMask obstacleLayer;

    [Header("Line of Sight – Cerca finché la ottiene")]
    public LayerMask losBlockLayer;
    public float losExtraPadding = 0.1f;

    [Header("LOS Search Tuning")]
    public float strafeStrength = 1.2f;
    public float approachWhileSearching = 0.35f;
    public float rechooseSideAfter = 2.0f;

    [Header("Coin Drop")]
    public GameObject coinPrefab;
    public int minCoins = 2;
    public int maxCoins = 5;
    public float dropForce = 2f;

    Transform target;
    Rigidbody2D rb;
    float nextFireTime;

    Vector2 smoothedDir = Vector2.right;

    int sideSign = 1;
    bool hadLOSLastFrame = true;
    float noLOSTimer = 0f;
    
    private bool isDead = false;
    private SniperAnimator sniperAnimator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sniperAnimator = GetComponent<SniperAnimator>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        if (data != null) maxHP = data.maxHP;
        currentHP = maxHP;

        var go = GameObject.FindGameObjectWithTag("Player");
        if (go != null) target = go.transform;

        if (losBlockLayer.value == 0) losBlockLayer = obstacleLayer;
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

        Vector2 toPlayer = (Vector2)target.position - (Vector2)transform.position;
        float dist = toPlayer.magnitude;
        Vector2 toPlayerDir = dist > 0.001f ? toPlayer / dist : smoothedDir;

        bool hasLOS = HasLineOfSight(toPlayerDir, dist);

        if (hasLOS) noLOSTimer = 0f;
        else noLOSTimer += Time.fixedDeltaTime;

        if (!hasLOS && hadLOSLastFrame)
        {
            sideSign = ChooseSideByFreeSpace(toPlayerDir);
        }

        Vector2 desired = Vector2.zero;

        if (hasLOS)
        {
            if (dist < minDistance) desired = -toPlayerDir;
            else if (dist > attackRange) desired = toPlayerDir;
            else desired = Vector2.zero;
        }
        else
        {
            Vector2 strafe = new Vector2(-toPlayerDir.y, toPlayerDir.x) * sideSign;
            Vector2 approach = toPlayerDir * approachWhileSearching;
            desired = (strafe * strafeStrength + approach).normalized;

            if (noLOSTimer >= rechooseSideAfter)
            {
                sideSign *= -1;
                noLOSTimer = 0f;
            }
        }

        Vector2 avoidance = desired != Vector2.zero ? ComputeAvoidance(desired) : Vector2.zero;
        Vector2 final = desired == Vector2.zero ? Vector2.zero : (desired + avoidance).normalized;

        if (final != Vector2.zero)
        {
            smoothedDir = Vector2.Lerp(smoothedDir, final, steerSmoothing * Time.fixedDeltaTime).normalized;
            rb.linearVelocity = smoothedDir * data.moveSpeed;
            
            if (sniperAnimator != null)
                sniperAnimator.UpdateAnimation(smoothedDir);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            
            if (sniperAnimator != null)
                sniperAnimator.UpdateAnimation(toPlayerDir);
        }

        hadLOSLastFrame = hasLOS;
    } // 🔑 QUESTA PARENTESI ERA MANCANTE!

    void Update()
    {
        if (target == null || projectilePrefab == null) return;

        FlipFirePoint();

        Vector2 toPlayer = (Vector2)target.position - (Vector2)firePoint.position;
        float dist = toPlayer.magnitude;
        if (dist <= 0.001f) return;

        if (dist <= attackRange && HasLineOfSight(toPlayer / dist, dist) && Time.time >= nextFireTime)
        {
            FireAtPlayer();
            nextFireTime = Time.time + (1f / fireRate);
        }
    }

    bool HasLineOfSight(Vector2 dir, float dist)
    {
        Vector2 origin = firePoint != null ? (Vector2)firePoint.position : (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origin + dir * losExtraPadding, dir, dist, losBlockLayer);
        return hit.collider == null;
    }

    int ChooseSideByFreeSpace(Vector2 toPlayerDir)
    {
        Vector2 left = new Vector2(-toPlayerDir.y, toPlayerDir.x);
        Vector2 right = -left;
        Vector2 origin = (Vector2)transform.position;

        bool blockedLeft = Physics2D.Raycast(origin, left, detectionDistance * 1.5f, obstacleLayer);
        bool blockedRight = Physics2D.Raycast(origin, right, detectionDistance * 1.5f, obstacleLayer);

        if (blockedLeft && !blockedRight) return -1;
        if (blockedRight && !blockedLeft) return 1;
        return sideSign;
    }

    Vector2 ComputeAvoidance(Vector2 dir)
    {
        Vector2 origin = (Vector2)transform.position + dir * rayOriginOffset;

        Vector2 left = Rotate(dir, sideAngle);
        Vector2 right = Rotate(dir, -sideAngle);

        RaycastHit2D c = Physics2D.Raycast(origin, dir, detectionDistance, obstacleLayer);
        RaycastHit2D l = Physics2D.Raycast(origin, left, detectionDistance, obstacleLayer);
        RaycastHit2D r = Physics2D.Raycast(origin, right, detectionDistance, obstacleLayer);

        Vector2 avoid = Vector2.zero;

        if (c.collider != null) avoid += (Vector2)c.normal * avoidanceStrength;
        if (l.collider != null) avoid += (Vector2)l.normal * avoidanceStrength * 0.7f;
        if (r.collider != null) avoid += (Vector2)r.normal * avoidanceStrength * 0.7f;

        return avoid;
    }

    static Vector2 Rotate(Vector2 v, float deg)
    {
        float rad = deg * Mathf.Deg2Rad;
        return new Vector2(
            Mathf.Cos(rad) * v.x - Mathf.Sin(rad) * v.y,
            Mathf.Sin(rad) * v.x + Mathf.Cos(rad) * v.y
        );
    }

    void FlipFirePoint()
    {
        if (firePoint == null || target == null) return;
        firePoint.localPosition = target.position.x > transform.position.x
            ? new Vector3(1f, 0f, 0f)
            : new Vector3(-1f, 0f, 0f);
    }

    void FireAtPlayer()
    {
        if (shootSound != null && audioSource != null)
            audioSource.PlayOneShot(shootSound);

        Vector2 dir = ((Vector2)target.position - (Vector2)firePoint.position).normalized;
        Projectile p = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        float z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        p.transform.rotation = Quaternion.Euler(0, 0, z);
        p.Init(dir, projectileDamage, projectileSpeed, 5f, 0, true);
    }

    public void TakeDamage(float dmg)
    {
        if (isDead) return;
        
        currentHP = Mathf.Clamp(currentHP - dmg, 0, maxHP);
        if (currentHP <= 0) Die();
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
        int n = Random.Range(minCoins, maxCoins + 1);
        for (int i = 0; i < n; i++)
        {
            var c = Instantiate(coinPrefab, transform.position, Quaternion.identity);
            var rb2 = c.GetComponent<Rigidbody2D>();
            if (rb2) rb2.AddForce(Random.insideUnitCircle.normalized * dropForce, ForceMode2D.Impulse);
        }
    }
}
