using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimator : MonoBehaviour
{
    [SerializeField] private bool _subscribeToPlayer = false;

    [Space(10)]
    [SerializeField] private Transform _rotationPivot;

    [SerializeField] private float _facingRightLowerThreshold = 90f;
    [SerializeField] private float _facingRightUpperThreshold = 270f;
    private bool _isFacingRight => (_rotationPivot.eulerAngles.z < _facingRightLowerThreshold) || (_rotationPivot.eulerAngles.z > _facingRightUpperThreshold);


    [Space(5)]
    [SerializeField] private Transform _weaponParent;
    private GameObject[] _weaponInstances;
    private int _shownIndex;
    private Coroutine _showOriginalWeaponCoroutine;

    private Animator _activeWeaponAnimator;


    private void Awake()
    {
        if (_weaponInstances == null)
            _weaponInstances = new GameObject[0];
    }
    private void OnEnable()
    {
        if (_subscribeToPlayer)
        {
            PlayerAttacks.OnPrimaryWeaponChanged += OnPrimaryChanged;
            PlayerAttacks.OnSecondaryWeaponChanged += OnSecondaryChanged;
        }
    }
    private void OnDisable()
    {
        if (_subscribeToPlayer)
        {
            PlayerAttacks.OnPrimaryWeaponChanged -= OnPrimaryChanged;
            PlayerAttacks.OnSecondaryWeaponChanged -= OnSecondaryChanged;
        }
    }


    private void Update()
    {
        if (_activeWeaponAnimator != null)
        {
            _activeWeaponAnimator.SetBool("FacingRight", _isFacingRight);
        }
    }


    public void StartAttack(int weaponIndex, int comboIndex, float showDuration)
    {
        // Set this weapon as active if it is not already.
        SetActiveWeapon(weaponIndex, showDuration);

        // Play the attack animation (If the weaponInstance has an animator).
        if (_weaponInstances[weaponIndex].TryGetComponent<Animator>(out Animator weaponAnim))
        {
            weaponAnim.SetBool("FacingRight", _isFacingRight);
            weaponAnim.SetInteger("ComboIndex", comboIndex);
            weaponAnim.SetTrigger("Attack");
        }
        else
            Debug.Log(string.Format("Weapon {0} does not have an animator", weaponIndex));
    }


    private void SetActiveWeapon(int index, float duration = -1f)
    {
        // Stop any already running HideAfterDuration coroutines.
        if (_showOriginalWeaponCoroutine != null)
            StopCoroutine(_showOriginalWeaponCoroutine);


        // Show the weapon we wish to show.
        ShowWeapon(index);


        // If our duration is greater than 0, then we are only temporarily showing this weapon and should revert to the original.
        if (duration > 0f)
            _showOriginalWeaponCoroutine = StartCoroutine(ShowOriginalAfterDuration(duration));
        // Otherwise, this weapon is now the shown weapon.
        else
            _shownIndex = index;
    }
    private IEnumerator ShowOriginalAfterDuration(float duration)
    {
        // Wait until 'duration' time has elapsed.
        yield return new WaitForSeconds(duration);

        // Revert to the original shown weapon.
        ShowWeapon(_shownIndex);
    }
    private void ShowWeapon(int indexToShow)
    {
        // Disable all weaponInstances, but enable the one we wish to show.
        for (int i = 0; i < _weaponInstances.Length; i++)
        {
            _weaponInstances[i].SetActive(i == indexToShow);
        }

        // Set the active animator as this weapon's animator (If it has one & the child exists).
        _activeWeaponAnimator = _weaponInstances.Length > indexToShow ? _weaponInstances[indexToShow].GetComponent<Animator>() : null;
    }


    public void OnPrimaryChanged(Weapon newWeapon) => OnWeaponChanged(newWeapon, 0, true);
    public void OnSecondaryChanged(Weapon newWeapon) => OnWeaponChanged(newWeapon, 1);
    public void OnWeaponChanged(Weapon newWeapon, int index)
    {
        Debug.Log(newWeapon.name);
        OnWeaponChanged(newWeapon, index, false);
    }
    public void OnWeaponChanged(Weapon newWeapon, int index, bool makeShownWeapon)
    {
        // Ensure weaponInstances has been initialised.
        if (_weaponInstances == null)
            _weaponInstances = new GameObject[1];

        // If the _weaponInstances array is too small to fit the index, resize it to fit.
        if (_weaponInstances.Length < (index + 1))
            System.Array.Resize(ref _weaponInstances, index + 1);

        // Destroy the old instance.
        GameObject.Destroy(_weaponInstances[index]);

        // Instantiate the new instance.
        if (newWeapon.WeaponPrefab != null)
            _weaponInstances[index] = Instantiate<GameObject>(newWeapon.WeaponPrefab, _weaponParent);
        else
        {
            _weaponInstances[index] = new GameObject(newWeapon.name + " Blank Sprite");
        }


        // If this weapon should become the shown weapon, then make it so.
        if (makeShownWeapon || _weaponInstances.Length == 1)
            SetActiveWeapon(index);
    }
}