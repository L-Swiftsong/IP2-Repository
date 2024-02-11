using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int _maxHealth;
    [SerializeField, ReadOnly] private int _currentHealth;
    private int _maxHealthProperty
    {
        get => _maxHealth;
        set
        {
            // Set max health.
            _maxHealth = value;
            Debug.Log(this.name + " New Max Health: " + value);

            int healthChange = Mathf.Max(0, value);

            // Constrain current health if it is larger than the new max.
            if (_currentHealthProperty > value)
                _currentHealthProperty = value;
            // If current health is less than the new max, and our max has increased, increase our current health by the same amount.
            else if (healthChange > 0)
                _currentHealthProperty += healthChange;
            // Otherwise, call the OnHealthChanged event.
            else
                OnHealthChanged?.Invoke(new HealthChangedValues(_currentHealthProperty, value));
        }
    }
    private int _currentHealthProperty
    {
        get => _currentHealth;
        set
        {
            // Health Changed Event.
            OnHealthChanged?.Invoke(new HealthChangedValues());
            
            // Clamp values between 0 & max.
            _currentHealth = Mathf.Clamp(value, 0, _maxHealthProperty);
            Debug.Log(this.name + " New Health: " + _currentHealth);

            // If the health is 0, trigger the death event.
            if (_currentHealth <= 0 && !_isDead)
                Die();
        }
    }
    public UnityEvent<HealthChangedValues> OnHealthChanged;


    [Header("Invulnerability Frames")]
    [SerializeField] private float _iFrameDuration;
    private float _iFrameEndTime;


    [Header("Death")]
    public UnityEvent OnDeath;
    private bool _isDead = false;



    private void Start()
    {
        _currentHealthProperty = _maxHealthProperty;
        _iFrameEndTime = 0f;
    }


    #region Damage
#if UNITY_EDITOR
    [ContextMenu(itemName: "Damage")]
    private void TakeDamage() => TakeDamage(1);
#endif

    public void TakeDamage(int damage = 1)
    {
        // If we are currently invulnerable, then block the damage.
        if (_iFrameEndTime >= Time.time) {
            Debug.Log(this.name + " was damaged but is Invulnerable");
            return;
        }
        
        // Deal damage.
        _currentHealthProperty -= damage;


        // Set IFrames.
        _iFrameEndTime = Time.time + _iFrameDuration;
    }
    #endregion


    #region Healing
    public void RecieveHealing(int healing = 1) => _currentHealthProperty += healing;
    #endregion


    #region Death
    private void Die()
    {
        _isDead = true;
        Debug.Log(this.name + " has died");

        OnDeath?.Invoke();
    }
    #endregion
}

public struct HealthChangedValues
{
    public int OldHealth;
    public int NewHealth;

    public int NewMax;

    public HealthChangedValues(int currentHealth, int newMaxHealth)
    {
        this.OldHealth = currentHealth;
        this.NewHealth = currentHealth;

        this.NewMax = newMaxHealth;
    }
    public HealthChangedValues(int oldHealth, int newHealth, int maxHealth)
    {
        this.OldHealth = oldHealth;
        this.NewHealth = newHealth;

        this.NewMax = maxHealth;
    }
}