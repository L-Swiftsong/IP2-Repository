using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombo : MonoBehaviour
{
    [Header("References")]
    private Transform _playerTransform;


    [Header("Combo Values")]
    [SerializeField] private float _maxCombo;
    [SerializeField, ReadOnly] private float _currentCombo;
    private float _currentComboProperty
    {
        get => _currentCombo;
        set
        {
            _currentCombo = Mathf.Clamp(value, 0f, _maxCombo);

            if (value != 0)
            {
                if (_comboResetCoroutine != null)
                    StopCoroutine(_comboResetCoroutine);
                _comboResetCoroutine = StartCoroutine(ResetCombo());
            }
        }
    }
    
    [SerializeField] private float _comboResetTime;
    [SerializeField, ReadOnly] private float _comboResetTimeRemaining;
    private Coroutine _comboResetCoroutine;



    private void Awake() => _playerTransform = this.transform;
    public void OnEnable() => HealthComponent.OnDead += IncrementScore;
    public void OnDisable() => HealthComponent.OnDead -= IncrementScore;


    private void IncrementScore(Transform deadEntity)
    {
        // Don't increment the combo if the dead entity is the player.
        if (deadEntity == _playerTransform)
            return;

        // Ensure that the deadEntity is one valid to increment (It has a Entity Faction).
        if (deadEntity.GetComponent<EntityFaction>() == null)
            return;
        


        // Increment the combo.
        _currentComboProperty += 1f;
    }
    private IEnumerator ResetCombo()
    {
        // Decrement combo reset time remaining.
        _comboResetTimeRemaining = _comboResetTime;
        while(_comboResetTimeRemaining > 0f)
        {
            yield return null;
            _comboResetTimeRemaining -= Time.deltaTime;
        }

        // Reset the combo.
        _currentCombo = 0f;
    }
}
