using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class PlayerAttacks : MonoBehaviour
{
    [Header("Primary Attacks")]
    [SerializeField] private WeaponWrapper _primaryWeapon;
    public bool _primaryAttackHeld;

    [Header("Combo")]
    [SerializeField] private int ComboCounter;
    int ComboCounter2;
    private int comboBefore;
    public float resetTime;
    public float actualReset;
    public Text ComboUI;
    int combo1;
    int combo2;
    int combo3;
    int combo4;
    int combo5;
    int combo6;

    bool Timer1;
    bool Timer2;
    bool Timer3;
    bool Timer4;
    bool Timer5;
    bool Timer6;

    [Header("Secondary Attacks")]
    [SerializeField] private WeaponWrapper _secondaryWeapon;
    public bool _secondaryAttackHeld;

    [SerializeField, Min(0)] private int _attackToDebug;


    [Header("Abilities")]
    [SerializeField] private Ability _currentAbility;
    private float _abilityCooldownComplete;
    private bool _useAbilityHeld;


    [Header("AoE Test")]
    [SerializeField] private Camera _playerCam;
    [SerializeField] private bool _throwToMouse;
    private Vector2 _mousePosition;

    [Header("Cooldown for ability")]
    [SerializeField] Image Coolbar;
    [SerializeField] float energy, maxEnergy;
    [SerializeField] float attackCost;
    [SerializeField] float chargeRate;
    private Coroutine recharge;

    [Header("Combo Counter Attacks")]
    public MeleeAttack GetMeleeAttack1;
    public MeleeAttack GetMeleeAttack2;
    public MeleeAttack GetMeleeAttack3;
    public RangedAttack GetRangedAttack1;
    public RangedAttack GetRangedAttack2;
    public AoEAttack GetAoEAttack;



    public void OnSecondaryAttack(InputAction.CallbackContext context)
    {
        /*if (context.started && energy == maxEnergy)
        {
            _secondaryAttackHeld = true;
            energy -= attackCost;
            Coolbar.fillAmount = energy / maxEnergy;

            if (recharge != null) StopCoroutine(recharge);
            recharge = StartCoroutine(RechargeAttack());
        }


        else if (context.canceled && energy != maxEnergy)
            _secondaryAttackHeld = false;*/
        if (context.started)
        {
            _secondaryAttackHeld = true;
            
        }
        else if (context.canceled)
            _secondaryAttackHeld = false;
    }
    public void OnPrimaryAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _primaryAttackHeld = true;
            
        }
        else if (context.canceled)
            _primaryAttackHeld = false;
    }

    
    public void OnUseAbilityPressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _useAbilityHeld = true;
            
        }

        else if (context.canceled)
            _useAbilityHeld = false;
    }
    public void GetMousePosition(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector3 worldPos = _playerCam.ScreenToWorldPoint(context.ReadValue<Vector2>());
            _mousePosition = worldPos;
        }
    }

    public void HitCombo()
    {
        ComboUI.text = "x" + ComboCounter;
    }


    private void Start()
    {

        actualReset = resetTime;
        if (_playerCam == null)
            _playerCam = Camera.main;
        
        _primaryWeapon = new WeaponWrapper(_primaryWeapon.Weapon, this);
        _secondaryWeapon = new WeaponWrapper(_secondaryWeapon.Weapon, this);
        GetMeleeAttack1.ComboMultiplier = 0;
        GetMeleeAttack2.ComboMultiplier = 0;
        GetMeleeAttack3.ComboMultiplier = 0;
        GetRangedAttack1.ComboMultiplier = 0;
        GetRangedAttack2.ComboMultiplier = 0;
        GetAoEAttack.ComboMultiplier = 0;

        Timer1 = GetMeleeAttack1.Timer;
        Timer2 = GetMeleeAttack2.Timer;
        Timer3 = GetMeleeAttack3.Timer;
        Timer4 = GetRangedAttack1.Timer;
        Timer5 = GetRangedAttack2.Timer;
        Timer6 = GetAoEAttack.Timer;

        combo1 = GetMeleeAttack1.ComboMultiplier;
        combo2 = GetMeleeAttack2.ComboMultiplier;
        combo3 = GetMeleeAttack3.ComboMultiplier;
        combo4 = GetRangedAttack1.ComboMultiplier;
        combo5 = GetRangedAttack2.ComboMultiplier;
        combo6 = GetAoEAttack.ComboMultiplier;



    }
    private void Update()
    {
        
        
        combo1 = GetMeleeAttack1.ComboMultiplier;
        combo2 = GetMeleeAttack2.ComboMultiplier;
        combo3 = GetMeleeAttack3.ComboMultiplier;
        combo4 = GetRangedAttack1.ComboMultiplier;
        combo5 = GetRangedAttack2.ComboMultiplier;
        combo6 = GetAoEAttack.ComboMultiplier;

        Timer1 = GetMeleeAttack1.Timer;
        Timer2 = GetMeleeAttack2.Timer;
        Timer3 = GetMeleeAttack3.Timer;
        Timer4 = GetRangedAttack1.Timer;
        Timer5 = GetRangedAttack2.Timer;
        Timer6 = GetAoEAttack.Timer;

        ComboCounter = ComboCounter + combo1 + combo2 + combo3 + combo4 + combo5 + combo6;
        ComboCounter2 = ComboCounter;
       
        if (Timer1 == true)
        {
            actualReset = resetTime;
            Timer1 = false;
            GetMeleeAttack1.Timer = false;
        }
        
        if (Timer2 == true)
        {
            actualReset = resetTime;
            Timer2 = false;
            GetMeleeAttack2.Timer = false;
        }
        
        if (Timer3 == true)
        {
            actualReset = resetTime;
            Timer3 = false;
            GetMeleeAttack3.Timer = false;
        }
        if (Timer4 == true)
        {
            actualReset = resetTime;
            Timer4 = false;
            GetRangedAttack1.Timer = false;
        }
        if (Timer5 == true)
        {
            actualReset = resetTime;
            Timer5 = false;
            GetRangedAttack2.Timer = false;
        }
        if(Timer6 == true)
        {
            actualReset = resetTime;
            Timer6 = false;
            GetAoEAttack.Timer = false;
        }


        if (ComboCounter >0)
        {
            actualReset -= Time.deltaTime;
        }

        

        if (actualReset <=0)
        {
            ComboCounter = 0;
            GetMeleeAttack1.ComboMultiplier = 0;
            GetMeleeAttack2.ComboMultiplier = 0;
            GetMeleeAttack3.ComboMultiplier = 0;
            GetRangedAttack1.ComboMultiplier = 0;
            GetRangedAttack2.ComboMultiplier = 0;
            GetAoEAttack.ComboMultiplier = 0;
            combo1 = GetMeleeAttack1.ComboMultiplier;
            combo2 = GetMeleeAttack2.ComboMultiplier;
            combo3 = GetMeleeAttack3.ComboMultiplier;
            combo4 = GetRangedAttack1.ComboMultiplier;
            combo5 = GetRangedAttack2.ComboMultiplier;
            combo6 = GetAoEAttack.ComboMultiplier;

            actualReset = resetTime;
        }

        GetMeleeAttack1.ComboMultiplier = 0;
        GetMeleeAttack2.ComboMultiplier = 0;
        GetMeleeAttack3.ComboMultiplier = 0;
        GetRangedAttack1.ComboMultiplier = 0;
        GetRangedAttack2.ComboMultiplier = 0;
        GetAoEAttack.ComboMultiplier = 0;





        // Abilities are checked before attacks.
        if (_useAbilityHeld && CanUseAbility())
        {
            UseAbility();
        }
        else if (_primaryAttackHeld)
        {
            AttemptAttack(_primaryWeapon);
        }

        else if (_secondaryAttackHeld)
        {
            AttemptAttack(_secondaryWeapon);
        }

        Coolbar.fillAmount = _secondaryWeapon.RechargePercentage;

        HitCombo();

        
    }

    private void AttemptAttack(WeaponWrapper weapon) => weapon.MakeAttack(_mousePosition, throwToMouse: _throwToMouse);


    public void EquipWeapon(Weapon newWeapon, bool replacePrimary = true)
    {
        WeaponWrapper newWrapper = new WeaponWrapper(newWeapon, this);

        if (replacePrimary)
            _primaryWeapon = newWrapper;
        else
            _secondaryWeapon = newWrapper;
    }


    private bool CanUseAbility()
    {
        if (_abilityCooldownComplete >= Time.time)
            return false;

        return true;
    }
    private void UseAbility()
    {
        Debug.Log("Used Ability");
        _abilityCooldownComplete = Time.time + _currentAbility.GetCooldownTime();
    }



    private void OnDrawGizmos()
    {
        if (_primaryWeapon.Weapon != null && _attackToDebug < _primaryWeapon.Weapon.Attacks.Length)
            _primaryWeapon.Weapon.Attacks[_attackToDebug].DrawGizmos(this.transform);

        else if (_secondaryWeapon.Weapon != null && _attackToDebug < _secondaryWeapon.Weapon.Attacks.Length)
            _secondaryWeapon.Weapon.Attacks[_attackToDebug].DrawGizmos(this.transform);
    }

    private IEnumerator RechargeAttack()
    {
        yield return new WaitForSeconds(1f);

        while (energy < maxEnergy)
        {
            energy += chargeRate / 10f;

            if (energy > maxEnergy)
            {
                energy = maxEnergy;
            }

            Coolbar.fillAmount = energy / maxEnergy;

            yield return new WaitForSeconds(.1f);


        }

    
        
    }

}
