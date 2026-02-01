using UnityEngine;
 
[CreateAssetMenu(menuName = "Game/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string displayName = "Weapon";
    public Projectile projectilePrefab;
    public float damage = 1f;
    public float fireRate = 5f;
    public float projectileSpeed = 12f;
    public float lifeTime = 2f;
    public int projectilesPerShot = 1;
    public float spread = 0f;
    public int pierce = 0;
    
    [Header("Munizioni")]
    public bool useAmmo = true;           // Se false = munizioni infinite (debug)
    public int startingAmmo = 100;         // Munizioni iniziali
    public int maxAmmo = 200;             // Munizioni massime totali
    
    [HideInInspector]
    public int currentAmmo;               // Munizioni attuali (runtime)
    
    // Chiama questo all'inizio del gioco
    public void Initialize()
    {
        currentAmmo = startingAmmo;
    }
}