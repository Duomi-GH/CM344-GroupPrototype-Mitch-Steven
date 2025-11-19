using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; private set; }

    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenuScene";

    [Tooltip("Scene names in order. Element 0 is Level 1, element 1 is Level 2, and so on.")]
    [SerializeField] private string[] levelSceneNames;   // size 6 for Level_1 to Level_6

    [Header("Level Progression")]
    [Tooltip("Total number of levels in the game.")]
    [SerializeField] private int totalLevels = 6;

    [Header("Main Menu UI (Optional)")]
    [Tooltip("Panel or canvas that contains the Level Select buttons. Used only in the Main Menu scene.")]
    [SerializeField] private GameObject levelSelectPanel;

    private bool isLoading = false;

    // Level numbering is 1 based: 1 to totalLevels
    private int currentLevelNumber = 1;

    private void Awake()
    {
        // Per scene singleton
        Instance = this;
    }

    private void Start()
    {
        // Ensure Level 1 is always unlocked
        if (!PlayerPrefs.HasKey("UnlockedLevel_1"))
        {
            PlayerPrefs.SetInt("UnlockedLevel_1", 1);
            PlayerPrefs.Save();
        }

        // Make sure Level Select panel starts hidden if assigned
        if (levelSelectPanel != null)
        {
            levelSelectPanel.SetActive(false);
        }
    }

    // -----------------------------
    // Public API for Main Menu UI
    // -----------------------------

    // "Play" button on Main Menu
    public void StartNewGame()
    {
        currentLevelNumber = 1;
        LoadLevelByNumber(1);
    }

    // Back to Main Menu button
    public void LoadMainMenu()
    {
        // If you ever pause with Time.timeScale = 0 in levels,
        // you can safely reset it here.
        Time.timeScale = 1f;
        LoadScene(mainMenuSceneName);
    }

    // Top right "return to main menu" in any level
    public void ReturnToMainMenuFromLevel()
    {
        LoadMainMenu();
    }

    // Level Select button in Main Menu
    public void OpenLevelSelectPanel()
    {
        if (levelSelectPanel != null)
        {
            levelSelectPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Level Select panel is not assigned on GameSceneManager.");
        }
    }

    // Optional: Back/Close button on Level Select panel
    public void CloseLevelSelectPanel()
    {
        if (levelSelectPanel != null)
        {
            levelSelectPanel.SetActive(false);
        }
    }

    // -----------------------------
    // Level Select API
    // -----------------------------

    // Called by Level Select buttons (Level 1 to Level 6)
    public void TryLoadLevelFromSelect(int levelNumber)
    {
        if (!IsValidLevel(levelNumber))
        {
            Debug.LogWarning($"Requested level {levelNumber} is outside valid range 1 to {totalLevels}.");
            return;
        }

        if (!IsLevelUnlocked(levelNumber))
        {
            Debug.Log($"Level {levelNumber} is locked. Ignoring load request from Level Select.");
            return;
        }

        LoadLevelByNumber(levelNumber);
    }

    // -----------------------------
    // Level Complete Handling
    // -----------------------------

    /// <summary>
    /// Called by your level complete trigger when the player wins.
    /// Parameters:
    ///  - currentLevelNumber: the level the player just finished
    ///  - nextLevelNumber: the level to load when the player hits Continue
    ///  - winPanel: panel to enable
    ///  - continueButton: button that will load the next level or main menu
    /// </summary>
    public void HandleLevelComplete(
        int currentLevelNumber,
        int nextLevelNumber,
        GameObject winPanel,
        Button continueButton)
    {
        this.currentLevelNumber = currentLevelNumber;

        // Unlock the next level if it is valid
        if (IsValidLevel(nextLevelNumber))
        {
            UnlockLevel(nextLevelNumber);
        }

        // Show the win panel
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        // Wire up the continue button
        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();

            if (IsValidLevel(nextLevelNumber) && !string.IsNullOrEmpty(GetSceneNameForLevel(nextLevelNumber)))
            {
                continueButton.onClick.AddListener(() => LoadLevelByNumber(nextLevelNumber));
            }
            else
            {
                // If next level is invalid or not configured, fall back to main menu
                continueButton.onClick.AddListener(LoadMainMenu);
            }
        }
    }

    // -----------------------------
    // Progression and PlayerPrefs
    // -----------------------------

    public bool IsLevelUnlocked(int levelNumber)
    {
        if (!IsValidLevel(levelNumber))
            return false;

        string key = $"UnlockedLevel_{levelNumber}";
        return PlayerPrefs.GetInt(key, 0) == 1;
    }

    public void UnlockLevel(int levelNumber)
    {
        if (!IsValidLevel(levelNumber))
            return;

        string key = $"UnlockedLevel_{levelNumber}";
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();
    }

    private bool IsValidLevel(int levelNumber)
    {
        return levelNumber >= 1 && levelNumber <= totalLevels;
    }

    // -----------------------------
    // Core scene loading
    // -----------------------------

    private void LoadLevelByNumber(int levelNumber)
    {
        if (!IsValidLevel(levelNumber))
        {
            Debug.LogError($"LoadLevelByNumber called with invalid level {levelNumber}");
            return;
        }

        string sceneName = GetSceneNameForLevel(levelNumber);
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError($"No scene name configured for level {levelNumber}. Check levelSceneNames array.");
            return;
        }

        currentLevelNumber = levelNumber;
        LoadScene(sceneName);
    }

    private string GetSceneNameForLevel(int levelNumber)
    {
        int index = levelNumber - 1;

        if (levelSceneNames == null || index < 0 || index >= levelSceneNames.Length)
            return null;

        return levelSceneNames[index];
    }

    private void LoadScene(string sceneName)
    {
        if (!isLoading)
        {
            StartCoroutine(LoadSceneAsyncRoutine(sceneName));
        }
    }

    private IEnumerator LoadSceneAsyncRoutine(string sceneName)
    {
        isLoading = true;
        Debug.Log($"Loading scene: {sceneName}");

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            yield return null;
        }

        isLoading = false;
    }

    // -----------------------------
    // Utility
    // -----------------------------

    public void ReloadCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        LoadScene(currentSceneName);
    }

    public void QuitApplication()
    {
        Debug.Log("Quitting application...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // -----------------------------
    // Backward compatibility
    // -----------------------------
    // These methods keep older scripts working
    // without having to rewrite MainMenuUI or MainMenuLevelSelect.

    // Old name used by MainMenuUI
    public void LoadGame()
    {
        StartNewGame();
    }

    // Old progression setter used by MainMenuLevelSelect
    public void SetCurrentLevel(int levelNumber)
    {
        if (IsValidLevel(levelNumber))
            currentLevelNumber = levelNumber;
    }

    // Old direct scene loader, sometimes called after SetCurrentLevel
    public void LoadSpecificLevel(string sceneName)
    {
        // Try to infer level number from scene name like "Level_1" or "Level_01"
        if (TryGetLevelNumberFromSceneName(sceneName, out int levelNumber) && IsValidLevel(levelNumber))
        {
            currentLevelNumber = levelNumber;
        }

        // Use the core loader so async logic and isLoading flag are respected
        LoadScene(sceneName);
    }

    private bool TryGetLevelNumberFromSceneName(string sceneName, out int levelNumber)
    {
        levelNumber = -1;

        if (string.IsNullOrEmpty(sceneName))
            return false;

        int underscoreIndex = sceneName.LastIndexOf('_');
        if (underscoreIndex < 0 || underscoreIndex == sceneName.Length - 1)
            return false;

        string numberPart = sceneName.Substring(underscoreIndex + 1);   // "1" or "01"

        if (int.TryParse(numberPart, out int parsed))
        {
            levelNumber = parsed;
            return true;
        }

        return false;
    }
}
