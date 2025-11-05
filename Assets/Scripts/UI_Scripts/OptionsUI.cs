using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// Manages all logic for the Options Panel.
/// - Loads and saves volume settings to PlayerPrefs.
/// - Translates slider values (0-100) to decibel values for the AudioMixer.
/// </summary>
public class OptionsUI : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioMixer mainMixer;

    [Header("Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider uiSlider;

    // PlayerPrefs keys
    public const string MASTER_KEY = "MasterVolume";
    public const string MUSIC_KEY = "MusicVolume";
    public const string SFX_KEY = "SFXVolume";
    public const string UI_KEY = "UIVolume"; // <-- THE TYPO IS FIXED HERE

    // The decibel value for 0% volume.
    // -80dB is effectively silent.
    private const float MIN_DB = -80.0f; 
    
    // The decibel value for 100% volume.
    private const float MAX_DB = 0.0f;   
    
    // The default volume (70%) as a 0.0-1.0 value
    private const float DEFAULT_VOLUME_NORMALIZED = 0.7f; 

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// We use this for the initial, one-time setup of the mixer volumes.
    /// This runs even if the GameObject is disabled.
    /// </summary>
    void Awake()
    {
        // We only need to set the mixer values here.
        // The sliders will be set in OnEnable() when the panel is shown.
        // We get the saved value (0.0-1.0), or the 70% default, and set the mixer.
        mainMixer.SetFloat(MASTER_KEY, ConvertToDecibels(PlayerPrefs.GetFloat(MASTER_KEY, DEFAULT_VOLUME_NORMALIZED)));
        mainMixer.SetFloat(MUSIC_KEY, ConvertToDecibels(PlayerPrefs.GetFloat(MUSIC_KEY, DEFAULT_VOLUME_NORMALIZED)));
        mainMixer.SetFloat(SFX_KEY, ConvertToDecibels(PlayerPrefs.GetFloat(SFX_KEY, DEFAULT_VOLUME_NORMALIZED)));
        mainMixer.SetFloat(UI_KEY, ConvertToDecibels(PlayerPrefs.GetFloat(UI_KEY, DEFAULT_VOLUME_NORMALIZED)));
    }


    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// We use this to load settings and refresh the slider visuals every time the panel is opened.
    /// </summary>
    void OnEnable()
    {
        LoadAllVolumes();
    }

    /// <summary>
    /// Loads all volumes from PlayerPrefs and sets the sliders and mixer.
    /// </summary>
    private void LoadAllVolumes()
    {
        // --- Master Volume ---
        // Get the saved normalized value (0.0 to 1.0), defaulting to 0.7f (70%)
        float masterVolNormalized = PlayerPrefs.GetFloat(MASTER_KEY, DEFAULT_VOLUME_NORMALIZED);
        // Set the slider's value (0 to 100)
        masterSlider.value = masterVolNormalized * 100.0f;
        // Set the mixer's decibel value
        SetMixerVolume(MASTER_KEY, masterVolNormalized);

        // --- Music Volume ---
        float musicVolNormalized = PlayerPrefs.GetFloat(MUSIC_KEY, DEFAULT_VOLUME_NORMALIZED);
        musicSlider.value = musicVolNormalized * 100.0f;
        SetMixerVolume(MUSIC_KEY, musicVolNormalized);

        // --- SFX Volume ---
        float sfxVolNormalized = PlayerPrefs.GetFloat(SFX_KEY, DEFAULT_VOLUME_NORMALIZED);
        sfxSlider.value = sfxVolNormalized * 100.0f;
        SetMixerVolume(SFX_KEY, sfxVolNormalized);

        // --- UI Volume ---
        float uiVolNormalized = PlayerPrefs.GetFloat(UI_KEY, DEFAULT_VOLUME_NORMALIZED);
        uiSlider.value = uiVolNormalized * 100.0f;
        SetMixerVolume(UI_KEY, uiVolNormalized);
    }

    // --- Public Functions for Slider Events ---
    // These functions are called by the 'On Value Changed' event in the Slider component.
    // The 'sliderValue' parameter will be 0-100 from the slider.

    public void SetMasterVolume(float sliderValue)
    {
        // Convert 0-100 to 0.0-1.0
        float normalizedValue = sliderValue / 100.0f;
        
        // Set the mixer
        SetMixerVolume(MASTER_KEY, normalizedValue);
        
        // Save the normalized value
        PlayerPrefs.SetFloat(MASTER_KEY, normalizedValue);
    }

    public void SetMusicVolume(float sliderValue)
    {
        // Convert 0-100 to 0.0-1.0
        float normalizedValue = sliderValue / 100.0f;
        
        SetMixerVolume(MUSIC_KEY, normalizedValue);
        PlayerPrefs.SetFloat(MUSIC_KEY, normalizedValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
        // Convert 0-100 to 0.0-1.0
        float normalizedValue = sliderValue / 100.0f;
        
        SetMixerVolume(SFX_KEY, normalizedValue);
        PlayerPrefs.SetFloat(SFX_KEY, normalizedValue);
    }

    public void SetUIVolume(float sliderValue)
    {
        // Convert 0-100 to 0.0-1.0
        float normalizedValue = sliderValue / 100.0f;
        
        SetMixerVolume(UI_KEY, normalizedValue);
        PlayerPrefs.SetFloat(UI_KEY, normalizedValue);
    }

    /// <summary>
    /// Converts a normalized value (0.0 to 1.0) to decibels and sets the mixer parameter.
    /// </summary>
    /// <param name="parameterName">The exact name of the exposed mixer parameter.</param>
    /// <param name="normalizedValue">The volume level from 0.0 (mute) to 1.0 (max).</param>
    private void SetMixerVolume(string parameterName, float normalizedValue)
    {
        // Set the exposed parameter on the mixer
        mainMixer.SetFloat(parameterName, ConvertToDecibels(normalizedValue));
    }

    /// <summary>
    /// Helper function to convert a 0.0-1.0 volume scale to a decibel value.
    /// </summary>
    private float ConvertToDecibels(float normalizedValue)
    {
        // Use a logarithmic conversion for a natural-sounding volume curve.
        // We must clamp the value to avoid 0.0, because Log10(0) is -Infinity.
        float clampedValue = Mathf.Clamp(normalizedValue, 0.0001f, 1.0f);
        float dbValue = Mathf.Log10(clampedValue) * 20.0f;
        
        // Clamp the decibel value to our defined min/max range
        return Mathf.Clamp(dbValue, MIN_DB, MAX_DB);
    }
}