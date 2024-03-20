using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Player_Animation : MonoBehaviour
{
    [SerializeField] private Animator playerAnim;
    [SerializeField] private PlayerController _playerController;

    private Vector2 movementInput;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       


    }

    public void OnMovementInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>().normalized;
        playerAnim.SetFloat("XInput", movementInput.x);
        playerAnim.SetFloat("YInput", movementInput .y);

        

    }


}
