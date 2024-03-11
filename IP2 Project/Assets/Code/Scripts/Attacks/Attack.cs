using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Attack : ScriptableObject
{
    [SerializeField] protected bool CanHitSelf = false;
    [SerializeField] protected bool CanHitAllies = false;
    [SerializeField] protected LayerMask HitMask = 1 << 0 | 1 << 3 | 1 << 6 | 1 << 8;

    [Space(5)]
    [SerializeField] protected float WindupTime = 0f;
    [SerializeField] protected float RecoveryTime = 1f;

    [Space(5)]
    [SerializeField] protected bool DealsDamage = true;


    public float GetWindupTime() => WindupTime;
    public float GetRecoveryTime() => RecoveryTime;
    public float GetTotalAttackTime() => WindupTime + RecoveryTime;



    public abstract void MakeAttack(Transform attackingTransform);
    public abstract void MakeAttack(Transform attackingTransform, Vector2 targetPos);

    public abstract Vector2? CalculateInterceptionPosition(Vector2 startPos, Vector2 targetPos, Vector2 targetVelocity);
    public abstract void DrawGizmos(Transform gizmosOrigin);
}
