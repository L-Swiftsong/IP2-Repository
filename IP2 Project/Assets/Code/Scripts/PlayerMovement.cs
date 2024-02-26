using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private Rigidbody2D rb;
    private Vector2 movementInput;

    public static System.Action<float> OnStaminaChanged; // <float staminaPercentage>
    public static System.Action<float, float> OnStaminaValuesChanged; // <float maxStamina, float dashCost>


    [Header("Dashing")]
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _dashDistance;
    private float _dashDurationRemaining;

    [SerializeField] private float dashCooldown = 0.25f;
    private float dashCooldownRemaining;
    [SerializeField] private float DashCost;

    private bool _isDashing;


    [Header("Stamina")]
    [SerializeField] private float Stamina;
    [SerializeField] private float MaxStamina;

    [Space(5)]
    [SerializeField] private float ChargeRate;
    private Coroutine dashRechargeCoroutine;

    [SerializeField] private float staminaRegenerationDelay = 1f;
    private float staminaRegenerationDelayRemaining;


    [Header("Animations")]
    [SerializeField] private AnimationController _animationController;
    [SerializeField] private AnimationContainerSO _dashAnim;


    [Header("Debug")]
    [SerializeField] private bool _drawGizmos;
    [SerializeField] private Color _dashGizmoColour = Color.red;



    public void OnMovementInput(InputAction.CallbackContext context) => movementInput = context.ReadValue<Vector2>().normalized;
    
    public void OnDashPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
            AttemptDash();
    }


    private void Start()
    {
        // Call stamina changed event to show the default values.
        OnStaminaValuesChanged?.Invoke(MaxStamina, DashCost);

        // Find the animation controller if we don't already have it.
        if (_animationController == null)
            _animationController = GetComponent<AnimationController>();
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
                StopDashing();
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

        // Move by setting the player's velocity.
        rb.velocity = movementInput * moveSpeed;

        // Play animations.
        if (movementInput == Vector2.zero)
            _animationController?.PlayIdle();
        else
            _animationController.PlayRunning();
    }


    private void AttemptDash()
    {
        // If we cannot dash for whatever reason, stop here.
        if (_isDashing || dashCooldownRemaining > 0 || Stamina < DashCost)
            return;

        
        // Start Dashing.
        Vector2 dashDirection = (movementInput != Vector2.zero ? movementInput : (Vector2)transform.up).normalized;
        rb.velocity = dashDirection * _dashSpeed;

        _dashDurationRemaining = _dashDistance / _dashSpeed; // From physics: 't = d/v'.
        _isDashing = true;


        // Attempt to call Anim.
        _animationController?.SetNextAnimation(_dashAnim, softOverridable: false, hardOverride: true, loops: true, isTemporary: false, customRevertCondition: () => !_isDashing);


        // Update Stamina.
        Stamina -= DashCost;
        OnStaminaChanged?.Invoke(Stamina / MaxStamina);

        // Recharge the player's stamina.
        if (dashRechargeCoroutine != null)
            StopCoroutine(dashRechargeCoroutine);
        dashRechargeCoroutine = StartCoroutine(RechargeStamina());
    }
    private void StopDashing()
    {
        _isDashing = false;
        dashCooldownRemaining = dashCooldown;

        // Attempt to stop the dash animation.
        _animationController?.StopCurrentAnimation();
    }


    private IEnumerator RechargeStamina()
    {
        // Don't start regenerating stamina for staminaRegenerationDelay seconds.
        yield return new WaitForSeconds(staminaRegenerationDelay);

        // Loop until we have fully regenerated our stamina.
        while (Stamina < MaxStamina)
        {
            // Increase our stamina, clamping to prevent exceeding our maximum.
            Stamina += ChargeRate * Time.deltaTime;
            if (Stamina > MaxStamina)
            {
                Stamina = MaxStamina;
            }

            // Notify listeners that the stamina has changed.
            OnStaminaChanged?.Invoke(Stamina / MaxStamina);

            // Wait until the next frame.
            yield return null;
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (!_drawGizmos)
            return;

        Gizmos.color = _dashGizmoColour;
        Gizmos.DrawRay(transform.position, transform.up * _dashDistance);
    }
}
