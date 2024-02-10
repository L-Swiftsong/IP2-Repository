using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Attack : ScriptableObject
{
    [SerializeField] protected bool CanHitSelf = false;
    [SerializeField] protected bool CanHitAllies = false;
    [SerializeField] protected LayerMask HitMask;

    [Space(5)]
    [SerializeField] protected float RecoveryTime;

    public float GetRecoveryTime() => RecoveryTime;



    public abstract void MakeAttack(Transform attackingTransform);
    public abstract void MakeAttack(Transform attackingTransform, Vector2 targetPos);
    public abstract void DrawGizmos(Transform gizmosOrigin);
}
