using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    public float moveSpeed;
    public Rigidbody2D rb;
    public bool sprint = false;

    private Vector2 movementInput;
    //private Vector2 moveDirection;


    public void OnMovementInput(InputAction.CallbackContext context) => movementInput = context.ReadValue<Vector2>().normalized;
    public void OnSprintPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveSpeed = 10f;
            sprint = true;
        }
        else if (context.canceled)
        {
            moveSpeed = 5f;
            sprint = false;
        }
    }


    //// Update is called once per frame
    //void Update()
    //{
    //    ProcessInputs();
    //}

    void FixedUpdate()
    {
        Move();
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
        rb.velocity = movementInput * moveSpeed;
    }
}
