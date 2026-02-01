using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ShopInteraction : MonoBehaviour
{
    [Header("Shop Settings")]
    public int healthCost = 5;
    public int healthAmount = 50;
    public int gunCost = 10;
    public int ammoCost = 15;
    public int ammoAmount = 50;
    
    [Header("UI")]
    public GameObject shopUI;
    public TextMeshProUGUI healthCostText;
    public TextMeshProUGUI gunCostText;
    public TextMeshProUGUI ammoCostText;
    public TextMeshProUGUI coinDisplayText;
    public TextMeshProUGUI feedbackTextSufficente;
    public TextMeshProUGUI feedbackTextInsufficiente;
    
    [Header("Gun Button")]
    public Button gunButton;
    public TextMeshProUGUI gunPurchasedText;
    
    [Header("Dialogue System")]
    public DialogueTypewriter dialogueTypewriter;
    
    [Header("Auto Open Settings")]
    public bool autoOpenOnEnter = true;
    public bool autoCloseOnExit = true;
    
    [Header("Audio")]
    private AudioSource shopPanelAudio;
    public AudioSource purchaseAudio; // ✅ Suono acquisto
    public AudioSource errorAudio; // ✅ NUOVO: Suono errore/saldo insufficiente
    
    private GameObject player;
    
    void Start()
    {
        if (shopUI != null)
            shopUI.SetActive(false);
        
        UpdateCostTexts();
        
        if (feedbackTextSufficente != null)
            feedbackTextSufficente.gameObject.SetActive(false);
        
        if (feedbackTextInsufficiente != null)
            feedbackTextInsufficiente.gameObject.SetActive(false);
        
        // Prendi l'AudioSource dal pannello shop
        if (shopUI != null)
        {
            shopPanelAudio = shopUI.GetComponent<AudioSource>();
            if (shopPanelAudio == null)
            {
                Debug.LogWarning("AudioSource non trovato su ShopUI! Aggiungi un AudioSource al pannello.");
            }
        }
    }
    
    void Update()
    {
        if (shopUI != null && shopUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            
            
            if (autoOpenOnEnter)
            {
                OpenShop();
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
            
            
            if (autoCloseOnExit && shopUI != null && shopUI.activeSelf)
            {
                CloseShop();
            }
        }
    }
    
    void OpenShop()
    {
        if (shopUI != null)
        {
            // Riproduci il suono prima di aprire
            if (shopPanelAudio != null)
            {
                shopPanelAudio.Play();
            }
            
            shopUI.SetActive(true);
            UpdateCoinDisplay();
            UpdateGunButtonState();
            Time.timeScale = 0f;
            
            if (dialogueTypewriter != null)
            {
                dialogueTypewriter.StartDialogue();
            }
            
        }
    }
    
    public void CloseShop()
    {
        if (shopUI != null)
        {
            shopUI.SetActive(false);
            Time.timeScale = 1f;
            
        }
    }
    
    void UpdateCostTexts()
    {
        if (healthCostText != null)
            healthCostText.text = healthCost.ToString();
        
        if (gunCostText != null)
            gunCostText.text = gunCost.ToString();
        
        if (ammoCostText != null)
            ammoCostText.text = ammoCost.ToString();
    }
    
    void UpdateCoinDisplay()
    {
        if (coinDisplayText != null && CoinCollector.Instance != null)
        {
            coinDisplayText.text = CoinCollector.Instance.GetCoins().ToString();
        }
    }
    
    void UpdateGunButtonState()
    {
        if (player == null) return;
        
        Transform weaponTransform = player.transform.Find("Weapon");
        bool hasWeapon = weaponTransform != null && weaponTransform.gameObject.activeSelf;
        
        if (hasWeapon)
        {
            if (gunButton != null)
                gunButton.interactable = false;
            
            if (gunCostText != null)
                gunCostText.gameObject.SetActive(false);
            
            if (gunPurchasedText != null)
                gunPurchasedText.gameObject.SetActive(true);
        }
        else
        {
            if (gunButton != null)
                gunButton.interactable = true;
            
            if (gunCostText != null)
                gunCostText.gameObject.SetActive(true);
            
            if (gunPurchasedText != null)
                gunPurchasedText.gameObject.SetActive(false);
        }
    }
    
    public void BuyHealth()
    {
        if (CoinCollector.Instance == null)
        {
            ShowFeedbackError("Coin Collector non trovato!");
            return;
        }
        
        if (CoinCollector.Instance.GetCoins() >= healthCost)
        {
            CoinCollector.Instance.SpendCoins(healthCost);
            
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healthAmount);
                ShowFeedbackSuccess("Health regenerated! +" + healthAmount + " HP");
                UpdateCoinDisplay();
            }
            else
            {
                ShowFeedbackError("PlayerHealth non trovato!");
            }
        }
        else
        {
            ShowFeedbackError("Ahahaha, you're poor...");
        }
    }
    
    public void BuyGun()
    {
        if (CoinCollector.Instance == null)
        {
            ShowFeedbackError("Coin Collector non trovato!");
            return;
        }

        if (CoinCollector.Instance.GetCoins() >= gunCost)
        {
            CoinCollector.Instance.SpendCoins(gunCost);

            Transform weaponTransform = player.transform.Find("Weapon");
            if (weaponTransform != null)
            {

                weaponTransform.gameObject.SetActive(true);


                PlayerController pc = player.GetComponent<PlayerController>();
                if (pc != null)
                {
                    pc.EquipWeaponFromShop();
                }
                else
                {
                    Debug.LogError("[Shop] ❌ PlayerController non trovato sul player!");
                }
            }
            else
            {
                Debug.LogError("[Shop] ❌ GameObject Weapon NON trovato!");
            }

            ShowFeedbackSuccess(".357 Magnum purchased! See you later!");
            UpdateCoinDisplay();
            UpdateGunButtonState();

            FirstShopWall wall = FindFirstObjectByType<FirstShopWall>();
            if (wall != null)
            {
                wall.ForceUnlock();
            }

        }
        else
        {
            ShowFeedbackError("Ahahaha, you're poor...");
        }
    }
    
    public void BuyAmmo()
    {
        if (CoinCollector.Instance == null)
        {
            ShowFeedbackError("Coin Collector non trovato!");
            return;
        }
        
        if (CoinCollector.Instance.GetCoins() >= ammoCost)
        {
            Weapon playerWeapon = player.GetComponentInChildren<Weapon>();
            
            if (playerWeapon == null)
            {
                ShowFeedbackError("Weapon non trovato sul player!");
                return;
            }
            
            if (playerWeapon.GetCurrentAmmo() >= playerWeapon.GetMaxAmmo())
            {
                ShowFeedbackError("Ammo already at maximum!");
                return;
            }
            
            CoinCollector.Instance.SpendCoins(ammoCost);
            playerWeapon.AddAmmo(ammoAmount);
            
            ShowFeedbackSuccess($"+{ammoAmount} Ammo purchased! Pew pew!");
            UpdateCoinDisplay();
            
            if (AmmoUI.Instance != null)
                AmmoUI.Instance.ForceUpdate();
            
        }
        else
        {
            ShowFeedbackError("Ahahaha, you're poor...");
        }
    }
    
    void ShowFeedbackSuccess(string message)
    {
        // ✅ RIPRODUCI IL SUONO DELL'ACQUISTO QUI!
        if (purchaseAudio != null)
        {
            purchaseAudio.Play();
        }
        
        if (feedbackTextInsufficiente != null)
            feedbackTextInsufficiente.gameObject.SetActive(false);
        
        if (feedbackTextSufficente != null)
        {
            feedbackTextSufficente.text = message;
            feedbackTextSufficente.gameObject.SetActive(true);
            
            StopCoroutine("HideFeedbackSuccessCoroutine");
            StartCoroutine(HideFeedbackSuccessCoroutine());
        }
        
    }
    
    void ShowFeedbackError(string message)
{
    // ✅ CAMBIA DA .Play() A .PlayOneShot()
    if (errorAudio != null && errorAudio.clip != null)
    {
        errorAudio.PlayOneShot(errorAudio.clip);
    }
    
    if (feedbackTextSufficente != null)
        feedbackTextSufficente.gameObject.SetActive(false);
    
    if (feedbackTextInsufficiente != null)
    {
        feedbackTextInsufficiente.text = message;
        feedbackTextInsufficiente.gameObject.SetActive(true);
        
        StopCoroutine("HideFeedbackErrorCoroutine");
        StartCoroutine(HideFeedbackErrorCoroutine());
    }
    
}
    
    IEnumerator HideFeedbackSuccessCoroutine()
    {
        yield return new WaitForSecondsRealtime(2f);
        
        if (feedbackTextSufficente != null)
            feedbackTextSufficente.gameObject.SetActive(false);
    }
    
    IEnumerator HideFeedbackErrorCoroutine()
    {
        yield return new WaitForSecondsRealtime(2f);
        
        if (feedbackTextInsufficiente != null)
            feedbackTextInsufficiente.gameObject.SetActive(false);
    }
}