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

    [Space(5)]
    [SerializeField] private bool _useCustomThrowCurve;
    [SerializeField] private AnimationCurve _customThrowCurve;
    public int ComboMultiplier;
    public bool Timer;
    

    public override void MakeAttack(Transform attackingTransform) => ProcessAttack(attackingTransform, attackingTransform.up, _defaultThrowDistance);
    public override void MakeAttack(Transform attackingTransform, Vector2 targetPos)
    {
        float throwStrength = Vector2.Distance(attackingTransform.position, targetPos);
        Vector2 throwDirection = (targetPos - (Vector2)attackingTransform.position).normalized;

        ProcessAttack(attackingTransform, throwDirection, throwStrength);
    }


    private void ProcessAttack(Transform attackingTransform, Vector2 attackDirection, float throwDistance)
    {
        Vector2 targetPosition = (Vector2)attackingTransform.position + (attackDirection * throwDistance);
        
        Factions ignoredFactions = Factions.Unaligned;
        if (!CanHitAllies && attackingTransform.TryGetComponentThroughParents<EntityFaction>(out EntityFaction entityFaction))
            ignoredFactions = entityFaction.Faction;
        Collider2D ignoredCollider = CanHitSelf ? null : attackingTransform.GetComponent<Collider2D>();

        if (attackingTransform.TryGetComponentThroughParents<EntityFaction>(out EntityFaction faction))
        {
            if(faction.IsAlly(Factions.Yakuza))
            {
                ComboMultiplier++;
                Timer = true;
            }
        }

        FixedDistanceProjectile instantiatedExplosive = Instantiate<GameObject>(_explosivePrefab.gameObject, attackingTransform.position, Quaternion.identity).GetComponent<FixedDistanceProjectile>();
        instantiatedExplosive.Init(
            startPos: attackingTransform.position,
            targetPos: targetPosition,
            radius: _aoeRadius,
            delay: _aoeDelay,
            throwCurve: _useCustomThrowCurve ? _customThrowCurve : null,
            damageableLayers: HitMask,
            ignoredCollider: ignoredCollider,
            ignoredFactions: ignoredFactions);
    }


    public override Vector2? CalculateInterceptionPosition(Vector2 startPos, Vector2 targetPos, Vector2 targetVelocity) => targetPos + targetVelocity * _aoeDelay;
    
    public override void DrawGizmos(Transform gizmosOrigin)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gizmosOrigin.position, _aoeRadius);
    }

    
}
