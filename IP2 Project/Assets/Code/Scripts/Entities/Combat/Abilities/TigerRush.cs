using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class TigerRush : Ability
{


    [SerializeField] private Vector2 _extents;
    [SerializeField] private Vector2 _offset;
    public float rushVelocity;
    public float rushTime;
    public float circleRadius = 6f;

    public override void Activate(GameObject parent, Transform transform)
    {
        PlayerMovement movement = parent.GetComponent<PlayerMovement>();
        Rigidbody2D rigidbody = parent.GetComponent<Rigidbody2D>();
        Collider2D collider = parent.GetComponent<Collider2D>();

        

        if (activeTime > 0)
        {
            rigidbody.velocity = movement.movementInput.normalized * rushVelocity;

            
        }




    }

    
   
}

