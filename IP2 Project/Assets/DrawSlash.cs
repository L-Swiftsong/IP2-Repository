using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DrawSlash : MonoBehaviour
{
    [SerializeField] private GameObject slashGizmo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPrimaryAttack(InputAction.CallbackContext context)
    {
        if (context.started)
           slashGizmo.SetActive(true);
        else if (context.canceled)
            slashGizmo.SetActive(false);
    }
}
