using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth player;   // trascina il Player (che ha PlayerHealth)
    public Image fillImage;       // trascina la Image del Fill (non il BG)

    [Header("Colors")]
    public Color fullColor = Color.green;
    public Color midColor  = Color.yellow;
    public Color lowColor  = Color.red;

    void Awake()
    {
        if (fillImage != null)
        {
            // assicura le impostazioni giuste
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
            var c = fillImage.color; c.a = 1f; fillImage.color = c; // alpha pieno
        }
    }

    void LateUpdate()
    {
        if (!player || !fillImage) return;

        // calcola percentuale vita (0..1)
        float max = Mathf.Max(0.0001f, player.maxHP);
        float t = Mathf.Clamp01(player.currentHP / max);

        // aggiorna lunghezza
        fillImage.fillAmount = t;

        // aggiorna colore (verde→giallo→rosso)
        Color color;
        if (t > 0.5f)
        {
            float k = (t - 0.5f) * 2f; // 0..1
            color = Color.Lerp(midColor, fullColor, k);
        }
        else
        {
            float k = t * 2f; // 0..1
            color = Color.Lerp(lowColor, midColor, k);
        }
        color.a = 1f;
        fillImage.color = color;
    }
}


 
