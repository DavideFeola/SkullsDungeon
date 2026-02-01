using UnityEngine;

public class UIButtonRestartMenuSfx : MonoBehaviour
{
    public void PlayClick()
    {
        if (UIAudioManager.I != null)
            UIAudioManager.I.PlayClose();
    }
}
