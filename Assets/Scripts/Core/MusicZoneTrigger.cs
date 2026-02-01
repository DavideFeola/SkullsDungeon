using UnityEngine;

public class MusicZoneTrigger : MonoBehaviour
{
    public enum ZoneType { Shop, Combat }
    
    [Header("Zone Settings")]
    public ZoneType zoneType = ZoneType.Shop;
    
    [Header("Debug")]
    public bool showDebugGizmos = true;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Solo quando il player entra nella zona
        if (!other.CompareTag("Player")) return;
        
        if (AudioManager.Instance == null) return;
        
        switch (zoneType)
        {
            case ZoneType.Shop:
                AudioManager.Instance.PlayShopMusic();
                break;
                
            case ZoneType.Combat:
                AudioManager.Instance.PlayCombatMusic();
                break;
        }
    }
    
    // Visualizza la zona nell'editor
    void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;
        
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            Gizmos.color = zoneType == ZoneType.Shop ? 
                new Color(0, 1, 0, 0.3f) :  // Verde per Shop
                new Color(1, 0, 0, 0.3f);   // Rosso per Combat
            
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.offset, box.size);
        }
    }
}
