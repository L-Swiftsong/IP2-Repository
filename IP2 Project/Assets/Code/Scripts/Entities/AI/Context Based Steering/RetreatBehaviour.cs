using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Steering Behaviours/Retreat Behaviour", fileName = "New Retreat Behaviour")]
public class RetreatBehaviour : BaseSteeringBehaviour
{
    // The interest map points away from the target.
    public override float[] GetInterestMap(Rigidbody2D movementBody, Vector2 targetPos, Vector2[] directions)
    {
        float[] interestMap = new float[directions.Length];

        // Cache values.
        Vector2 targetDirection = (targetPos - movementBody.position).normalized;

        // Loop through each direction.
        for(int i = 0; i < directions.Length; i++)
        {
            // Calculate the dot product.
            float dot = Vector2.Dot(targetDirection, directions[i]);

            // Weight the dot away from the target. (Note: Will still have positive values near the direction of the target)
            dot = Mathf.Abs(1 - dot) / 2f;

            // Assign the dot to the interest map.
            interestMap[i] = dot;
        }

        return interestMap;
    }

    // The danger map points towards the target.
    public override float[] GetDangerMap(Rigidbody2D movementBody, Vector2 targetPos, Vector2[] directions)
    {
        float[] dangerMap = new float[directions.Length];

        // Cache Values.
        Vector2 targetDirection = (targetPos - movementBody.position).normalized;

        // Loop through each direction.
        for(int i = 0; i < directions.Length; i++)
        {
            // Calculate the dot product.
            float dot = Vector2.Dot(targetDirection, directions[i]);

            // Clamp the dot product between 0 & 1 then weight it towards values closer to 1.
            dot = Mathf.Clamp01(dot);
            dot *= dot;

            // Assign the weighted dot to the danger map.
            dangerMap[i] = dot;
        }

        return dangerMap;
    }
}