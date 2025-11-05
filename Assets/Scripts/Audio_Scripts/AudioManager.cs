using UnityEngine;
using UnityEngine.Audio; // For the mixer output

/// <summary>
/// A persistent singleton service for managing all global audio.
/// Lives on the _GameSceneManager persistent object.
/// Provides a central place for other scripts to call to play music and UI sounds.
/// </summary>
public class AudioManager : MonoBehaviour
{
    // Singleton pattern
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [Tooltip("The AudioSource for persistent music loops. (Output: Music Group)")]
    [SerializeField] private AudioSource musicSource;

    [Tooltip("The AudioSource for all non-diegetic UI sounds. (Output: UI Group)")]
    [SerializeField] private AudioSource uiSource;
    
    [Tooltip("The AudioSource for testing SFX, routed to the SFX mixer group. (Output: SFX Group)")]
    [SerializeField] private AudioSource sfxSource; 

    [Header("Audio Clips")]
    [Tooltip("The main music loop for the menu.")]
    public AudioClip mainMenuMusicLoop;
    
    [Tooltip("The sound to play on most button clicks.")]
    public AudioClip defaultClickSound;
    
    [Tooltip("The sound to play when opening a panel.")]
    public AudioClip openPanelSound;
    
    [Tooltip("The sound to play when closing a panel.")]
    public AudioClip closePanelSound;

    [Tooltip("The default SFX to play when testing the SFX slider.")]
    public AudioClip defaultSfxTestSound;
    

    void Awake()
    {
        // Set up the Singleton
        if (Instance == null)
        {
            Instance = this;
            // This object is already being managed by GameSceneManager's DontDestroyOnLoad
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates
        }
    }

    void Start()
    {
        // Start playing the menu music as soon as the game loads
        PlayMusic(mainMenuMusicLoop, true);
    }

    // --- Public Methods for Other Scripts ---

    /// <summary>
    /// Plays a UI sound one time.
    /// </summary>
    /// <param name="clipToPlay">The clip to play.</param>
    public void PlayUISound(AudioClip clipToPlay)
    {
        if (uiSource != null && clipToPlay != null)
        {
            uiSource.PlayOneShot(clipToPlay);
        }
        else if (uiSource == null)
        {
            Debug.LogWarning("AudioManager: No UI Source assigned!");
        }
    }

    /// <summary>
    /// Sets and plays a new music track.
    /// </summary>
    /// <param name="musicClip">The music clip to loop.</param>
    /// <param name="loop">Should the music loop?</param>
    public void PlayMusic(AudioClip musicClip, bool loop = true)
    {
        if (musicSource != null && musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.loop = loop;
            musicSource.Play();
        }
        else if (musicSource == null)
        {
            Debug.LogWarning("AudioManager: No Music Source assigned!");
        }
    }

    /// <summary>
    /// Stops the music.
    /// </summary>
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    // --- NEW PUBLIC FUNCTIONS FOR EVENT TRIGGERS ---

    /// <summary>
    /// Plays the default UI click sound.
    /// This is called by the UI Slider's EventTrigger (OnPointerDown).
    /// </summary>
    public void PlayDefaultClick()
    {
        PlayUISound(defaultClickSound);
    }

    /// <summary>
    /// Plays the default SFX test sound.
    /// This is called by the SFX Slider's EventTrigger (OnPointerUp).
    /// </summary>
    public void PlayTestSFX()
    {
        if (sfxSource != null && defaultSfxTestSound != null)
        {
            sfxSource.PlayOneShot(defaultSfxTestSound);
        }
        else if (sfxSource == null)
        {
            Debug.LogWarning("AudioManager: No SFX Source assigned!");
        }
    }
}