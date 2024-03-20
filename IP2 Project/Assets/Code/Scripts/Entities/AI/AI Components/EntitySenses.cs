using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySenses : MonoBehaviour
{
    [SerializeField, ReadOnly] private Transform _currentTarget;
    public Transform CurrentTarget => _currentTarget;
    [SerializeField] private EntityFaction _thisFaction;


    private Coroutine _updateTargetCoroutine;
    private const float UPDATE_TARGET_DELAY = 0.15f;


    [Header("Sight")]
    [SerializeField] private float _visionRadius = 7.5f;
    [SerializeField] private float _visionAngle = 225f;

    [SerializeField] private LayerMask _visibleLayers = 1 << 3 | 1 << 8; // Default Value: Player, Entity.


    [Header("Gizmos")]
    [SerializeField] private bool _drawGizmos = false;
    [SerializeField] private Color _visionGizmosColour = Color.red;


    private void OnEnable()
    {
        if (_updateTargetCoroutine == null)
            _updateTargetCoroutine = StartCoroutine(UpdateTarget());
    }

    
    private IEnumerator UpdateTarget()
    {
        while (true)
        {
            CheckForTarget();
            yield return new WaitForSeconds(UPDATE_TARGET_DELAY);
        }
    }
    private void CheckForTarget()
    {
        // Find nearby targets.
        Transform closestTarget = null;
        float minDistance = float.MaxValue;
        bool selfHasFaction = _thisFaction != null;
        foreach (Collider2D potentialTarget in Physics2D.OverlapCircleAll(transform.position, _visionRadius, _visibleLayers))
        {
            // Discount allies.
            if (selfHasFaction && potentialTarget.TryGetComponent<EntityFaction>(out EntityFaction faction))
                if (_thisFaction.IsAlly(faction))
                    continue;

            
            

            // Discount targets out of viewcone.
            float angleToTarget = Vector2.Angle(transform.up, potentialTarget.transform.position - transform.position);
            if (angleToTarget > _visionAngle / 2f)
                continue;


            // Choose closest valid target.
            DrawDebugCross(potentialTarget.transform.position, Color.red);
            float distanceToTarget = Vector2.Distance(transform.position, potentialTarget.transform.position);
            if (distanceToTarget < minDistance)
            {
                minDistance = distanceToTarget;
                closestTarget = potentialTarget.transform;
            }
        }


        // Update the current target.
        _currentTarget = closestTarget;
        if (closestTarget != null)
            DrawDebugCross(closestTarget.position, Color.green);
    }
    private void DrawDebugCross(Vector2 position, Color debugColour)
    {
        Debug.DrawLine(position + Vector2.down, position + Vector2.up, debugColour, UPDATE_TARGET_DELAY);
        Debug.DrawLine(position + Vector2.left, position + Vector2.right, debugColour, UPDATE_TARGET_DELAY);
    }



    private void OnDrawGizmosSelected()
    {
        if (!_drawGizmos)
            return;


        // Vision.
        Gizmos.color = _visionGizmosColour;
        Gizmos.DrawWireSphere(transform.position, _visionRadius);

        Vector2 rightDir = Quaternion.AngleAxis(_visionAngle / 2f, Vector3.forward) * transform.up * _visionRadius;
        Vector2 leftDir = Quaternion.AngleAxis(-_visionAngle / 2f, Vector3.forward) * transform.up * _visionRadius;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + rightDir);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + leftDir);
    }
}
