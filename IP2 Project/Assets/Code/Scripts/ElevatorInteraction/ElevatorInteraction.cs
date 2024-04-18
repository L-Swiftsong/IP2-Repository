using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ElevatorInteraction : MonoBehaviour
{
    [SerializeField] private float _interactionRadius;


    public void OnInteractPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
            AttemptInteraction();
    }


    void AttemptInteraction()
    {

    }
}
