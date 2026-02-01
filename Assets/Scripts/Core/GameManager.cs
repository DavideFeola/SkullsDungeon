using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager I;
 
    void Awake()
    {
        // Singleton: ce ne deve essere solo uno
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
 
        I = this;
        DontDestroyOnLoad(gameObject);
    }

}
