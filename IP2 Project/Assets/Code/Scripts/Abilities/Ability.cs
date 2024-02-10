using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Ability : ScriptableObject
{
    [SerializeField] protected float CooldownTime;

    public float GetCooldownTime() => CooldownTime;
}
