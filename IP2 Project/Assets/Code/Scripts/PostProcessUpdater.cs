using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Volume))]
public class PostProcessUpdater : MonoBehaviour
{
    private Volume _volume;

    private Bloom _bloom;

    private void Start()
    {
        // Get the post process volume.
        _volume = GetComponent<Volume>();

        // Get the components of the Post Process Volume.
        _volume.profile.TryGet<Bloom>(out _bloom);


        // Subscribe to GameManager Events.
        GameManager.OnPostProcessSettingsChanged += UpdateVolumeParameters;


        // Set default values of the volume.
        UpdateVolumeParameters();
    }

    private void OnDestroy() => GameManager.OnPostProcessSettingsChanged -= UpdateVolumeParameters;


    private void UpdateVolumeParameters()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("WARNING: Game Manager not initialised. Returning to prevent errors");
            return;
        }
        
        // Update the Post Process Volumes.
        UpdateBloom(GameManager.Instance.UseBloom);
    }


    private void UpdateBloom(bool newVal) => _bloom.active = newVal;
}
