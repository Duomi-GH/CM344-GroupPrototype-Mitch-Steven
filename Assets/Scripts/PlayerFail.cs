using UnityEngine;

public class PlayerFail : MonoBehaviour
{
    public GameObject failPanel;

    // references to the key managers
    public BlueKeyManager blueKey;
    public RedKeyManager redKey;
    public YellowKeyManager yellowKey;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        string tag = other.tag;

        // --- BLACK HAZARD (always kills) ---
        if (tag == "Hazard")
        {
            Fail();
            return;
        }

        // --- BLUE HAZARD ---
        if (tag == "HazardBlue")
        {
            if (blueKey != null && blueKey.blueHave)
            {
                Destroy(other.gameObject);  // player destroys hazard
            }
            else
            {
                Fail(); // no key = fail
            }
            return;
        }

        // --- RED HAZARD ---
        if (tag == "HazardRed")
        {
            if (redKey != null && redKey.redHave)
            {
                Destroy(other.gameObject);
            }
            else
            {
                Fail();
            }
            return;
        }

        // --- YELLOW HAZARD ---
        if (tag == "HazardYellow")
        {
            if (yellowKey != null && yellowKey.yellowHave)
            {
                Destroy(other.gameObject);
            }
            else
            {
                Fail();
            }
        }
    }

    public void Fail()
    {
        if (triggered) return;
        triggered = true;

        Time.timeScale = 0f;

        if (failPanel != null)
            failPanel.SetActive(true);

        Debug.Log("PLAYER FAILED.");
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        GameSceneManager.Instance.ReloadCurrentScene();
    }
}
