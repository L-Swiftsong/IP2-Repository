using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAbilityUI : MonoBehaviour
{
    [SerializeField] private ProgressBar _abilityDurationBar;
    [SerializeField] private ProgressBar _abilityCooldownBar;

    [Space(5)]
    [SerializeField] private Image _currentAbilityImage;


    private void OnEnable()
    {
        // Subscribe to Events.
        AbilityHolder.OnAbilityDurationRemainingChanged += OnDurationChanged;
        AbilityHolder.OnAbilityCooldownRemainingChanged += OnCooldownChanged;
        AbilityHolder.OnAbilityChanged += SwapAbilityUI;


        // Hide the Cooldown Bar.
        StartCoroutine(HideCooldownBarAfterFrame());
    }
    private void OnDisable()
    {
        // Unsubscribe to Events.
        AbilityHolder.OnAbilityDurationRemainingChanged -= OnDurationChanged;
        AbilityHolder.OnAbilityCooldownRemainingChanged -= OnCooldownChanged;
        AbilityHolder.OnAbilityChanged -= SwapAbilityUI;
    }


    private IEnumerator HideCooldownBarAfterFrame()
    {
        yield return null;
        _abilityCooldownBar.SetValues(0f, 1f, 0f);
    }


    // Update the Duration Bar.
    private void OnDurationChanged(float elapsedPercent) => _abilityDurationBar.SetValues(1f - elapsedPercent, 1f, 0f);

    private void OnCooldownChanged(float elapsedPercent)
    {
        // Update the cooldown bar.
        if (elapsedPercent != 1)
            _abilityCooldownBar.SetValues(elapsedPercent, 1f, 0f);
        else
            _abilityCooldownBar.SetValues(0f, 1f, 0f);

        // Update the Duration Bar.
        _abilityDurationBar.SetValues(elapsedPercent, 1f, 0f);
    }

    // Update the Ability's Image.
    private void SwapAbilityUI(Ability newAbility)
    {
        if (_currentAbilityImage.sprite == null || newAbility.AbilitySprite != null)
            _currentAbilityImage.sprite = newAbility.AbilitySprite;
    }
}
