using UnityEngine;
using UnityEngine.Audio; // We'll need this

/// <summary>
/// Sits on a UI Manager object in the MainMenuScene.
/// Manages top-level UI panels and provides button functions.
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("Panel References")]
    [Tooltip("The panel containing Play, Options, Exit buttons.")]
    [SerializeField] private GameObject mainPanel;

    [Tooltip("The panel containing options like volume sliders.")]
    [SerializeField] private GameObject optionsPanel;

    [Header("Audio")] // --- NEW SECTION ---
    [Tooltip("The AudioSource that plays UI sounds.")]
    [SerializeField] private AudioSource uiAudioSource;
    [Tooltip("The sound to play on most button clicks.")]
    [SerializeField] private AudioClip defaultClickSound;
    [Tooltip("The sound to play when opening the options panel.")]
    [SerializeField] private AudioClip openPanelSound;
    [Tooltip("The sound to play when closing the options panel.")]
    [SerializeField] private AudioClip closePanelSound;


    private GameSceneManager gameSceneManager;

    void Start()
    {
        // Find the GameSceneManager
        gameSceneManager = GameSceneManager.Instance;

        if (gameSceneManager == null)
        {
            Debug.LogError("MainMenuUI: Could not find GameSceneManager.Instance!");
        }

        // --- NEW ---
        if (uiAudioSource == null)
        {
            // Try to find it on this same object
            uiAudioSource = GetComponent<AudioSource>();
            if (uiAudioSource == null)
            {
                Debug.LogWarning("MainMenuUI: No UI AudioSource assigned or found!");
            }
        }

        // Ensure correct panel is active on start
        mainPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    /// <summary>
    /// Public method to be linked to the Play Button's OnClick event.
    /// </summary>
    public void OnPlayButtonPressed()
    {
        PlaySound(defaultClickSound); // --- NEW ---
        if (gameSceneManager != null)
        {
            gameSceneManager.LoadGame();
        }
    }

    /// <summary>
    /// Public method to be linked to the Options Button's OnClick event.
    /// This now swaps panels instead of loading a scene.
    /// </summary>
    public void OnOptionsButtonPressed()
    {
        PlaySound(openPanelSound); // --- NEW ---
        mainPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    /// <summary>
    /// Public method to be linked to the Exit Button's OnClick event.
    /// </summary>
    public void OnExitButtonPressed()
    {
        PlaySound(defaultClickSound); // --- NEW ---
        if (gameSceneManager != null)
        {
            gameSceneManager.QuitApplication();
        }
    }

    /// <summary>
    /// Public method to be linked to the Options Panel's 'Back' Button.
    /// </summary>
    public void OnOptionsBackButtonPressed()
    {
        PlaySound(closePanelSound); // --- NEW ---
        // You might also want to call optionsUI.ApplySettings() here
        optionsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    // --- NEW FUNCTION ---
    /// <summary>
    /// Plays a one-shot audio clip through the assigned UI AudioSource.
    /// </summary>
    /// <param name="clipToPlay">The audio clip to play.</param>
    private void PlaySound(AudioClip clipToPlay)
    {
        if (uiAudioSource != null && clipToPlay != null)
        {
            uiAudioSource.PlayOneShot(clipToPlay);
        }
    }
}