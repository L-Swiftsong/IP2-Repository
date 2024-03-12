using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component to hold a reference to this entity's faction.
/// </summary>
public class EntityFaction : MonoBehaviour
{
    [SerializeField] private Factions _faction;
    public Factions Faction => _faction;


    public bool IsAlly(EntityFaction entityFaction) => _faction.IsAlly(entityFaction.Faction);
    public bool IsAlly(Factions factionToTest) => _faction.IsAlly(factionToTest);
}
