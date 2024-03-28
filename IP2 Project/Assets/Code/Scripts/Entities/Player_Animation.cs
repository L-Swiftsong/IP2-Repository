using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Player_Animation : MonoBehaviour
{
    [SerializeField] private Animator playerAnim;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Transform rotationPivot;

    private Vector2 movementInput;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(cursorPos.x, cursorPos.y);

        Vector2 direction = new Vector2(cursorPos.x - transform.position.x, cursorPos.y - transform.position.y);

        playerAnim.SetFloat("XInput", direction.x);
        playerAnim.SetFloat("YInput", direction.y);

        // Look up
        if (rotationPivot.transform.rotation.eulerAngles.z > -0.5f && rotationPivot.transform.rotation.z < 0.5f)
        {
            playerAnim.SetFloat("YInput", 1);
        }

        // Look right
        if (rotationPivot.transform.rotation.eulerAngles.z > -0.7f && rotationPivot.transform.rotation.z < -0.5f)
        {
            playerAnim.SetFloat("XInput", 1);
        }

        // Look down
        if (rotationPivot.transform.rotation.eulerAngles.z > 0.3f && rotationPivot.transform.rotation.z < 0.7f)
        {
            playerAnim.SetFloat("YInput", -1);
        }

        // Look left
        if (rotationPivot.transform.rotation.eulerAngles.z > 0.5f && rotationPivot.transform.rotation.z < -0.5f)
        {
            playerAnim.SetFloat("XInput", -1);
        }

        Debug.Log(rotationPivot.transform.rotation.z);
    }

    /*public void OnMovementInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>().normalized;
        playerAnim.SetFloat("XInput", movementInput.x);
        playerAnim.SetFloat("YInput", movementInput.y);

        

    }*/

    

}
