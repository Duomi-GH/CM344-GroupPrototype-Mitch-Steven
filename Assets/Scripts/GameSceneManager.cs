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
    public int CurrentLevel => currentLevelNumber;
    private int currentLevelNumber = 1;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Ensure Level 1 is unlocked
        if (!PlayerPrefs.HasKey("UnlockedLevel_1"))
        {
            PlayerPrefs.SetInt("UnlockedLevel_1", 1);
            PlayerPrefs.Save();
        }

        // Hide Level Select initially
        if (levelSelectPanel != null)
            levelSelectPanel.SetActive(false);
    }

    // -------------------------------------------------
    // Menu Buttons
    // -------------------------------------------------

    public void StartNewGame()
    {
        currentLevelNumber = 1;
        LoadLevelByNumber(1);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // unpause
        LoadScene(mainMenuSceneName);
    }

    public void ReturnToMainMenuFromLevel()
    {
        LoadMainMenu();
    }

    public void OpenLevelSelectPanel()
    {
        if (levelSelectPanel != null)
            levelSelectPanel.SetActive(true);
    }

    public void CloseLevelSelectPanel()
    {
        if (levelSelectPanel != null)
            levelSelectPanel.SetActive(false);
    }

    // -------------------------------------------------
    // Level Select
    // -------------------------------------------------

    public void TryLoadLevelFromSelect(int levelNumber)
    {
        if (!IsValidLevel(levelNumber))
        {
            Debug.LogWarning($"Requested level {levelNumber} is invalid.");
            return;
        }

        if (!IsLevelUnlocked(levelNumber))
        {
            Debug.Log($"Level {levelNumber} is locked.");
            return;
        }

        LoadLevelByNumber(levelNumber);
    }

    // -------------------------------------------------
    // Level Complete Logic
    // -------------------------------------------------

    public void HandleLevelComplete(
        int currentLevelNumber,
        int nextLevelNumber,
        GameObject winPanel,
        Button continueButton)
    {
        this.currentLevelNumber = currentLevelNumber;

        // Unlock next level
        if (IsValidLevel(nextLevelNumber))
            UnlockLevel(nextLevelNumber);

        // Show win panel
        if (winPanel != null)
            winPanel.SetActive(true);

        // Set up Continue button
        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();

            if (IsValidLevel(nextLevelNumber) &&
                !string.IsNullOrEmpty(GetSceneNameForLevel(nextLevelNumber)))
            {
                continueButton.onClick.AddListener(() =>
                {
                    Time.timeScale = 1f;    // UNPAUSE
                    LoadLevelByNumber(nextLevelNumber);
                });
            }
            else
            {
                continueButton.onClick.AddListener(() =>
                {
                    Time.timeScale = 1f;    // UNPAUSE
                    LoadMainMenu();
                });
            }
        }
    }

    // -------------------------------------------------
    // Progression / PlayerPrefs
    // -------------------------------------------------

    public bool IsLevelUnlocked(int levelNumber)
    {
        if (!IsValidLevel(levelNumber))
            return false;

        return PlayerPrefs.GetInt($"UnlockedLevel_{levelNumber}", 0) == 1;
    }

    public void UnlockLevel(int levelNumber)
    {
        if (!IsValidLevel(levelNumber))
            return;

        PlayerPrefs.SetInt($"UnlockedLevel_{levelNumber}", 1);
        PlayerPrefs.Save();
    }

    private bool IsValidLevel(int levelNumber)
    {
        return levelNumber >= 1 && levelNumber <= totalLevels;
    }

    // -------------------------------------------------
    // Scene Loading (FIXED for unpause)
    // -------------------------------------------------

    private void LoadLevelByNumber(int levelNumber)
    {
        if (!IsValidLevel(levelNumber))
        {
            Debug.LogError($"Level {levelNumber} is invalid.");
            return;
        }

        string sceneName = GetSceneNameForLevel(levelNumber);
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError($"Missing scene name for level {levelNumber}.");
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
        // *** FIX: UNPAUSE ANY TIME WE LOAD A SCENE ***
        Time.timeScale = 1f;

        if (!isLoading)
            StartCoroutine(LoadSceneAsyncRoutine(sceneName));
    }

    private IEnumerator LoadSceneAsyncRoutine(string sceneName)
    {
        isLoading = true;

        Debug.Log("Loading scene: " + sceneName);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
            yield return null;

        isLoading = false;
    }

    // -------------------------------------------------
    // Utility
    // -------------------------------------------------

    public void ReloadCurrentScene()
    {
        Time.timeScale = 1f;
        LoadScene(SceneManager.GetActiveScene().name);
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

    // -------------------------------------------------
    // Backwards compatibility
    // -------------------------------------------------

    public void LoadGame()
    {
        StartNewGame();
    }

    public void SetCurrentLevel(int levelNumber)
    {
        if (IsValidLevel(levelNumber))
            currentLevelNumber = levelNumber;
    }

    public void LoadSpecificLevel(string sceneName)
    {
        if (TryGetLevelNumberFromSceneName(sceneName, out int levelNumber)
            && IsValidLevel(levelNumber))
        {
            currentLevelNumber = levelNumber;
        }

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

        string numberPart = sceneName.Substring(underscoreIndex + 1);

        return int.TryParse(numberPart, out levelNumber);
    }
}
