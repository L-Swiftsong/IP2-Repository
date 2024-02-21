using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    
    public float moveSpeed;
    public Rigidbody2D rb;
    public bool sprint = false;

    public Image StaminaBar;

    public float Stamina, MaxStamina;

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
            if (dashCoolCounter <=0 && dashCounter <=0 && Stamina >=50 && isMoving == true)
            {
                activeMoveSpeed = dashSpeed;
                dashCounter = dashLength;
                Stamina -= DashCost;
                StaminaBar.fillAmount = Stamina / MaxStamina;

                if (recharge != null)StopCoroutine(recharge);
                recharge = StartCoroutine(RechargeStamina());
                
            }
            
        }
       
       
    }


    //// Update is called once per frame
    //void Update()
    //{
    //    
    //}

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
        yield return new WaitForSeconds(1f);

        while (Stamina < MaxStamina)
        {
            Stamina += ChargeRate / 10f;

            if (Stamina > MaxStamina)
            {
                Stamina = MaxStamina;
            }

            StaminaBar.fillAmount = Stamina / MaxStamina;

            yield return new WaitForSeconds(.1f);

           
        }
    }
    //void ProcessInputs()
    //{
    //    float moveX = Input.GetAxisRaw("Horizontal");
    //    float moveY = Input.GetAxisRaw("Vertical");
    //    if(Input.GetKeyDown("left shift"))
    //    {
    //        moveSpeed = 10;
    //        sprint = true;
    //    }
    //    if (Input.GetKeyUp("left shift"))
    //    {
    //        moveSpeed = 5;
    //        sprint = false;
    //    }
    //    moveDirection = new Vector2(moveX, moveY).normalized;
    //}

    void Move()
    {
        rb.velocity = movementInput * activeMoveSpeed;
    }


}
