using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
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
    [SerializeField] private Image _currentPlayerOutlineColour;

    [Space(5)]
    [SerializeField] private Material _enemyOutline;
    [SerializeField] private Image _currentEnemyOutlineColour;
    
    [Space(5)]
    [SerializeField] private Material _interactableOutline;
    [SerializeField] private Image _currentInteractableOutlineColour;


    [Space(5)]
    [SerializeField] private OutlineEditorUI _outlineEditor;
    private ShaderType _selectedShaderType;


    [Space(10)]
    [SerializeField] private Material _aoeTotalRadiusMat;
    [SerializeField] private Image _currentAoeTotalRadiusColour;

    [SerializeField] private Material _aoeInnerRadiusMat;
    [SerializeField] private Image _currentAoeInnerRadiusColour;

    [SerializeField] private AoEEditorUI _aoeEditor;
    private bool _editingOuterRadius;




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

        #region Accessibility
        // Hide the outline selection box.
        _outlineEditor.gameObject.SetActive(false);


        // Set default outline indicator values.
        if (_playerOutline != null)
            _currentPlayerOutlineColour.color = _playerOutline.GetColor("_Outline_Colour");
        if (_enemyOutline != null)
            _currentEnemyOutlineColour.color = _enemyOutline.GetColor("_Outline_Colour");
        if (_interactableOutline != null)
            _currentInteractableOutlineColour.color = _interactableOutline.GetColor("_Outline_Colour");


        // Set the default AoE Colour Indicator values.
        SetAoERadiusColourIndicator(true);
        SetAoERadiusColourIndicator(false);
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

    public void SetBloom(bool newVal) => GameManager.Instance.UseBloom = newVal;
    #endregion


    #region Audio
    public void SetMainVolume(float newVolume) => _mixer.SetFloat("MasterVolume", newVolume);
    public void SetMusicVolume(float newVolume) => _mixer.SetFloat("MusicVolume", newVolume);
    public void SetSFXVolume(float newVolume) => _mixer.SetFloat("EffectsVolume", newVolume);
    #endregion


    #region Accessibility
    #region Outline Shader Colour and Thickness
    public void SetSelectedOutlineColour(Color newColour)
    {
        switch (_selectedShaderType)
        {
            case ShaderType.Player:
                if (_playerOutline != null)
                    _playerOutline.SetColor("_Outline_Colour", newColour);
                _currentPlayerOutlineColour.color = newColour;
                break;
            case ShaderType.Enemy:
                if (_enemyOutline != null)
                    _enemyOutline.SetColor("_Outline_Colour", newColour);
                _currentEnemyOutlineColour.color = newColour;
                break;
            case ShaderType.Interactable:
                if (_interactableOutline != null)
                    _interactableOutline.SetColor("_Outline_Colour", newColour);
                _currentInteractableOutlineColour.color = newColour;
                break;
        };
    }
    public void SetSelectedOutlineThickness(float newThickness)
    {
        switch (_selectedShaderType)
        {
            case ShaderType.Player:
                if (_playerOutline != null)
                    _playerOutline.SetFloat("_Outline_Thickness", newThickness);
                break;
            case ShaderType.Enemy:
                if (_enemyOutline != null)
                    _enemyOutline.SetFloat("_Outline_Thickness", newThickness);
                break;
            case ShaderType.Interactable:
                if (_interactableOutline != null)
                    _interactableOutline.SetFloat("_Outline_Thickness", newThickness);
                break;
        }
    }
    public void SetSelectedShader(ShaderType newVal) => _selectedShaderType = newVal;
    public void SetSelectedShader(int newVal) => _selectedShaderType = (ShaderType)newVal;
    
    public void OpenOutlineEditor()
    {
        // Enable the outline editor
        _outlineEditor.gameObject.SetActive(true);

        // Set the current colour & thickness of the outline editor to that of the selected outline.
        (Color Colour, float Thickness)? currentShaderValues = _selectedShaderType switch
        {
            ShaderType.Enemy => _enemyOutline != null ? (_enemyOutline.GetColor("_Outline_Colour"), _enemyOutline.GetFloat("_Outline_Colour")) : null,
            ShaderType.Interactable => _interactableOutline != null ? (_interactableOutline.GetColor("_Outline_Colour"), _interactableOutline.GetFloat("_Outline_Colour")) : null,
            _ => _playerOutline != null ? (_playerOutline.GetColor("_Outline_Colour"), _playerOutline.GetFloat("_Outline_Colour")) : null
        };
        _outlineEditor.SetColour(currentShaderValues.HasValue ? currentShaderValues.Value.Colour : Color.white);
        _outlineEditor.SetThickness(currentShaderValues.HasValue ? currentShaderValues.Value.Thickness : 1.0f);
    }
    public void CancelOutlineEditor() => _outlineEditor.gameObject.SetActive(false);
    public void ConfirmOutlineEditor(Color newColour, float newThickness)
    {
        // Update values.
        SetSelectedOutlineColour(newColour);
        SetSelectedOutlineThickness(newThickness);

        // Hide the editor.
        _outlineEditor.gameObject.SetActive(false);
    }
    #endregion


    public void SetUseSimplifiedText(bool newValue) => AccessibilityManager.Instance.UseSimplifiedFont = newValue;


    #region AoE Colours
    private void SetAoETotalRadiusColour(Color newColour)
    {
        _aoeTotalRadiusMat.color = newColour;
        SetAoERadiusColourIndicator(true);
    }
    private void SetAoEInnerRadiusColour(Color newColour)
    {
        _aoeInnerRadiusMat.color = newColour;
        SetAoERadiusColourIndicator(false);
    }

    private void SetAoERadiusColourIndicator(bool isOuter)
    {
        if (isOuter)
        {
            Color color = _aoeTotalRadiusMat.color;
            _currentAoeTotalRadiusColour.color = new Color(color.r, color.g, color.b);
        }
        else
        {
            Color color = _aoeInnerRadiusMat.color;
            _currentAoeInnerRadiusColour.color = new Color(color.r, color.g, color.b);
        }
    }
    

    public void OpenAoEEditor(bool editingTotalRadius)
    {
        // Enable the AoE editor
        _aoeEditor.gameObject.SetActive(true);

        // Set the value of _editingOuterRadius.
        _editingOuterRadius = editingTotalRadius;

        // Set the current colour of the outline editor to that of the selected radius.
        Color currentColour = _editingOuterRadius ? _aoeTotalRadiusMat.color : _aoeInnerRadiusMat.color;
        _aoeEditor.SetColour(currentColour);
    }
    public void CancelAoEEditor() => _aoeEditor.gameObject.SetActive(false);
    public void ConfirmAoEEditor(Color newColour)
    {
        // Update values.
        if (_editingOuterRadius)
            SetAoETotalRadiusColour(newColour);
        else
            SetAoEInnerRadiusColour(newColour);

        // Hide the editor.
        _aoeEditor.gameObject.SetActive(false);
    }

    public void ResetAoEColours()
    {
        // Outer.
        _aoeTotalRadiusMat.color = new Color(1f, 0f, 0f, 0.25f);
        SetAoERadiusColourIndicator(true);

        // Inner.
        _aoeInnerRadiusMat.color = new Color(1f, 1f, 1f, 0.6f);
        SetAoERadiusColourIndicator(false);
    }
    #endregion
    #endregion
}

[System.Serializable]
public enum ShaderType { None = -1, Player = 0, Enemy = 1, Interactable = 2 };