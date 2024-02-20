using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Steering Behaviours/Strafe", fileName = "New Strafe Behaviour")]
public class StrafeBehaviour : BaseSteeringBehaviour
{
    public override float[] GetInterestMap(Rigidbody2D movementBody, Vector2 targetPos, Vector2[] directions)
    {
        float[] interestMap = new float[directions.Length];
        
        // Cache Values.
        Vector2 targetDirection = (targetPos - movementBody.position).normalized;
        

        // Loop through each direction.
        for(int i = 0; i < directions.Length; i++)
        {
            // Calculate the dot product.
            float dot = Vector2.Dot(targetDirection, directions[i]);
            bool toTheRight = Vector2.Dot(Vector2.Perpendicular(targetDirection), directions[i]) < 0;
            Debug.Log("Direction: " + directions[i] + " is " + (toTheRight ? "the Right" : "the Left"));

            // Weight the dot product to face the sides.
            float weightedDot = Mathf.Clamp01(1f - Mathf.Abs(dot));
            // Strafe only in the same direction as our current velocity.
            weightedDot *= Vector2.Dot(movementBody.velocity.normalized, directions[i]) > 0f ? 1f : -1f;

            // Assign our weighted dot as the interest.
            if (weightedDot > interestMap[i])
                interestMap[i] = weightedDot;
        }

        return interestMap;
    }

    // Directions leading towards the target will be mapped to the danger map.
    public override float[] GetDangerMap(Rigidbody2D movementBody, Vector2 targetPos, Vector2[] directions) => new float[directions.Length];
}