using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Attacks/Melee Attack", fileName = "New Melee Attack")]
public class MeleeAttack : Attack
{
    [Header("Melee")]
    [SerializeField] private Vector2 _extents;
    [SerializeField] private Vector2 _offset;


    public override void MakeAttack(Transform attackingTransform)
    {
        // Calculate ally factions.
        Factions allyFactions = Factions.Unaligned;
        if (!CanHitAllies && attackingTransform.TryGetComponent<EntityFaction>(out EntityFaction entityFaction))
            allyFactions = entityFaction.Faction;

        // Calculate variables needed for OverlapBoxAll.
        Vector2 attackOrigin = attackingTransform.TransformPoint(_offset);
        float attackAngle = attackingTransform.eulerAngles.z;


        // Create a list that will be used for our already hit targets.
        List<Transform> hitTargets = new List<Transform>();
        if (CanHitSelf == false)
            hitTargets.Add(attackingTransform);

        // Loop through each hit collider.
        foreach (Collider2D target in Physics2D.OverlapBoxAll(attackOrigin, _extents, attackAngle, HitMask))
        {
            
            // Don't hit allies (If CanHitAllies is true, this should always return false due to allyFactions being set to Factions.Unaligned).
            if (target.TryGetComponentThroughParents<EntityFaction>(out entityFaction))
                if (entityFaction.IsAlly(allyFactions))
                    continue;

            // Get the HealthComponent and Target's Transform.
            Transform targetTransform = target.transform;
            HealthComponent healthComponent;
            if (targetTransform.TryGetComponentThroughParents<HealthComponent>(out healthComponent))
                targetTransform = healthComponent.transform;

            // If we have already hit this transform, skip over it.
            if (hitTargets.Contains(targetTransform))
                continue;


            // This collider is a valid target.
            Debug.Log("Hit: " + target.name);


            // Deal damage.
            if (healthComponent != null)
                healthComponent.TakeDamage();


            // Add this transform to the list of already hit transforms.
            hitTargets.Add(targetTransform);
        }
    }


    public override void DrawGizmos(Transform gizmosOrigin)
    {
        // Change the gizmos matrix into a local matrix around the origin transform, allowing for local offsets & rotations to be used.
        Matrix4x4 originalMatrix = Gizmos.matrix;
        Gizmos.matrix = gizmosOrigin.localToWorldMatrix;


        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_offset, _extents);


        Gizmos.matrix = originalMatrix;
    }
}
