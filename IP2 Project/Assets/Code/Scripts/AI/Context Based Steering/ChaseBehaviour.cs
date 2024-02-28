using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Steering Behaviours/Chase", fileName = "New Chase Behaviour")]
public class ChaseBehaviour : BaseSteeringBehaviour
{
    [SerializeField] private float _minChaseDistance = 0.5f;

    
    // Return a interest map based on the direction to the targets.
    public override float[] GetInterestMap(Rigidbody2D movementBody, Vector2 targetPos, Vector2[] directions)
    {
        float[] interestMap = new float[directions.Length];

        // Cache values.
        Vector2 targetDirection = (targetPos - movementBody.position).normalized;
        float distanceToTarget = Vector2.Distance(targetPos, movementBody.position);

        // Ignore targets that are too close.
        if (distanceToTarget < _minChaseDistance)
            return interestMap;

        // Loop through each direction we should consider.
        for(int i = 0; i < directions.Length; i++)
        {
            // Calculate the interest towards this point based on the dot product scaled to a range of 0-1.
            float interest = Vector2.Dot(targetDirection, directions[i]);

            // If this interest is the largest in this direction slot, then assign it.
            if (interest > interestMap[i])
                interestMap[i] = interest;
        }
        

        // Return our calculated interest map.
        return interestMap;
    }

    // The ChaseBehaviour behaviour returns no danger map.
    public override float[] GetDangerMap(Rigidbody2D movementBody, Vector2 targetPos, Vector2[] directions) => new float[directions.Length];
    
}