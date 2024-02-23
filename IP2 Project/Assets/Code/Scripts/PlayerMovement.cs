using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    
    public float moveSpeed;
    public Rigidbody2D rb;
    public bool sprint = false;

    public float Stamina, MaxStamina;
    public static System.Action<float> OnStaminaChanged; // <float staminaPercentage>
    public static System.Action<float, float> OnStaminaValuesChanged; // <float maxStamina, float dashCost>

    public float DashCost;

    private Coroutine recharge;
    public float ChargeRate;

    private Vector2 movementInput;
    //private Vector2 moveDirection;

    private float activeMoveSpeed;
    public float dashSpeed;

    public float dashLength = .5f, dashCooldown = 1f;

    private float dashCounter;
    private float dashCoolCounter;
    bool isMoving;

    private void Start()
    {
        activeMoveSpeed = moveSpeed;
        isMoving = false;

        OnStaminaValuesChanged?.Invoke(MaxStamina, DashCost);
    }

    public void OnMovementInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>().normalized;

        if (context.started)
        {
            isMoving = true;
        }

        if (context.canceled)
        {
            isMoving = false;
        }
    }

  

    public void OnDashPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (dashCoolCounter <= 0 && dashCounter <= 0 && Stamina >= DashCost && isMoving == true)
            {
                activeMoveSpeed = dashSpeed;
                dashCounter = dashLength;
                Stamina -= DashCost;

                if (recharge != null)
                    StopCoroutine(recharge);
                recharge = StartCoroutine(RechargeStamina());
            }
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    void Update()
    {
        if (dashCounter > 0)
        {
            dashCounter -= Time.deltaTime;

            if (dashCounter <= 0)
            {
                activeMoveSpeed = moveSpeed;
                dashCoolCounter = dashCooldown;
            }
        }

        if (dashCoolCounter > 0)
        {
            dashCoolCounter -= Time.deltaTime;
        }
    }

    private IEnumerator RechargeStamina()
    {
        OnStaminaChanged?.Invoke(Stamina / MaxStamina);
        
        yield return new WaitForSeconds(1f);

        while (Stamina < MaxStamina)
        {
            Stamina += ChargeRate / 10f;

            if (Stamina > MaxStamina)
            {
                Stamina = MaxStamina;
            }

            OnStaminaChanged?.Invoke(Stamina / MaxStamina);

            yield return new WaitForSeconds(.1f);
        }
    }

    void Move()
    {
        rb.velocity = movementInput * activeMoveSpeed;
    }
}
