using UnityEngine;

public class UIButtonClose : MonoBehaviour
{
    public void CloseSound()
    {
        if (UIAudioManager.I != null)
            UIAudioManager.I.PlayClose();
    }
}
