using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using TMPro;

// Note: Try keep useable for in both the TitleScreen & PauseMenu.
public class OptionsMenu : MonoBehaviour
{
    [Header("General")]


    [Header("Graphics")]
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    private Resolution[] _resolutions;

    [Space(5)]
    [SerializeField] private TMP_Dropdown _fullscreenDropdown;


    [Header("Audio")]
    [SerializeField] private AudioMixer _mixer;


    [Header("Accessibility")]
    [SerializeField] private Material _playerOutline;
    [SerializeField] private Material _enemyOutline;
    [SerializeField] private Material _interactableOutline;


    private void Start()
    {
        #region Graphics
        // Setup Fullscreen Dropdown.
        _fullscreenDropdown.ClearOptions();

        // Set the options of the fullscreen dropdown.
        _fullscreenDropdown.AddOptions(new List<string>() { "Windowed", "Fullscreen" });
        
        // Set the default value of the dropdown to match whether the screen is currently fullscreen.
        _fullscreenDropdown.value = Screen.fullScreen ? 1 : 0;



        // Setup Resolutions Dropdown.
        _resolutions = Screen.resolutions;

        _resolutionDropdown.ClearOptions();

        // Add all the available resolutions to a string list of options.
        List<string> dropdownOptions = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < _resolutions.Length; i++)
        {
            dropdownOptions.Add(_resolutions[i].width + " x " + _resolutions[i].height);

            // If this index is our current index, save that value. (Note: You cannot directly compare resolutions, hence the seperation of width and height).
            if ((_resolutions[i].width == Screen.currentResolution.width) && (_resolutions[i].height == Screen.currentResolution.height))
                currentResolutionIndex = i;
        }

        // Add the options from the list to the resolution dropdown.
        _resolutionDropdown.AddOptions(dropdownOptions);
        
        // Set our currently selected option to the current resolution.
        _resolutionDropdown.value = currentResolutionIndex;
        #endregion
    }


    #region General
    #endregion


    #region Graphics
    public void SetGraphicsQuality(int newIndex) => QualitySettings.SetQualityLevel(newIndex);
    public void SetFullscreenOption(int newIndex) => Screen.fullScreen = newIndex != 0; // Index 0 is Windowed, while 1 is Fullscreen.

    public void SetResolution(int newIndex)
    {
        Resolution newResolution = _resolutions[newIndex];
        Screen.SetResolution(newResolution.width, newResolution.height, Screen.fullScreen);
    }

    public void SetBloom(bool newVal) => Debug.Log("Bloom is " + (newVal ? "enabled" : "disabled"));
#endregion


#region Audio
    public void SetMainVolume(float newVolume) => _mixer.SetFloat("MasterVolume", newVolume);
    public void SetMusicVolume(float newVolume) => _mixer.SetFloat("MusicVolume", newVolume);
    public void SetSFXVolume(float newVolume) => _mixer.SetFloat("EffectsVolume", newVolume);
    #endregion


    #region Accessibility
    public void SetPlayerOutlineColour(Color newColour)
    {
        Debug.Log("Set Player Outline Colour: " + newColour);
    }
    public void SetPlayerOutlineThickness(float newThickness)
    {
        Debug.Log("Set Player Outline Thickness: " + newThickness);
    }

    public void SetEnemyOutlineColour(Color newColour)
    {
        Debug.Log("Set Enemy Outline Colour: " + newColour);
    }
    public void SetEnemyOutlineThickness(float newThickness)
    {
        Debug.Log("Set Enemy Outline Thickness: " + newThickness);
    }

    public void SetInteractableOutlineColour(Color newColour)
    {
        Debug.Log("Set Interactable Outline Colour: " + newColour);
    }
    public void SetInteractableOutlineThickness(float newThickness)
    {
        Debug.Log("Set Interactable Outline Thickness: " + newThickness);
    }


    public void SetUseSimplifiedText(bool newValue) => AccessibilityManager.Instance.UseSimplifiedFont = newValue;
    #endregion
}