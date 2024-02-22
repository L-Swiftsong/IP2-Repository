using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/AoE Attack", fileName = "New AoE Attack")]
public class AoEAttack : Attack
{
    [Header("AoE Variables")]
    [SerializeField] private float _aoeRadius;
    [SerializeField] private float _aoeDelay;

    [Space(5)]
    [SerializeField] private GameObject _explosivePrefab;
    [SerializeField] private float _defaultThrowDistance;
    [SerializeField] private float _throwSpeed;

    [Space(5)]
    [SerializeField] private bool _useCustomThrowCurve;
    [SerializeField] private AnimationCurve _customThrowCurve;


    public override void MakeAttack(Transform attackingTransform) => ProcessAttack(attackingTransform, attackingTransform.up, _defaultThrowDistance);
    public override void MakeAttack(Transform attackingTransform, Vector2 targetPos)
    {
        float throwDistance = Vector2.Distance(attackingTransform.position, targetPos);
        Vector2 throwDirection = (targetPos - (Vector2)attackingTransform.position).normalized;

        ProcessAttack(attackingTransform, throwDirection, throwDistance);
    }


    private void ProcessAttack(Transform attackingTransform, Vector2 attackDirection, float throwDistance)
    {
        Vector2 targetPosition = (Vector2)attackingTransform.position + (attackDirection * throwDistance);

        Factions ignoredFactions = Factions.Unaligned;
        if (!CanHitAllies && attackingTransform.TryGetComponentThroughParents<EntityFaction>(out EntityFaction entityFaction))
            ignoredFactions = entityFaction.Faction;
        Collider2D ignoredCollider = CanHitSelf ? null : attackingTransform.GetComponent<Collider2D>();


        FixedDistanceProjectile instantiatedExplosive = Instantiate<GameObject>(_explosivePrefab.gameObject, attackingTransform.position, Quaternion.identity).GetComponent<FixedDistanceProjectile>();
        instantiatedExplosive.Init(
            startPos: attackingTransform.position,
            targetPos: targetPosition,
            speed: _throwSpeed,
            radius: _aoeRadius,
            delay: _aoeDelay,
            throwCurve: _useCustomThrowCurve ? _customThrowCurve : null,
            damageableLayers: HitMask,
            ignoredCollider: ignoredCollider,
            ignoredFactions: ignoredFactions);
    }


    public override Vector2? CalculateInterceptionPosition(Vector2 startPos, Vector2 targetPos, Vector2 targetVelocity)
    {
        // Calculate values.
        Vector2 startToTarget = startPos - targetPos;
        float targetDistance = startToTarget.magnitude;
        float targetVelocityDirection = Vector2.Angle(startToTarget, targetVelocity) * Mathf.Deg2Rad;
        float targetSpeed = targetVelocity.magnitude;


        float r = targetSpeed / _throwSpeed;
        // If there is no valid direction, then stop here.
        if (MathFunctions.SolveQuadratic(
            1 - (r * r),
            2 * r * targetDistance * Mathf.Cos(targetVelocityDirection),
            -(targetDistance * targetDistance),
            out float root1,
            out float root2) == 0)
        {
            return null;
        }


        // Find the positive root.
        float distanceToEstimatedPosition = Mathf.Max(root1, root2);

        // Calculate and output the interception position.
        float timeToInterception = distanceToEstimatedPosition / _throwSpeed;
        return targetPos + targetVelocity * timeToInterception;


        //return targetPos + targetVelocity * _aoeDelay;
    }


    public override void DrawGizmos(Transform gizmosOrigin)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gizmosOrigin.position, _aoeRadius);
    }
}
