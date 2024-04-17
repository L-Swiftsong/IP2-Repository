using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForAbility : MonoBehaviour
{
    void Update()
    {
        if (!PlayerManager.IsInitialised || PlayerManager.Instance.Player == null)
            return;

        if (PlayerManager.Instance.Player.TryGetComponent<AbilityHolder>(out AbilityHolder abilityHolder))
        {
            if (abilityHolder.IsActive && abilityHolder.Ability.name == "TigerRush" && !gameObject.GetComponent<EntityFaction>().Faction.IsAlly(Factions.Player))
            {
                gameObject.GetComponent<CircleCollider2D>().isTrigger = true;
            }
            if (!abilityHolder.IsActive && abilityHolder.Ability.name == "TigerRush" && !gameObject.GetComponent<EntityFaction>().Faction.IsAlly(Factions.Player))
            {
                gameObject.GetComponent<CircleCollider2D>().isTrigger = false;
            }
        }
    }
}
