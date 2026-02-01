using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    public WeaponData data;
    public Transform firePoint;

    [Header("Audio")]
    public AudioClip shootSound;
    private AudioSource audioSource;

    Vector2 aimDir = Vector2.right;
    bool firing;
    Coroutine fireCR;

    float MinFireDelay => 1f / Mathf.Max(0.01f, data ? data.fireRate : 5f);

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        if (data != null)
        {
            data.Initialize();
        }
    }

    public void SetAimDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude > 1e-6f) 
            aimDir = dir.normalized;
    }

    public void TryStartFire()
    {
        if (firing) return;

        if (!DungeonGenerator.IsInCombatRoom)
            return;

        if (data.useAmmo && data.currentAmmo <= 0)
            return;

        if (!data || !data.projectilePrefab || !firePoint)
            return;

        firing = true;
        fireCR = StartCoroutine(FireLoop());
    }

    public void StopFire()
    {
        firing = false;
        if (fireCR != null) 
            StopCoroutine(fireCR);
        fireCR = null;
    }

    IEnumerator FireLoop()
    {
        while (firing)
        {
            if (data.useAmmo && data.currentAmmo <= 0)
            {
                StopFire();
                yield break;
            }

            FireOnce();
            yield return new WaitForSeconds(MinFireDelay);
        }
    }

    void FireOnce()
    {
        if (!data || !data.projectilePrefab || !firePoint)
            return;

        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        if (data.useAmmo)
        {
            data.currentAmmo--;
        }

        Vector2 baseDir = aimDir.sqrMagnitude > 1e-6f ? aimDir : Vector2.right;
        int shots = Mathf.Max(1, data.projectilesPerShot);

        for (int i = 0; i < shots; i++)
        {
            float angle = Random.Range(-data.spread, data.spread);
            Vector2 dir = (Quaternion.Euler(0, 0, angle) * (Vector3)baseDir).normalized;

            Projectile proj = Instantiate(data.projectilePrefab, firePoint.position, Quaternion.identity);
            float zRot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            proj.transform.rotation = Quaternion.Euler(0, 0, zRot);
            proj.Init(dir, data.damage, data.projectileSpeed, data.lifeTime, data.pierce);
        }
    }

    void OnDisable()
    {
        StopFire();
    }

    public void ResetAimDirection()
    {
        aimDir = Vector2.right;
    }

    public void AddAmmo(int amount)
    {
        if (!data) return;
        data.currentAmmo = Mathf.Min(data.currentAmmo + amount, data.maxAmmo);
    }

    public int GetCurrentAmmo()
    {
        return data ? data.currentAmmo : 0;
    }

    public int GetMaxAmmo()
    {
        return data ? data.maxAmmo : 0;
    }
}
