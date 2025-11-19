using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteTrigger : MonoBehaviour
{
    [Header("Level Numbers")]
    public int currentLevelNumber = 1;
    public int nextLevelNumber = 2;

    [Header("Win UI")]
    public GameObject winPanel;
    public Button continueButton;

    private bool triggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            if (GameSceneManager.Instance != null)
            {
                GameSceneManager.Instance.HandleLevelComplete(
                    currentLevelNumber,
                    nextLevelNumber,
                    winPanel,
                    continueButton);
            }
            else
            {
                Debug.LogError("GameSceneManager.Instance is null. Make sure it exists in the Main Menu scene.");
            }
        }
    }
}