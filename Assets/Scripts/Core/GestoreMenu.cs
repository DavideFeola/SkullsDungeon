using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;

public class GestoreMenu : MonoBehaviour
{
    [Header("Riferimenti Pannelli")]
    public GameObject pannelloPrincipale;
    public GameObject pannelloOpzioni;
    public GameObject pannelloInformazioni;

    [Header("Nome Scena Iniziale")]
    public string nomeDellaPrimaStanza = "Stanza1"; 

    void Start()
    {
        // All'inizio attiva solo il menu principale
        MostraSoloPrincipale();
    }

    // --- FUNZIONI PULSANTI ---

    public void Gioca()
    {
        // Carica la prima stanza del gioco
        SceneManager.LoadScene(nomeDellaPrimaStanza);
    }

    public void ApriOpzioni()
    {
        DisattivaTuttiIPannelli();
        pannelloOpzioni.SetActive(true);
    }

    public void ApriInformazioni()
    {
        DisattivaTuttiIPannelli();
        pannelloInformazioni.SetActive(true);
    }

    public void MostraSoloPrincipale()
    {
        DisattivaTuttiIPannelli();
        pannelloPrincipale.SetActive(true);
    }

    // --- LOGICA INTERNA ---

    private void DisattivaTuttiIPannelli()
    {
        pannelloPrincipale.SetActive(false);
        pannelloOpzioni.SetActive(false);
        pannelloInformazioni.SetActive(false);
    }

    public void CambiaVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}
