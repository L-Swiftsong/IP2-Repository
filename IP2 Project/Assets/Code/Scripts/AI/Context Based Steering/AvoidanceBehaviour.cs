using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Steering Behaviours/Avoidance", fileName = "New Avoidance Behaviour")]
public class AvoidanceBehaviour : BaseSteeringBehaviour
{
    [SerializeField] private float _detectionDistance;
    [SerializeField] private LayerMask _obstacleLayers;

    [Space(5)]
    [SerializeField] private float _maxMagnitudeRadius;


    // The AvoidanceBehaviour behaviour does not return interest values.
    public override float[] GetInterestMap(Vector2 position, Vector2 targetPos, Vector2[] directions)
    {
        float[] interestMap = new float[directions.Length];

        /*// Loop through each potential target.
        foreach (Collider2D obstacle in Physics2D.OverlapCircleAll(position, _detectionDistance, _obstacleLayers))
        {
            // Cache values.
            Vector2 closestPoint = obstacle.ClosestPoint(position);
            Vector2 targetDirection = (closestPoint - position).normalized;
            float targetDistance = Vector2.Distance(closestPoint, position);

            // Discount targets that are a further distance than the maxMagnitudeRadius.
            if (targetDistance > _maxMagnitudeRadius)
                continue;


            // Calculate the weight applied to the weights, inversely proportional to the distance to the target.
            float targetWeight = 1f - (targetDistance / _maxMagnitudeRadius);

            // Loop through each direction.
            for (int i = 0; i < directions.Length; i++)
            {
                // Calculate the interest by inverting the dot product and clamping between 0 & 1.
                //float dot = Mathf.Clamp01(-Vector2.Dot(targetDirection, directions[i]));
                // Apply a shaping function to favour moving away from the target at a slight angle.
                float dot = Mathf.Clamp01(-(1f - Mathf.Abs(Vector2.Dot(targetDirection, directions[i]) - 0.65f)));

                // Weigh the interest based on distance.
                float interest = dot * targetWeight;

                if (interest > interestMap[i])
                    interestMap[i] = interest;
            }
        }
        */
        return interestMap;
    }

    // Calculate danger values based on the distance to the obstacle.
    public override float[] GetDangerMap(Vector2 position, Vector2 targetPos, Vector2[] directions)
    {
        float[] dangerMap = new float[directions.Length];

        // Loop through each potential target.
        foreach (Collider2D obstacle in Physics2D.OverlapCircleAll(position, _detectionDistance, _obstacleLayers))
        {
            // Cache values.
            Vector2 closestPoint = obstacle.ClosestPoint(position);
            Vector2 targetDirection = (closestPoint - position).normalized;
            float targetDistance = Vector2.Distance(closestPoint, position);

            // Calculate the weight applied to the weights, inversely proportional to the distance to the target.
            /*float targetWeight = targetDistance < _maxMagnitudeRadius
                ? 1
                : 1f - ((targetDistance - _maxMagnitudeRadius) / (_detectionDistance - _maxMagnitudeRadius));
            targetWeight = Mathf.Clamp01(targetWeight);*/
            float targetWeight = 1f - (targetDistance / _detectionDistance);

            // Loop through each direction.
            for (int i = 0; i < directions.Length; i++)
            {
                // Calculate the danger using on the dot product, then weigh using the targetWeight.
                //float dot = Mathf.Clamp01(1f - Mathf.Abs(Vector2.Dot(targetDirection, directions[i]) - 0.65f));
                float dot = Vector2.Dot(targetDirection, directions[i]);
                float danger = Mathf.Clamp01(dot * targetWeight);

                if (danger > dangerMap[i])
                    dangerMap[i] = danger;
            }
        }

        return dangerMap; 
    }


    public override void DrawGizmos(Transform transform)
    {
        if (ShowGizmos)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _maxMagnitudeRadius);
        Gizmos.DrawWireSphere(transform.position, _detectionDistance);
    }
}