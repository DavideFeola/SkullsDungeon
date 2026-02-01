using UnityEngine;
using Unity.Cinemachine;

public class CameraSetup : MonoBehaviour
{
    void Start()
    {
        // Aspetta che il player sia spawnato
        Invoke("FindPlayer", 0.5f);
    }
    
    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            CinemachineCamera vcam = GetComponent<CinemachineCamera>();
            vcam.Follow = player.transform;
            vcam.LookAt = player.transform;
        }
    }
}