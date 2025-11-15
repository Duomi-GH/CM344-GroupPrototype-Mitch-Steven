using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; private set; }

    [Header("Scene Names")]
    public string mainMenuSceneName = "MainMenuScene";
    public string firstLevelSceneName = "Level_01";

    [Header("Level Progression")]
    [Tooltip("Total number of levels in the game.")]
    public int totalLevels = 5;

    [Header("UI References (Optional)")]
    public Canvas levelCompleteCanvas;
    public Text congratulationText;
    public Button continueButton;
    public Button mainMenuButton;

    private bool isLoading = false;
    private int currentLevel = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Ensure Level 1 is always unlocked on a new save
        if (!PlayerPrefs.HasKey("UnlockedLevel_1"))
            PlayerPrefs.SetInt("UnlockedLevel_1", 1);

        PlayerPrefs.Save();
    }

    // --- Public Scene Loading Methods ---

    public void LoadGame()
    {
        currentLevel = 1;
        LoadScene(firstLevelSceneName);
    }

    public void LoadMainMenu()
    {
        LoadScene(mainMenuSceneName);
    }

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

    // --- Safe External Scene Loader (for other scripts like MainMenuLevelSelect) ---
    public void LoadSpecificLevel(string sceneName)
    {
        if (!isLoading)
        {
            StartCoroutine(LoadSceneAsyncRoutine(sceneName));
        }
    }

    // --- Core Scene Loading Logic ---
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

    // --- Level Progression and UI Logic ---
    public void OnLevelComplete()
    {
        UnlockNextLevel();

        if (levelCompleteCanvas != null)
        {
            levelCompleteCanvas.gameObject.SetActive(true);
            congratulationText.text = $"Congratulations! You've completed Level {currentLevel}!";

            // Set up Continue button
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(LoadNextLevel);

            // Set up Return to Main Menu button
            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.RemoveAllListeners();
                mainMenuButton.onClick.AddListener(LoadMainMenu);
            }
        }
    }

    private void UnlockNextLevel()
    {
        int nextLevel = currentLevel + 1;
        if (nextLevel <= totalLevels)
        {
            string key = $"UnlockedLevel_{nextLevel}";
            PlayerPrefs.SetInt(key, 1);
            PlayerPrefs.Save();
        }
    }

    private void LoadNextLevel()
    {
        currentLevel++;
        string nextSceneName = $"Level_{currentLevel:D2}";

        if (Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            LoadScene(nextSceneName);
        }
        else
        {
            Debug.Log($"Next level '{nextSceneName}' not found. Returning to Main Menu.");
            LoadMainMenu();
        }
    }

    // --- Public Helpers for Level Select UI ---
    public bool IsLevelUnlocked(int levelNumber)
    {
        return PlayerPrefs.GetInt($"UnlockedLevel_{levelNumber}", 0) == 1;
    }

    public void SetCurrentLevel(int levelNumber)
    {
        currentLevel = levelNumber;
    }
}
