using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForAbility : MonoBehaviour
{
    private GameObject Player;

    private void Start()
    {
        if (PlayerManager.IsInitialised)
            Player = PlayerManager.Instance.Player;
    }

    // Update is called once per frame
    void Update()
    {
        AbilityHolder abilityHolder = Player.GetComponent<AbilityHolder>();

        if (abilityHolder.IsActive && abilityHolder.Ability.name == "TigerRush" && gameObject.GetComponent<EntityFaction>().Faction.IsAlly(Factions.Yakuza))
        {
            gameObject.GetComponent<CircleCollider2D>().isTrigger = true;
        }
        if (!abilityHolder.IsActive && abilityHolder.Ability.name == "TigerRush" && gameObject.GetComponent<EntityFaction>().Faction.IsAlly(Factions.Yakuza))
        {
            gameObject.GetComponent<CircleCollider2D>().isTrigger = false;
        }
    }
}
