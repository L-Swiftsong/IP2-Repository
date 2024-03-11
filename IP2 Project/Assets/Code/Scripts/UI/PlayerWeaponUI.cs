using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponUI : MonoBehaviour
{
    [Header("Primary Weapon UI")]
    [SerializeField] private ProgressBar _primaryWeaponRecoveryBar;
    [SerializeField] private ProgressBar _primaryWeaponUseCooldownBar;

    [Space(5)]
    [SerializeField] private Image _primaryWeaponImage;


    [Header("Secondary Weapon UI")]
    [SerializeField] private ProgressBar _secondaryWeaponRecoveryBar;
    [SerializeField] private ProgressBar _secondaryWeaponUseCooldownBar;

    [Space(5)]
    [SerializeField] private Image _secondaryWeaponImage;



    private void OnEnable() // Subscribe to events.
    {
        // Primary Weapon.
        PlayerAttacks.OnPrimaryRecoveryTimeChanged += UpdatePrimaryRecoveryBar;
        PlayerAttacks.OnPrimaryUseRechargeTimeChanged += UpdatePrimaryCooldownBar;
        PlayerAttacks.OnPrimaryWeaponChanged += SwapPrimaryUI; // (Image newSprite) VS (Weapon newWeapon)?


        // Secondary Weapon.
        PlayerAttacks.OnSecondaryRecoveryTimeChanged += UpdateSecondaryRecoveryBar;
        PlayerAttacks.OnSecondaryUseRechargeTimeChanged += UpdateSecondaryCooldownBar;
        PlayerAttacks.OnSecondaryWeaponChanged += SwapSecondaryUI; // (Image newSprite) VS (Weapon newWeapon)?
    }
    private void OnDisable() // Unsubscribe to events.
    {
        // Primary Weapon.
        PlayerAttacks.OnPrimaryRecoveryTimeChanged -= UpdatePrimaryRecoveryBar;
        PlayerAttacks.OnPrimaryUseRechargeTimeChanged -= UpdatePrimaryCooldownBar;
        PlayerAttacks.OnPrimaryWeaponChanged -= SwapPrimaryUI; // (Image newSprite) VS (Weapon newWeapon)?


        // Secondary Weapon.
        PlayerAttacks.OnSecondaryRecoveryTimeChanged -= UpdateSecondaryRecoveryBar;
        PlayerAttacks.OnSecondaryUseRechargeTimeChanged -= UpdateSecondaryCooldownBar;
        PlayerAttacks.OnSecondaryWeaponChanged -= SwapSecondaryUI; // (Image newSprite) VS (Weapon newWeapon)?
    }


    // Primary Weapon.
    private void UpdatePrimaryRecoveryBar(float newPercent)
    {
        if (_primaryWeaponRecoveryBar != null)
            _primaryWeaponRecoveryBar.SetValues(newPercent, 1f, 0f);
    }
    private void UpdatePrimaryCooldownBar(float newPercent)
    {
        if (_primaryWeaponUseCooldownBar != null)
            _primaryWeaponUseCooldownBar.SetValues(1f - newPercent, 1f, 0f);
    }
    private void SwapPrimaryUI(Weapon newWeapon)
    {
        if (_primaryWeaponImage != null && newWeapon.WeaponSprite != null)
            _primaryWeaponImage.sprite = newWeapon.WeaponSprite;
    }


    // Secondary Weapon.
    private void UpdateSecondaryRecoveryBar(float newPercent)
    {
        if (_secondaryWeaponRecoveryBar != null)
            _secondaryWeaponRecoveryBar.SetValues(newPercent, 1f, 0f);
    }
    private void UpdateSecondaryCooldownBar(float newPercent)
    {
        if (_secondaryWeaponUseCooldownBar != null)
            _secondaryWeaponUseCooldownBar.SetValues(1f - newPercent, 1f, 0f);
    }
    private void SwapSecondaryUI(Weapon newWeapon)
    {
        if (_secondaryWeaponImage != null && newWeapon.WeaponSprite != null)
            _secondaryWeaponImage.sprite = newWeapon.WeaponSprite;
    }
}
