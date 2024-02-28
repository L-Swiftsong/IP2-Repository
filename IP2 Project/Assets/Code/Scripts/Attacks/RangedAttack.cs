using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Ranged Attack", fileName = "New Ranged Attack")]
public class RangedAttack : Attack
{
    [Header("Ranged Attack Variables")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private bool _dealsDamage;

    [SerializeField, Min(1)] private int _projectileCount;
    [SerializeField] private float _angleBetweenProjectiles;


    [Header("Attack Variability")]
    [Tooltip("The maximum for the projectile's accuracy deviation (In Degrees)")]
        [SerializeField] private float _projectileAccuracy;
    [Tooltip("Should each projectile be randomly rotated?")]
        [SerializeField] private bool _individualAccuracy;
    public int ComboMultiplier;

    
    public override void MakeAttack(Transform attackingTransform) => ProcessAttack(attackingTransform, attackingTransform.up);
    public override void MakeAttack(Transform attackingTransform, Vector2 targetPos)
    {
        Vector2 targetDirection = (targetPos - (Vector2)attackingTransform.position).normalized;
        
        ProcessAttack(attackingTransform, targetDirection);
    }

    public void ProcessAttack(Transform attackingTransform, Vector2 attackDirection)
    {
        // Cache values.
        float minAngle = -_angleBetweenProjectiles * ((_projectileCount - 1) / 2f);

        Collider2D attackingCollider = !CanHitSelf ? attackingTransform.GetComponent<Collider2D>() : null;
        Factions ignoredFactions = Factions.Unaligned;
        if (!CanHitAllies && attackingTransform.TryGetComponent<EntityFaction>(out EntityFaction entityFaction))
            ignoredFactions = entityFaction.Faction;

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
            CreateProjectile(attackingTransform, firingDirection, attackingCollider, ignoredFactions);
        }
    }
    private void CreateProjectile(Transform originTransform, Vector2 upDir, Collider2D ignoredCollider, Factions ignoredFactions)
    {
        // Instantiate the projectile GO.
        Projectile projectile = Instantiate<GameObject>(_projectilePrefab.gameObject, originTransform.position, Quaternion.LookRotation(Vector3.forward, upDir)).GetComponent<Projectile>();
        
        // Initialise the projectile.
        projectile.Init(ignoredCollider, OnHit,
            targetLayers: HitMask,
            ignoredFactions: ignoredFactions);
    }


    private void OnHit(Collider2D hitCollider)
    {
        Debug.Log(this.name + " was used to hit: " + hitCollider.name);

        // Deal damage.
        if (_dealsDamage && hitCollider.TryGetComponent<HealthComponent>(out HealthComponent healthComponent))
        {
            healthComponent.TakeDamage();

            if (hitCollider.name == "Test Enemy")
            {
                ComboMultiplier++;
            }
        }

        
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
