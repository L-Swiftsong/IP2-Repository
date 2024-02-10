using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Ranged Attack", fileName = "New Ranged Attack")]
public class RangedAttack : Attack
{
    [Header("Ranged Attack Variables")]
    [SerializeField] private GameObject _projectilePrefab;

    [SerializeField, Min(1)] private int _projectileCount;
    [SerializeField] private float _angleBetweenProjectiles;

    
    public override void MakeAttack(Transform attackingTransform)
    {
        // Cache values.
        Collider2D attackingCollider = !CanHitSelf ? attackingTransform.GetComponent<Collider2D>() : null;
        Factions ignoredFactions = Factions.Unaligned;
        if (!CanHitAllies && attackingTransform.TryGetComponent<EntityFaction>(out EntityFaction entityFaction))
            ignoredFactions = entityFaction.Faction;

        Vector2 attackDirection = attackingTransform.up;
        float minAngle = -_angleBetweenProjectiles * ((_projectileCount - 1) / 2f);


        // Loop through for each projectile we should create.
        for (int i = 0; i < _projectileCount; i++)
        {
            // Calculate the firing direction of this projectile.
            float projectileAngle = minAngle + (_angleBetweenProjectiles * i);
            Vector2 firingDirection = (Quaternion.Euler(0f, 0f, projectileAngle) * attackDirection).normalized;

            // Create the projectile.
            CreateProjectile(attackingTransform, firingDirection, attackingCollider, ignoredFactions);
        }
    }
    private void CreateProjectile(Transform originTransform, Vector2 upDir, Collider2D ignoredCollider, Factions ignoredFactions)
    {
        // Instantiate the projectile GO.
        Projectile projectile = Instantiate<GameObject>(_projectilePrefab, originTransform.position, Quaternion.LookRotation(Vector3.forward, upDir)).GetComponent<Projectile>();
        
        // Initialise the projectile.
        projectile.Init(ignoredCollider, OnHit,
            targetLayers: HitMask,
            ignoredFactions: ignoredFactions);
    }


    private void OnHit(Collider2D hitCollider)
    {
        Debug.Log(this.name + " was used to hit: " + hitCollider.name);

        // Deal damage.
        if (hitCollider.TryGetComponent<HealthComponent>(out HealthComponent healthComponent))
            healthComponent.TakeDamage();
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
