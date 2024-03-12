using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class UIElement : MonoBehaviour
{
    [SerializeField] private List<Image> _primaryRenderers;
    [SerializeField] private List<Image> _secondaryRenderers;
    
    private void Awake()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
#endif
        UpdateUIColours(UIManager.Instance.PrimaryColour, UIManager.Instance.SecondaryColour);

        // Subscribe to events.
        UIManager.OnUIColoursChanged += UpdateUIColours;
    }
    private void OnDestroy() => UIManager.OnUIColoursChanged += UpdateUIColours;


#if UNITY_EDITOR
    private void Update()
    {
        if (!Application.isPlaying && UIManager.Instance != null)
        {
            UpdateUIColours(UIManager.Instance.PrimaryColour, UIManager.Instance.SecondaryColour);
        }
    }
#endif


    private void UpdateUIColours(Color newPrimary, Color newSecondary)
    {
        if (UIManager.Instance == null)
        {
            Debug.LogWarning("WARNING: Persistent Scene not loaded or UIManager not initialised. Returning to prevent errors");
            return;
        }

        // Update primary colours.
        foreach (Image renderer in _primaryRenderers)
            renderer.color = newPrimary;

        // Update secondary colours.
        foreach (Image renderer in _secondaryRenderers)
            renderer.color = newSecondary;
    }
}
