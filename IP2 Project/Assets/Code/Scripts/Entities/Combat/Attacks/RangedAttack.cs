using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Ranged Attack", fileName = "New Ranged Attack", order = 2)]
public class RangedAttack : Attack
{
    [Header("Ranged Attack Variables")]
    [SerializeField] private GameObject _projectilePrefab;

    [SerializeField, Min(1)] private int _projectileCount = 1;
    [SerializeField] private float _angleBetweenProjectiles;


    [Header("Attack Variability")]
    [Tooltip("The maximum for the projectile's accuracy deviation (In Degrees)")]
        [SerializeField] private float _projectileAccuracy;
    [Tooltip("Should each projectile be randomly rotated?")]
        [SerializeField] private bool _individualAccuracy;

    
    public override Coroutine MakeAttack(AttackReferences references)
    {
        // Calculate the AttackDirection.
        Vector2 attackDirection = references.TargetPos.HasValue ? (references.TargetPos.Value - (Vector2)references.AttackingTransform.position).normalized : references.AttackingTransform.up;

        // Handle the Attacking Logic.
        return references.MonoScript.StartCoroutine(ProcessAttack(references.AttackingTransform, attackDirection));
    }

    public IEnumerator ProcessAttack(Transform attackingTransform, Vector2 attackDirection)
    {
        // Cache values.
        float minAngle = -_angleBetweenProjectiles * ((_projectileCount - 1) / 2f);

        // Find ignoredFactions.
        Factions ignoredFactions = Factions.Unaligned;
        if (!CanHitAllies && attackingTransform.TryGetComponentThroughParents<EntityFaction>(out EntityFaction entityFaction))
            ignoredFactions = entityFaction.Faction;

        // Find the first transform of the target that has a collider2D.
        Transform ignoredTransform = null;
        if (CanHitSelf == false && attackingTransform.TryGetComponentThroughParents<Collider2D>(out Collider2D firstCollider))
            ignoredTransform = firstCollider.transform;


        float sharedAccuracyDeviation = Random.Range(-_projectileAccuracy, _projectileAccuracy);
        // Loop through for each projectile we should create.
        for (int i = 0; i < _projectileCount; i++)
        {
            // Calculate the accuracy deviation for this projetile.
            float accuracyDeviation = _individualAccuracy ? Random.Range(-_projectileAccuracy, _projectileAccuracy) : sharedAccuracyDeviation;

            // Calculate the firing direction for this projectile.
            float firingAngle = minAngle + (_angleBetweenProjectiles * i) + accuracyDeviation;
            Vector2 firingDirection = (Quaternion.Euler(0f, 0f, firingAngle) * attackDirection).normalized;

            // Create the projectile.
            CreateProjectile(attackingTransform, firingDirection, ignoredTransform, ignoredFactions);
        }


        // Return using yield break to allow for this to be made a coroutine.
        yield break;
    }
    private void CreateProjectile(Transform originTransform, Vector2 upDir, Transform ignoredTransform, Factions ignoredFactions)
    {
        // Instantiate the projectile GO.
        Projectile projectile = Instantiate<GameObject>(_projectilePrefab.gameObject, originTransform.position, Quaternion.LookRotation(Vector3.forward, upDir)).GetComponent<Projectile>();
        
        // Initialise the projectile.
        projectile.Init(ignoredTransform, OnHit,
            targetLayers: HitMask,
            ignoredFactions: ignoredFactions);
    }


    private void OnHit(Transform hitTransform, Vector2 hitDirection)
    {
        Debug.Log(this.name + " was used to hit: " + hitTransform.name);

        // Deal damage.
        if (DealsDamage && hitTransform.TryGetComponent<HealthComponent>(out HealthComponent healthComponent))
            healthComponent.TakeDamage();

        // Try to apply Knockback to the hit Entity.
        Vector2 force = hitDirection * KnockbackStrength;
        hitTransform.TryApplyForce(force);
    }


    public override Vector2? CalculateInterceptionPosition(Vector2 startPos, Vector2 targetPos, Vector2 targetVelocity)
    {
        // Calculate values.
        Vector2 startToTarget = startPos - targetPos;
        float targetDistance = startToTarget.magnitude;
        float targetVelocityDirection = Vector2.Angle(startToTarget, targetVelocity) * Mathf.Deg2Rad;
        float targetSpeed = targetVelocity.magnitude;
        float projectileSpeed = _projectilePrefab.GetComponent<Projectile>().ProjectileSpeed;


        float r = targetSpeed / projectileSpeed;
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
        float timeToInterception = distanceToEstimatedPosition / projectileSpeed;
        Debug.Log(targetVelocity);
        return targetPos + targetVelocity * timeToInterception;
    }
    public override void DrawGizmos(Transform gizmosOrigin)
    {
        Gizmos.color = Color.red;

        Vector2 up = gizmosOrigin.up;
        float minRotation = -_angleBetweenProjectiles * ((_projectileCount - 1) / 2f);
        for (int i = 0; i < _projectileCount; i++)
        {
            float projectileAngle = minRotation + _angleBetweenProjectiles * i;
            Vector2 projectileUp = (Quaternion.Euler(0f, 0f, projectileAngle) * up).normalized;

            Gizmos.DrawRay(gizmosOrigin.position, projectileUp * 2f);
        }
    }
}
