using UnityEngine;
using UnityEngine.SceneManagement;
 
public class SceneLoader : MonoBehaviour
{
    [Tooltip("Nome della scena da caricare dopo il Boot")]
    public string sceneName = "Game";
 
    void Start()
    {
        SceneManager.LoadScene(sceneName);
    }
}
