using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CanvasManager : MonoBehaviour
{
    // Nascondi questo canvas e fai scalare tutti quelli sotto
    public void HideAndShiftUp()
    {
        Canvas myCanvas = GetComponent<Canvas>();
        if (myCanvas == null) return;

        int myOrder = myCanvas.sortingOrder;
        
        // Nascondi questo canvas
        gameObject.SetActive(false);
        
        // Trova tutti i canvas attivi
        Canvas[] allCanvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        
        // Filtra solo quelli che erano SOTTO questo (Sort Order inferiore)
        List<Canvas> canvasesBelow = new List<Canvas>();
        foreach (Canvas c in allCanvases)
        {
            if (c.gameObject.activeInHierarchy && c.sortingOrder < myOrder)
            {
                canvasesBelow.Add(c);
            }
        }
        
        // Ordina per Sort Order decrescente (dal più alto al più basso)
        canvasesBelow = canvasesBelow.OrderByDescending(c => c.sortingOrder).ToList();
        
        // Il primo (quello più vicino sotto) prende il posto di questo canvas
        if (canvasesBelow.Count > 0)
        {
            Canvas topBelow = canvasesBelow[0];
            int oldOrder = topBelow.sortingOrder;
            topBelow.sortingOrder = myOrder;
            
            
            // Opzionale: scala anche tutti gli altri di conseguenza
            for (int i = 1; i < canvasesBelow.Count; i++)
            {
                canvasesBelow[i].sortingOrder = myOrder - (i);
            }
        }
    }
    
    // Mostra questo canvas sopra tutti gli altri
    public void ShowOnTop()
    {
        Canvas myCanvas = GetComponent<Canvas>();
        if (myCanvas == null) return;
        
        // Trova il Sort Order più alto
        Canvas[] allCanvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        int maxOrder = 0;
        
        foreach (Canvas c in allCanvases)
        {
            if (c.gameObject.activeInHierarchy && c != myCanvas && c.sortingOrder > maxOrder)
                maxOrder = c.sortingOrder;
        }
        
        // Mettiti sopra tutti
        myCanvas.sortingOrder = maxOrder + 1;
        gameObject.SetActive(true);
        
    }
    
    // Imposta Sort Order manualmente
    public void SetSortOrder(int order)
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.sortingOrder = order;
        }
    }
}
