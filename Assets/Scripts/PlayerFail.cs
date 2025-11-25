using UnityEngine;

public class PlayerFail : MonoBehaviour
{
    public GameObject failPanel;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If we already failed, ignore further collisions
        if (triggered) return;

        // Check if the object has the Hazard tag
        if (other.CompareTag("Hazard"))
        {
            Fail();
        }
    }

    public void Fail()
    {
        if (triggered) return;
        triggered = true;

        // Pause the game
        Time.timeScale = 0f;

        // Show the fail UI
        if (failPanel != null)
            failPanel.SetActive(true);

        Debug.Log("Player failed (hit a hazard).");
    }

    // Connected to your retry button
    public void RestartLevel()
    {
        Time.timeScale = 1f; // Unpause the game
        GameSceneManager.Instance.ReloadCurrentScene();
    }
}
