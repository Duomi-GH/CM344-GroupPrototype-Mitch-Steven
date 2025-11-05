using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management
using System.Collections; // Required for Coroutines

/// <summary>
/// Persistent Singleton class for managing game state and scene loading.
/// This object will persist across all scenes.
/// </summary>
public class GameSceneManager : MonoBehaviour
{
    // Singleton pattern
    public static GameSceneManager Instance { get; private set; }

    // --- Public Fields ---
    [Header("Scene Names")]
    [Tooltip("The name of your main menu scene file.")]
    public string mainMenuSceneName = "MainMenuScene";
    
    // Note: Removed optionsSceneName
    
    [Tooltip("The name of the first game level scene file.")]
    public string firstLevelSceneName = "Level_01";
    
    // --- Private State ---
    private bool isLoading = false; // Prevents multiple load operations

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// This is used to set up the Singleton pattern.
    /// </summary>
    void Awake()
    {
        // Check if an Instance already exists
        if (Instance == null)
        {
            // If not, set this as the instance
            Instance = this;
            // And make it persist across scene loads
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If an instance already exists (e.g., returning to main menu),
            // destroy this duplicate.
            Destroy(gameObject);
        }
    }

    // --- Public Scene Loading Methods ---

    /// <summary>
    /// Loads the first game level.
    /// Called by the Play button.
    /// </summary>
    public void LoadGame()
    {
        // Use the public field for the scene name
        LoadScene(firstLevelSceneName);
    }

    // Note: Removed LoadOptions()

    /// <summary>
    /// Loads the main menu.
    /// Called by 'Back' buttons in other scenes (like a Pause Menu).
    /// </summary>
    public void LoadMainMenu()
    {
        LoadScene(mainMenuSceneName);
    }

    /// <summary>
    /// Reloads the currently active scene.
    /// Useful for a 'Restart Level' button.
    /// </summary>
    public void ReloadCurrentScene()
    {
        // Get the currently active scene and load it again
        string currentSceneName = SceneManager.GetActiveScene().name;
        LoadScene(currentSceneName);
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void QuitApplication()
    {
        Debug.Log("Quitting application...");

        // Application.Quit() only works in a built game.
        // This conditional logic handles quitting in the Unity Editor.
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }


    // --- Core Scene Loading Logic ---

    /// <summary>
    /// The main private method to load a scene by its name.
    /// </summary>
    /// <param name="sceneName">The exact name of the scene file to load.</param>
    public void LoadScene(string sceneName)
    {
        if (!isLoading)
        {
            StartCoroutine(LoadSceneAsyncRoutine(sceneName));
        }
    }

    /// <summary>
    /// Asynchronous routine to load a scene.
    /// This allows for loading screens or fade transitions.
    /// </summary>
    private IEnumerator LoadSceneAsyncRoutine(string sceneName)
    {
        isLoading = true;
        
        Debug.Log($"Loading scene: {sceneName}");

        // --- OPTIONAL: Add fade-out logic here ---
        // e.g., yield return StartCoroutine(FadeCanvas.FadeOut(1.0f));

        // Start loading the new scene
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the scene is fully loaded
        while (!operation.isDone)
        {
            // You could update a loading bar here using operation.progress
            yield return null;
        }

        // --- OPTIONAL: Add fade-in logic here ---
        // e.g., yield return StartCoroutine(FadeCanvas.FadeIn(1.0f));

        isLoading = false;
    }
}