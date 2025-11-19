using UnityEngine;

public class LevelSelectButton : MonoBehaviour
{
    [Tooltip("The human readable level number. Level 1, Level 2, etc.")]
    public int levelNumber = 1;

    // Hook this to the Button.onClick in the inspector
    public void OnLevelButtonClicked()
    {
        if (GameSceneManager.Instance != null)
        {
            GameSceneManager.Instance.TryLoadLevelFromSelect(levelNumber);
        }
        else
        {
            Debug.LogError("GameSceneManager.Instance is null. Make sure it exists in the Main Menu scene.");
        }
    }
}