using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrafeBehaviour : BaseSteeringBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _strafeDistance;


    public override float[] GetInterestMap(Vector2 position, Vector2[] directions)
    {
        float[] interestMap = new float[directions.Length];
        
        // Cache Values.
        Vector2 targetDirection = ((Vector2)_target.position - position).normalized;

        // Loop through each direction.
        for(int i = 0; i < directions.Length; i++)
        {
            // Calculate the dot product.
            float dot = Vector2.Dot(targetDirection, directions[i]);

            // Weight the dot product to face the sides.
            float weightedDot = 1f - Mathf.Abs(dot);

            // Assign our weighted dot as the interest.
            interestMap[i] = weightedDot;
        }

        return interestMap;
    }

    // Directions leading towards the target will be mapped to the danger map.
    public override float[] GetDangerMap(Vector2 position, Vector2[] directions)
    {
        float[] dangerMap = new float[directions.Length];

        // Cache Values.
        Vector2 targetDirection = ((Vector2)_target.position - position).normalized;
        float targetDistance = Vector2.Distance(_target.position, position);


        // Loop through each direction.
        for(int i = 0; i < directions.Length; i++)
        {
            // If we are too close, discourage moving closer.
            if (targetDistance < _strafeDistance)
            {
                // Discourage moving towards (Dot > 0)
                float dot = Vector2.Dot(targetDirection, directions[i]);
                float distanceWeight = (1f - targetDistance / _strafeDistance);
                //float weightedDot = (1f - Mathf.Abs(dot - 0.65f)) * distanceWeight;
                dot *= -1;
                float weightedDot = Mathf.Pow((1f - dot) / 2f, 2f) * (distanceWeight * 2f);

                dangerMap[i] = weightedDot;
            }
            // If we are too far, discourage moving away.
            else
            {
                // Discourage moving away (Dot < 0)
                float dot = -Vector2.Dot(targetDirection, directions[i]);
                float distancePercent = (targetDistance - _strafeDistance) / _strafeDistance;
                
                dangerMap[i] = dot * distancePercent;
            }
        }

        return dangerMap;
    }
}