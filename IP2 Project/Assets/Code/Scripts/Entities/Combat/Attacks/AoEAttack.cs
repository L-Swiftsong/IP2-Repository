using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Attacks/AoE Attack", fileName = "New AoE Attack")]
public class AoEAttack : Attack
{
    [Header("AoE Variables")]
    [SerializeField] private float _aoeRadius;
    [SerializeField] private float _aoeDelay;
    [SerializeField] private bool _explosionDealsDamage = true;

    [Space(5)]
    [SerializeField] private bool _explodeOnCollision;
    [SerializeField] private bool _showExplosionRadius;
    [SerializeField] private bool _earlyExplosionReducesSize;

    [Space(5)]
    [SerializeField] private GameObject _explosivePrefab;
    [SerializeField] private float _defaultThrowDistance;
    [SerializeField] private float _throwSpeed;


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

        
        // Calculate ignored values.
        Transform ignoredTransform = !CanHitSelf ? attackingTransform : null;
        Factions ignoredFactions = Factions.Unaligned;
        if (!CanHitAllies && attackingTransform.TryGetComponentThroughParents<EntityFaction>(out EntityFaction entityFaction))
            ignoredFactions = entityFaction.Faction;


        ExplosiveProjectile projectile = Instantiate<GameObject>(_explosivePrefab.gameObject, attackingTransform.position, Quaternion.identity).GetComponent<ExplosiveProjectile>();
        projectile.Init(
            ignoreTransform: ignoredTransform,
            callback: OnProjectileHit,
            explosionDelay: _aoeDelay,
            explodeOnCollision: _explodeOnCollision,
            explosionRadius: _aoeRadius,
            showRadius: _showExplosionRadius,
            explosionCallback: OnExplosionHit,
            targetLayers: HitMask,
            targetPosition: targetPosition,
            ignoredFactions: ignoredFactions,
            earlyExplosionReducesSize: _earlyExplosionReducesSize
            );
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


    private void OnProjectileHit(Transform hitTransform)
    {
        Debug.Log(this.name + " was used to hit: " + hitTransform.name);

        // Deal damage.
        if (DealsDamage && hitTransform.TryGetComponent<HealthComponent>(out HealthComponent healthComponent))
            healthComponent.TakeDamage();
    }
    private void OnExplosionHit(Transform[] hitTransforms)
    {
        // Loop through each hit transform.
        foreach (Transform hitTransform in hitTransforms)
        {
            Debug.Log(hitTransform.name + " was hit by an explosion from: " + this.name);

            // Deal damage.
            if (_explosionDealsDamage && hitTransform.TryGetComponent<HealthComponent>(out HealthComponent healthComponent))
                healthComponent.TakeDamage();
        }
    }


    public override void DrawGizmos(Transform gizmosOrigin)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gizmosOrigin.position, _aoeRadius);
    }
}
