using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElement : MonoBehaviour
{
    [SerializeField] private List<Image> _primaryRenderers;
    [SerializeField] private List<Image> _secondaryRenderers;
    
    private void Awake()
    {
        UpdateUIColours();

        // Subscribe to events.
        UIManager.OnUIColoursChanged += UpdateUIColours;
    }
    private void OnDestroy() => UIManager.OnUIColoursChanged += UpdateUIColours;

    private void UpdateUIColours()
    {
        if (UIManager.Instance == null)
        {
            Debug.LogWarning("WARNING: Persistent Scene not loaded or UIManager not initialised.");
            return;
        }

        // Cache Values
        Color newPrimary = UIManager.Instance.PrimaryColour;
        Color newSecondary = UIManager.Instance.SecondaryColour;

        // Update primary colours.
        foreach (Image renderer in _primaryRenderers)
        {
            renderer.color = newPrimary;
            Debug.Log(renderer.color);
        }

        // Update secondary colours.
        foreach (Image renderer in _secondaryRenderers)
            renderer.color = newSecondary;
    }
}
