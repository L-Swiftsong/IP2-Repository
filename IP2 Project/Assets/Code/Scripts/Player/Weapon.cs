using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/New Weapon")]
public class Weapon : ScriptableObject
{
    public Attack[] Attacks;
    [Tooltip("How long after the previous attack before the combo resets to the initial attack.")]
        public float ComboResetTime;


    [Header("Recharging Uses Variables")]
    [Tooltip("Set to true for if this weapon should have unrestricted uses.")]
        public bool IgnoreUses = true;
    [Tooltip("Whether the Weapon should only count down its uses before recharge for the first attack in a combo.")]
        public bool TickUseOnFirstAttackOnly;
    [Tooltip("The number of times this Weapon can be used before needing to recharge.")]
        public int UsesBeforeRecharge;
    [Tooltip("How long after an attack before this weapon recharges its uses.")]
        public float TimeToRecharge;
}