using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Attack : ScriptableObject
{
    [SerializeField] protected bool CanHitSelf = false;
    [SerializeField] protected bool CanHitAllies = false;
    [SerializeField] protected LayerMask HitMask;


    public abstract void MakeAttack(Transform attackingTransform);
    public abstract void DrawGizmos(Transform gizmosOrigin);
}
