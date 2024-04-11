using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int _maxHealth = 1;
    [SerializeField, ReadOnly] private int _currentHealth;
    public int _maxHealthProperty
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
            // Otherwise, call the OnHealingRecieved event to notify things such as UI without triggering negatives.
            else
                OnHealingReceived?.Invoke(new HealthChangedValues(_currentHealthProperty, value));
        }
    }
    public int _currentHealthProperty
    {
        get => _currentHealth;
        set
        {
            // Health Changed Event.
            HealthChangedValues healthChangedValues = new HealthChangedValues(_currentHealthProperty, value, _maxHealth);
            if (_currentHealthProperty > value)
                OnDamageTaken?.Invoke(healthChangedValues); // Old Health > New Health = Damage Taken.
            else
                OnHealingReceived?.Invoke(healthChangedValues); // Old Health < New Health = Healing Recieved. 


            // Clamp values between 0 & max.
            _currentHealth = Mathf.Clamp(value, 0, _maxHealthProperty);

            // If the health is 0, trigger the death event.
            if (_currentHealth <= 0 && !_isDead)
                Die();
        }
    }

    

    public UnityEvent<HealthChangedValues> OnHealingReceived;
    public UnityEvent<HealthChangedValues> OnDamageTaken;


    [Header("Invulnerability Frames")]
    [SerializeField] private float _iFrameDuration = 0f;
    [SerializeField, ReadOnly] private float _iFrameEndTime;


    [Header("Death")]
    public UnityEvent OnDeath;
    public static System.Action<Transform> OnDead; // Transform = this.
    private bool _isDead = false;



    public void Start()
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
        if (_iFrameDuration > 0)
            _iFrameEndTime = Time.time + _iFrameDuration;
    }
    #endregion


    #region Healing
#if UNITY_EDITOR
    [ContextMenu(itemName: "Heal")]
    private void RecieveHealing() => RecieveHealing(1);
#endif

    public void RecieveHealing(int healing = 1) => _currentHealthProperty += healing;
    #endregion


    #region Death
    private void Die()
    {
        _isDead = true;
        Debug.Log(this.name + " has died");

        OnDeath?.Invoke();
        OnDead?.Invoke(this.transform);
    }
    #endregion


    public float GetHealthPercentage() => (float)_currentHealthProperty / (float)_maxHealthProperty;
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