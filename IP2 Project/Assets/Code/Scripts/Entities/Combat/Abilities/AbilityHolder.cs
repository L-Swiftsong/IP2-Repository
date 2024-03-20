using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityHolder : MonoBehaviour
{

    public bool pressed = false; 
    public Ability ability;
    public float cooldownTime;
    public float activeTime;
    DecptiveScreen clones;
    public enum AbilityState
    {
        ready,
        active,
        cooldown
    }

    AbilityState state = AbilityState.ready;

    public void OnAbilityPressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            pressed = true;
        }
        if(context.canceled)
        {
            pressed = false;
        }
    }





    // Update is called once per frame
    void Update()
    {
        

        

        switch (state)
        {
            case AbilityState.ready:
                if(pressed == true)
                {
                    ability.Activate(gameObject, transform);
                    state = AbilityState.active;
                    activeTime = ability.activeTime;
                    
                }
            break;
            case AbilityState.active:
                if(activeTime > 0)
                {
                    activeTime -= Time.deltaTime;
                }
                else
                {
                    state = AbilityState.cooldown;
                    cooldownTime = ability.cooldownTime;
                }
            break;
            case AbilityState.cooldown:
                if (cooldownTime > 0)
                {
                    cooldownTime -= Time.deltaTime;
                }
                else
                {
                    state = AbilityState.ready;
                    
                }
            break;

            
           

        }

        if(activeTime <= 0 && ability.name == "DragonBreath")
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(255,255,255,1);
        }

        if(activeTime > 0 && ability.name == "DragonBreath")
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(183, 0, 0, 1);
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (ability.name == "TigerRush" && activeTime > 0)
        {
            

            if (collision.TryGetComponent<EntityFaction>(out EntityFaction faction))
            {


                if(faction.IsAlly(Factions.Yakuza))
                {
                    

                    if(collision.TryGetComponent<HealthComponent>(out HealthComponent health))
                    {   
                        
                        health.TakeDamage();
                        
                        
                       
                        
                    }
                }
            }

            
        }

        if (ability.name == "DragonBreath" && activeTime > 0)
        {
            if (collision.TryGetComponent<EntityFaction>(out EntityFaction faction))
            {


                if (faction.IsAlly(Factions.Yakuza))
                {

                    if (collision.TryGetComponent<HealthComponent>(out HealthComponent health))
                    {

                        health.TakeDamage();


                    }
                }
            }
        }


    }

   


}
