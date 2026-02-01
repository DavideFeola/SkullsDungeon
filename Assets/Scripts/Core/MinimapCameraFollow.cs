using UnityEngine;

public class MinimapCameraFollow : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float cameraDistance = 27f; // Distanza camera su asse Z per 2D
    [SerializeField] private bool followPlayer = true;
    
    [Header("Combat Room Only")]
    [SerializeField] private bool onlyInCombatRoom = true;

    [Header("Camera Bounds (limiti stanza)")]
    [SerializeField] private bool useBounds = true;
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX = 8f;
    [SerializeField] private float minY = -6f;
    [SerializeField] private float maxY = 6f;

    [Header("UI References")]
    [SerializeField] private GameObject minimapPanel; // ← Trascina MiniMapPanel qui nell'inspector
    
    private Transform player;
    private Camera minimapCam;
    
    void Start()
    {
        minimapCam = GetComponent<Camera>();
        
        FindPlayer();
        
        // Se non assegnato nell'inspector, prova a trovarlo
        if (minimapPanel == null)
        {
            minimapPanel = GameObject.Find("MiniMapPanel");
        }
        
        // Inizialmente disattiva la minimap
        if (onlyInCombatRoom)
        {
            SetMinimapActive(false);
        }
    }
    
    void LateUpdate()
    {
        // Se non abbiamo ancora trovato il player, riprova
        if (player == null)
        {
            FindPlayer();
            return;
        }
        
        // Segue il player (2D: camera su asse Z, guarda verso gli sprite su piano XY)
        if (followPlayer && minimapCam != null && minimapCam.enabled)
        {
            float targetX = player.position.x;
            float targetY = player.position.y;

            // Limita la camera ai confini della stanza, considerando quanto vede la camera
            if (useBounds && minimapCam.orthographic)
            {
                // Calcola quanto vede la camera
                float cameraHeight = minimapCam.orthographicSize * 2f;
                float cameraWidth = cameraHeight * minimapCam.aspect;

                // Riduci i bounds per mantenere il player sempre visibile
                float clampMinX = minX + (cameraWidth / 2f);
                float clampMaxX = maxX - (cameraWidth / 2f);
                float clampMinY = minY + (cameraHeight / 2f);
                float clampMaxY = maxY - (cameraHeight / 2f);

                // Se i bounds sono troppo stretti, centra la camera
                if (clampMinX >= clampMaxX)
                {
                    targetX = (minX + maxX) / 2f; // Centro X
                }
                else
                {
                    targetX = Mathf.Clamp(targetX, clampMinX, clampMaxX);
                }

                if (clampMinY >= clampMaxY)
                {
                    targetY = (minY + maxY) / 2f; // Centro Y
                }
                else
                {
                    targetY = Mathf.Clamp(targetY, clampMinY, clampMaxY);
                }
            }

            transform.position = new Vector3(
                targetX,
                targetY,
                -cameraDistance // Camera dietro gli sprite su asse Z negativo
            );
        }
    }
    
    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }
    
    // Metodo pubblico per centrare su una stanza specifica
    public void CenterOnRoom(Vector3 roomCenter)
    {
        float targetX = roomCenter.x;
        float targetY = roomCenter.y;

        // Limita la camera ai confini della stanza, considerando quanto vede la camera
        if (useBounds && minimapCam != null && minimapCam.orthographic)
        {
            // Calcola quanto vede la camera
            float cameraHeight = minimapCam.orthographicSize * 2f;
            float cameraWidth = cameraHeight * minimapCam.aspect;

            // Riduci i bounds per mantenere tutto visibile
            float clampMinX = minX + (cameraWidth / 2f);
            float clampMaxX = maxX - (cameraWidth / 2f);
            float clampMinY = minY + (cameraHeight / 2f);
            float clampMaxY = maxY - (cameraHeight / 2f);

            // Se i bounds sono troppo stretti, centra la camera
            if (clampMinX >= clampMaxX)
            {
                targetX = (minX + maxX) / 2f;
            }
            else
            {
                targetX = Mathf.Clamp(targetX, clampMinX, clampMaxX);
            }

            if (clampMinY >= clampMaxY)
            {
                targetY = (minY + maxY) / 2f;
            }
            else
            {
                targetY = Mathf.Clamp(targetY, clampMinY, clampMaxY);
            }
        }

        transform.position = new Vector3(targetX, targetY, -cameraDistance);
    }
    
    // Metodo pubblico per attivare/disattivare la minimap
    public void SetMinimapActive(bool active)
    {
        // Attiva/disattiva la camera
        if (minimapCam != null)
        {
            minimapCam.enabled = active;

            // Quando si attiva, centra subito la camera
            if (active && player != null)
            {
                float centerX = (minX + maxX) / 2f;
                float centerY = (minY + maxY) / 2f;
                transform.position = new Vector3(centerX, centerY, -cameraDistance);
            }
        }

        // Attiva/disattiva il panel UI
        if (minimapPanel != null)
        {
            minimapPanel.SetActive(active);
        }
    }
    
    // Metodo pubblico per sapere se sta seguendo il player
    public bool IsFollowingPlayer()
    {
        return followPlayer;
    }

    // Metodo pubblico per impostare i limiti della stanza dinamicamente
    public void SetRoomBounds(float newMinX, float newMaxX, float newMinY, float newMaxY)
    {
        minX = newMinX;
        maxX = newMaxX;
        minY = newMinY;
        maxY = newMaxY;
    }
}