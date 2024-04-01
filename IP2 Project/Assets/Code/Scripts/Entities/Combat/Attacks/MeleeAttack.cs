using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Attacks/Melee Attack", fileName = "New Melee Attack", order = 1)]
public class MeleeAttack : Attack
{
    [Header("Melee")]
    [SerializeField] private Vector2 _extents;
    [SerializeField] private Vector2 _offset;
    [SerializeField] private float _attackDuration = 0f;

    [Space(5)]
    [SerializeField] private bool _reflectProjectiles = false;
    [SerializeField] private float _knockbackStrength = 0f;

    [Space(5)]
    [SerializeField] private LayerMask _environmentMask = 1 << 6;


    public override float GetDuration() => _attackDuration;


    public override void MakeAttack(AttackReferences references)
    {
        // Calculate the AttackDirection.
        Vector2 attackDirection = references.TargetPos.HasValue ? (references.TargetPos.Value - (Vector2)references.AttackingTransform.position).normalized : references.AttackingTransform.up;
        
        // Handle the Attacking Logic.
        references.MonoScript.StartCoroutine(ProcessAttack(references.AttackingTransform, attackDirection));
    }


    private IEnumerator ProcessAttack(Transform attackingTransform, Vector2 attackDirection)
    {
        // Calculate ally factions.
        Factions allyFactions = Factions.Unaligned;
        if (!CanHitAllies && attackingTransform.TryGetComponentThroughParents<EntityFaction>(out EntityFaction entityFaction))
            allyFactions = entityFaction.Faction;

        // Create a list that will be used for our already hit targets.
        List<Transform> hitTargets = new List<Transform>();
        if (CanHitSelf == false && attackingTransform.TryGetComponentThroughParents<Collider2D>(out Collider2D firstCollider))
            hitTargets.Add(firstCollider.transform);

        float durationRemaining = _attackDuration;
        do
        {
            // Calculate/Update variables needed for OverlapBoxAll.
            Vector2 attackOrigin = attackingTransform.TransformPoint(_offset);
            float attackAngle = Vector2.Angle(Vector2.up, attackDirection);

            // Loop through each hit collider.
            foreach (Collider2D target in Physics2D.OverlapBoxAll(attackOrigin, _extents, attackAngle, HitMask))
            {
                // Don't hit allies (If CanHitAllies is true, this should always return false due to allyFactions being set to Factions.Unaligned).
                if (target.TryGetComponentThroughParents<EntityFaction>(out entityFaction))
                    if (entityFaction.IsAlly(allyFactions))
                        continue;

                // Don't hit obstructed targets.
                if (Physics2D.Linecast(attackingTransform.position, target.transform.position, _environmentMask).transform != null)
                    continue;

                // Get the Target's Transform.
                Transform targetTransform = target.transform;
                // If we have already hit this transform, skip over it.
                if (hitTargets.Contains(targetTransform))
                    continue;

                // Get the target's HealthComponent.
                HealthComponent healthComponent;
                if (targetTransform.TryGetComponentThroughParents<HealthComponent>(out healthComponent))
                    targetTransform = healthComponent.transform;


                // Deal damage.
                if (healthComponent != null)
                    healthComponent.TakeDamage();


                // Reflect Projectiles.
                if (_reflectProjectiles && targetTransform.TryGetComponent<Projectile>(out Projectile projectile))
                    projectile.Reflect(attackingTransform);


                // Knockback Entities with Rigidbodies.
                if (targetTransform.TryGetComponentThroughParents<Rigidbody2D>(out Rigidbody2D rb2D))
                {
                    Vector2 force = ((Vector2)targetTransform.position - attackOrigin).normalized * _knockbackStrength;
                    rb2D.AddForce(force, ForceMode2D.Impulse);
                }

                // Add this transform to the list of already hit transforms.
                hitTargets.Add(targetTransform);
            }

            yield return null;
            durationRemaining -= Time.deltaTime;
        } while (durationRemaining > 0f);
    }


    public override Vector2? CalculateInterceptionPosition(Vector2 startPos, Vector2 targetPos, Vector2 targetVelocity) => targetPos;
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
