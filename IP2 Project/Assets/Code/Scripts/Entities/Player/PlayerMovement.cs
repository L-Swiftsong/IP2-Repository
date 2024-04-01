using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private Rigidbody2D _rb2D;
    [SerializeField] private Transform _rotationPivot;
    private Vector2 _movementInput;
    

    public static System.Action<float> OnStaminaChanged; // <float staminaPercentage>
    public static System.Action<float, float> OnStaminaValuesChanged; // <float maxStamina, float dashCost>


    [Header("Dashing")]
    [Tooltip("How fast the player travels when they dash.")]
        [SerializeField] private float _dashSpeed;
    [Tooltip("How far the player travels when they dash.")]
        [SerializeField] private float _dashDistance;
    private float _dashDurationRemaining;

    [Tooltip("How long the playuer needs to wait after their previous dash before they can dash again.")]
        [SerializeField] private float dashCooldown = 0.25f;
    private float dashCooldownRemaining;

    [Tooltip("How many times the player can dash if they started at full stamina.")]
        [SerializeField] private int _dashCount;
    private float _dashCost
    {
        get => _maxStamina / (float)_dashCount;
    }

    private bool _isDashing;

    public UnityEvent OnDashStarted;
    public UnityEvent OnDashCompleted;



    [Header("Dash Cost & Recharge")]
    [Tooltip("The player's maximum stamina (Used for Dashing).")]
        [SerializeField] private float _maxStamina = 100f;
    private float _stamina;

    [Tooltip("How much stamina the player regenerates in 1 second.")]
       [SerializeField] private float _dashRechargeRate;
    private Coroutine dashRechargeCoroutine;

    [Tooltip("How long after the player dashes until their stamina starts regenerating")]
        [SerializeField] private float _dashRechargeDelay = 1f;
    private float staminaRegenerationDelayRemaining;


    [Header("Debug")]
    [SerializeField] private bool _drawGizmos;
    [SerializeField] private Color _dashGizmoColour = Color.red;



    public void OnMovementInput(InputAction.CallbackContext context) => _movementInput = context.ReadValue<Vector2>().normalized;
    
    public void OnDashPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
            AttemptDash();
    }


    private void Start()
    {
        _stamina = _maxStamina;
        OnStaminaValuesChanged?.Invoke(_maxStamina, _dashCost);
    }

    void Update()
    {
        // If we are currently dashing, handle dashing stuff.
        if (_isDashing)
        {
            // Decrement the dash duration remaining.
            _dashDurationRemaining -= Time.deltaTime;

            // If we are to stop dashing, do so.
            if (_dashDurationRemaining <= 0f)
            {
                _isDashing = false;
                OnDashCompleted?.Invoke();
                dashCooldownRemaining = dashCooldown;
            }
        }
        // If we aren't dashing & our cooldown time hasn't elapsed, decrement the cooldown time remaining.
        else if (dashCooldownRemaining > 0f)
            dashCooldownRemaining -= Time.deltaTime;
    }
    void FixedUpdate() => Move();


    void Move()
    {
        // Don't process movement if we are dashing.
        if (_isDashing)
            return;
      
        if (gameObject.GetComponent<AbilityHolder>().activeTime <= 0 && gameObject.GetComponent<AbilityHolder>().ability.name == "TigerRush")
        {
            _rb2D.velocity = _movementInput * _moveSpeed;
        }

        if(gameObject.GetComponent<AbilityHolder>().ability.name != "TigerRush")
        {
            _rb2D.velocity = _movementInput * _moveSpeed;
        }
    }


    private void AttemptDash()
    {
        // If we cannot dash for whatever reason, stop here.
        if (_isDashing || dashCooldownRemaining > 0 || _stamina < _dashCost)
            return;

        // Start Dashing.
        StartDashing();
    }
    private void StartDashing()
    {
        Vector2 dashDirection = GetDashDirection();
        _rb2D.velocity = dashDirection * _dashSpeed;

        _dashDurationRemaining = _dashDistance / _dashSpeed; // From physics: 't = d/v'.
        _isDashing = true;

        // Notify subscribed scripts.
        OnDashStarted?.Invoke();

        // Update Stamina.
        _stamina -= _dashCost;
        OnStaminaChanged?.Invoke(_stamina / _maxStamina);

        // Recharge the player's stamina.
        if (dashRechargeCoroutine != null)
            StopCoroutine(dashRechargeCoroutine);
        dashRechargeCoroutine = StartCoroutine(RechargeStamina());
    }


    private IEnumerator RechargeStamina()
    {
        // Don't start regenerating stamina for staminaRegenerationDelay seconds.
        yield return new WaitForSeconds(_dashRechargeDelay);

        // Loop until we have fully regenerated our stamina.
        while (_stamina < _maxStamina)
        {
            // Increase our stamina, clamping to prevent exceeding our maximum.
            _stamina += _dashRechargeRate * Time.deltaTime;
            if (_stamina > _maxStamina)
            {
                _stamina = _maxStamina;
            }

            // Notify listeners that the stamina has changed.
            OnStaminaChanged?.Invoke(_stamina / _maxStamina);

            // Wait until the next frame.
            yield return null;
        }
    }


    // Used for TigerRush (To-Do: If we have time, find a better way).
    public Vector2 GetDashDirection() => (_movementInput != Vector2.zero ? _movementInput : (Vector2)_rotationPivot.up).normalized;

    private void OnDrawGizmosSelected()
    {
        if (!_drawGizmos)
            return;

        Gizmos.color = _dashGizmoColour;
        Gizmos.DrawRay(transform.position, transform.up * _dashDistance);
    }
}
