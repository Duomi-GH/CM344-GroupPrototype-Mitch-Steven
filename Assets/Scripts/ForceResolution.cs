using UnityEngine;

public class ForceResolution : MonoBehaviour
{
    [Header("Target Resolution")]
    public int targetWidth = 1920;
    public int targetHeight = 1080;

    [Header("Screen Mode")]
    public FullScreenMode fullScreenMode = FullScreenMode.FullScreenWindow;
    public bool allowRefreshRateFallback = true;

    private void Start()
    {
        // This will set the resolution and fullscreen mode when the scene starts.
        Screen.SetResolution(targetWidth, targetHeight, fullScreenMode, 0);

        // If you want strictly exclusive fullscreen (Windows only), use:
        // fullScreenMode = FullScreenMode.ExclusiveFullScreen;
    }
}