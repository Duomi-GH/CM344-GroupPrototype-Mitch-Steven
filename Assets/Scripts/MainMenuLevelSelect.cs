using UnityEngine;
using UnityEngine.UI;

public class MainMenuLevelSelect : MonoBehaviour
{
    [Header("Panels")]
    [Tooltip("The main menu panel (Play, Options, Quit, etc.)")]
    public GameObject mainPanel;

    [Tooltip("The panel that contains the level select buttons (starts inactive)")]
    public GameObject levelSelectPanel;

    [Header("Buttons")]
    [Tooltip("Button on the main panel that opens the Level Select menu")]
    public Button openLevelSelectButton;

    [Tooltip("Button inside the Level Select panel that closes it and returns to main menu")]
    public Button closeLevelSelectButton;

    [Header("Level Buttons")]
    [Tooltip("Assign level buttons here in order (Level_01, Level_02, etc.).")]
    public Button[] levelButtons;

    private void Start()
    {
        // Ensure panels are in correct state at start
        mainPanel.SetActive(true);
        levelSelectPanel.SetActive(false);

        // Wire up open/close buttons
        if (openLevelSelectButton != null)
            openLevelSelectButton.onClick.AddListener(OpenLevelSelect);

        if (closeLevelSelectButton != null)
            closeLevelSelectButton.onClick.AddListener(CloseLevelSelect);

        // Setup level buttons based on unlock state
        UpdateLevelButtons();
    }

    private void OpenLevelSelect()
    {
        mainPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
        UpdateLevelButtons();
    }

    private void CloseLevelSelect()
    {
        levelSelectPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    private void UpdateLevelButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelNumber = i + 1;
            bool unlocked = GameSceneManager.Instance.IsLevelUnlocked(levelNumber);

            levelButtons[i].interactable = unlocked;
            levelButtons[i].onClick.RemoveAllListeners();

            if (unlocked)
            {
                levelButtons[i].onClick.AddListener(() => LoadLevel(levelNumber));
            }
        }
    }

    private void LoadLevel(int levelNumber)
    {
        string sceneName = $"Level_{levelNumber:D2}";
        GameSceneManager.Instance.SetCurrentLevel(levelNumber);
        GameSceneManager.Instance.LoadSpecificLevel(sceneName);
    }
}
