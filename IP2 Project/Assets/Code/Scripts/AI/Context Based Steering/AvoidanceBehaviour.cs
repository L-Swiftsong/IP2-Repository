using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidanceBehaviour : BaseSteeringBehaviour
{
    [SerializeField] private float _detectionDistance;
    [SerializeField] private LayerMask _obstacleLayers;

    // The AvoidanceBehaviour behaviour does not return interest values.
    public override float[] GetInterestMap(Vector2 position, Vector2[] directions) => new float[directions.Length];

    // Calculate danger values based on the distance to the obstacle.
    public override float[] GetDangerMap(Vector2 position, Vector2[] directions)
    {
        float[] dangerMap = new float[directions.Length];

        // Loop through each potential target.
        foreach(Collider2D obstacle in Physics2D.OverlapCircleAll(position, _detectionDistance, _obstacleLayers))
        {
            // Cache values.
            Vector2 targetDirection = ((Vector2)obstacle.transform.position - position).normalized;
            float targetDistance = Vector2.Distance(obstacle.transform.position, position);


            // Loop through each direction.
            for (int i = 0; i < directions.Length; i++)
            {
                // Calculate the danger based on the dot product weighted between 0 & 1.
                //float danger = (1f + Vector2.Dot(targetDirection, directions[i])) / 2f;
                float danger = Vector2.Dot(targetDirection, directions[i]);

                // Weigh the danger based on distance & clamp between 0 & 1.
                danger = Mathf.Clamp01(danger * (1f - (danger * (targetDistance / _detectionDistance))));

                if (danger > dangerMap[i])
                    dangerMap[i] = danger;
            }
        }

        return dangerMap; 
    }
}