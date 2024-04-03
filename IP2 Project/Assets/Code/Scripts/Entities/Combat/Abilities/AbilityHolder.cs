using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityHolder : MonoBehaviour
{
    private bool _isPressed = false;

    [SerializeField] private Ability _ability;
    public Ability Ability
    {
        get => _ability;
        set
        {
            _ability = value;
            OnAbilityChanged?.Invoke(_ability);
        }
    }


    private float _cooldownTime;
    private float _activeTime;
    
    [SerializeField] private float time = 0;
    DecptiveScreen clones;
    [ReadOnly]public int numberOfUses;

    public enum AbilityState
    {
        Ready,
        Active,
        Cooldown
    }
    private AbilityState state = AbilityState.Ready;


    // Accessors.
    public bool IsActive => _activeTime > 0f;


    public static System.Action<float> OnAbilityDurationRemainingChanged; // Called every frame while the ability is active.
    public static System.Action<float> OnAbilityCooldownRemainingChanged; // Called every frame while the ability is recharging.
    public static System.Action<Ability> OnAbilityChanged; // Called when the ability is changed.


    public void OnAbilityPressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isPressed = true;
        }
        else if(context.canceled)
        {
            _isPressed = false;
        }
    }

    // Initialise the ability UI.
    private void Start() => OnAbilityChanged?.Invoke(Ability);


    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case AbilityState.Ready:
                if(_isPressed == true)
                {
                    Ability.Activate(gameObject, transform);
                    state = AbilityState.Active;
                    _activeTime = Ability.activeTime;
                    numberOfUses += 1;
                    if(Ability.name == "DeceptiveScreen")
                    {
                        if ((int)gameObject.GetComponent<PlayerCombo>()._currentComboProperty == 0)
                        {
                            gameObject.GetComponent<PlayerCombo>().score += 300;
                        }
                        if ((int)gameObject.GetComponent<PlayerCombo>()._currentComboProperty > 0)
                        {
                            gameObject.GetComponent<PlayerCombo>().score += 300 * (int)gameObject.GetComponent<PlayerCombo>()._currentComboProperty;
                        }
                    }
                }
                break;
            case AbilityState.Active:
                // Call events for UI.
                float durationPercentageElapsed = 1f -Mathf.Clamp01(_activeTime / Ability.activeTime);
                OnAbilityDurationRemainingChanged?.Invoke(durationPercentageElapsed);


                if(_activeTime > 0)
                {
                    _activeTime -= Time.deltaTime;
                }
                else
                {
                    state = AbilityState.Cooldown;
                    _cooldownTime = Ability.cooldownTime;
                }
                break;
            case AbilityState.Cooldown:
                // Call events for UI.
                float cooldownPercentageElapsed = 1f - Mathf.Clamp01(_cooldownTime / Ability.cooldownTime);
                OnAbilityCooldownRemainingChanged?.Invoke(cooldownPercentageElapsed);
                

                if (_cooldownTime > 0)
                {
                    _cooldownTime -= Time.deltaTime;
                }
                else
                {
                    state = AbilityState.Ready;
                }
                break;
        }

        if(_activeTime <= 0 && Ability.name == "DragonBreath")
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(0).GetComponent<CircleCollider2D>().enabled = false;
            transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(255,255,255,1);
        }

        if(_activeTime > 0 && Ability.name == "DragonBreath")
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(0).GetComponent<CircleCollider2D>().enabled = true;
            transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(183, 0, 0, 1);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Ability.name == "TigerRush" && _activeTime > 0)
        {
            if (collision.TryGetComponent<EntityFaction>(out EntityFaction faction))
            {
                if(faction.IsAlly(Factions.Yakuza))
                {
                    if(collision.TryGetComponent<HealthComponent>(out HealthComponent health))
                    {   
                        health.TakeDamage();

                        if ((int)gameObject.GetComponent<PlayerCombo>()._currentComboProperty == 0)
                        {
                            gameObject.GetComponent<PlayerCombo>().score += 100;
                        }
                        if ((int)gameObject.GetComponent<PlayerCombo>()._currentComboProperty > 0)
                        {
                            gameObject.GetComponent<PlayerCombo>().score += 100 * (int)gameObject.GetComponent<PlayerCombo>()._currentComboProperty;
                        }
                    }
                }
            }
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        time += Time.deltaTime;
        
        if (Ability.name == "DragonBreath" && _activeTime > 0)
        {
            if (other.TryGetComponent<EntityFaction>(out EntityFaction faction))
            {
                if (faction.IsAlly(Factions.Yakuza))
                {
                    if (time >= 1)
                    {
                        if (other.TryGetComponent<HealthComponent>(out HealthComponent health))
                        {
                            health.TakeDamage();
                            time = 0;

                            if ((int)gameObject.GetComponent<PlayerCombo>()._currentComboProperty == 0)
                            {
                                gameObject.GetComponent<PlayerCombo>().score += 100;
                            }
                            if ((int)gameObject.GetComponent<PlayerCombo>()._currentComboProperty > 0)
                            {
                                gameObject.GetComponent<PlayerCombo>().score += 100 * (int)gameObject.GetComponent<PlayerCombo>()._currentComboProperty;
                            }
                        }
                    }
                }
            }
        }
    }
}
