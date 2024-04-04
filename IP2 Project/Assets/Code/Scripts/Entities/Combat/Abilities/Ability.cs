using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : ScriptableObject
{
    public new string name;
    public float cooldownTime;
    public float activeTime;

    
    [Space(5)]
    public Sprite AbilitySprite;


    [Space(5)]
    [SerializeField] protected LayerMask HitMask = 1 << 0 | 1 << 3 | 1 << 6 | 1 << 8;
    [SerializeField] protected bool CanHitSelf = false;
    [SerializeField] protected bool CanHitAllies = false;

    
    public virtual void Activate(GameObject parent, Transform transform) { }
}
