using UnityEngine;
using TMPro;
using System.Collections;

public class FirstShopWall : MonoBehaviour
{
    [Header("Warning UI")]
    public GameObject warningPanel;
    public float warningDisplayTime = 2f;
    
    private bool isUnlocked = false;
    
    void Start()
    {
        if (warningPanel != null)
            warningPanel.SetActive(false);
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (!collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("[FirstShopWall] Non è il player, ignoro");
            return;
        }
        
        Debug.Log("[FirstShopWall] È il PLAYER!");
        Debug.Log($"[FirstShopWall] isUnlocked = {isUnlocked}");
        
        if (isUnlocked)
        {
            Debug.Log("[FirstShopWall] Già sbloccato, ignoro");
            return;
        }
        
        // ✅ NUOVO: Cerca l'oggetto Weapon per nome
        Transform weaponTransform = collision.transform.Find("Weapon");
        
        if (weaponTransform != null)
        {
            Debug.Log($"[FirstShopWall] Weapon trovato! Nome: {weaponTransform.name}");
            Debug.Log($"[FirstShopWall] Weapon attivo? {weaponTransform.gameObject.activeSelf}");
            
            if (weaponTransform.gameObject.activeSelf)
            {
                Debug.Log("[FirstShopWall] ✅ Arma ATTIVA - Sblocco muro");
                Unlock();
                return;
            }
            else
            {
                Debug.Log("[FirstShopWall] ❌ Arma DISATTIVA - Mostro warning");
            }
        }
        else
        {
            Debug.Log("[FirstShopWall] ❌ Weapon NON trovato - Mostro warning");
        }
        
        ShowWarning();
    }
    
    void ShowWarning()
    {
        if (warningPanel != null)
        {
            StopAllCoroutines();
            StartCoroutine(ShowWarningCoroutine());
        }
    }
    
    IEnumerator ShowWarningCoroutine()
    {
        warningPanel.SetActive(true);
        yield return new WaitForSeconds(warningDisplayTime);
        warningPanel.SetActive(false);
    }
    
    void Unlock()
    {
        isUnlocked = true;
        gameObject.SetActive(false);
    }
    
    public void ForceUnlock()
    {
        Unlock();
    }
}
