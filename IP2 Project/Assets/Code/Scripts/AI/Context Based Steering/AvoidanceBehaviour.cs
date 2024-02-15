using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidanceBehaviour : BaseSteeringBehaviour
{
    [SerializeField] private float _detectionDistance;
    [SerializeField] private LayerMask _obstacleLayers;
    private Collider2D[] _obstacles;
    private bool _cachedObstacles;

    [Space(5)]
    [SerializeField] private float _maxMagnitudeRadius;


    [Header("Gizmos")]
    [SerializeField] private bool _drawGizmos;


    private void LateUpdate() => _cachedObstacles = false;


    // The AvoidanceBehaviour behaviour does not return interest values.
    public override float[] GetInterestMap(Vector2 position, Vector2[] directions)
    {
        float[] interestMap = new float[directions.Length];

        if (!_cachedObstacles)
            _obstacles = Physics2D.OverlapCircleAll(position, _detectionDistance, _obstacleLayers);

        // Loop through each potential target.
        foreach (Collider2D obstacle in _obstacles)
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
                float dot = Mathf.Clamp01(-Vector2.Dot(targetDirection, directions[i]));

                // Weigh the interest based on distance.
                float interest = dot * targetWeight;

                if (interest > interestMap[i])
                    interestMap[i] = interest;
            }
        }

        return interestMap;
    }

    // Calculate danger values based on the distance to the obstacle.
    public override float[] GetDangerMap(Vector2 position, Vector2[] directions)
    {
        float[] dangerMap = new float[directions.Length];

        if (!_cachedObstacles)
            _obstacles = Physics2D.OverlapCircleAll(position, _detectionDistance, _obstacleLayers);

        // Loop through each potential target.
        foreach (Collider2D obstacle in _obstacles)
        {
            // Cache values.
            Vector2 closestPoint = obstacle.ClosestPoint(position);
            Vector2 targetDirection = (closestPoint - position).normalized;
            float targetDistance = Vector2.Distance(closestPoint, position);

            // Calculate the weight applied to the weights, inversely proportional to the distance to the target.
            float targetWeight = targetDistance < _maxMagnitudeRadius
                ? 1
                : 1f - ((targetDistance - _maxMagnitudeRadius) / (_detectionDistance - _maxMagnitudeRadius));
            targetWeight = Mathf.Clamp01(targetWeight);

            // Loop through each direction.
            for (int i = 0; i < directions.Length; i++)
            {
                // Calculate the danger using on the dot product, then weigh using the targetWeight.
                float dot = Vector2.Dot(targetDirection, directions[i]);
                float danger = dot * targetWeight;

                if (danger > dangerMap[i])
                    dangerMap[i] = danger;
            }
        }

        return dangerMap; 
    }


    private void OnDrawGizmosSelected()
    {
        if (!_drawGizmos)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _maxMagnitudeRadius);
        Gizmos.DrawWireSphere(transform.position, _detectionDistance);
    }
}