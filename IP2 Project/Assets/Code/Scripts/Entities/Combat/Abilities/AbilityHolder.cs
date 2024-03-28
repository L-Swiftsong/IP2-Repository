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
    [SerializeField]private float time = 0;
    DecptiveScreen clones;
    [ReadOnly]public int numberOfUses;
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
                    numberOfUses += 1;
                    if(ability.name == "DeceptiveScreen")
                    {
                        if ((int)gameObject.GetComponent<PlayerCombo>()._currentComboProperty == 0)
                        {
                            gameObject.GetComponent<PlayerCombo>().score += 300;
                        }
                        if ((int)gameObject.GetComponent<PlayerCombo>()._currentComboProperty > 0)
                        {
                            gameObject.GetComponent<PlayerCombo>().score += 300 * (int)gameObject.GetComponent<PlayerCombo>()._currentComboProperty;
                        }
                    }
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
            transform.GetChild(0).GetComponent<CircleCollider2D>().enabled = false;
            transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(255,255,255,1);
        }

        if(activeTime > 0 && ability.name == "DragonBreath")
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(0).GetComponent<CircleCollider2D>().enabled = true;
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

                        if ((int)gameObject.GetComponent<PlayerCombo>()._currentComboProperty == 0)
                        {
                            gameObject.GetComponent<PlayerCombo>().score += 100;
                        }
                        if ((int)gameObject.GetComponent<PlayerCombo>()._currentComboProperty > 0)
                        {
                            gameObject.GetComponent<PlayerCombo>().score += 100 * (int)gameObject.GetComponent<PlayerCombo>()._currentComboProperty;
                        }
                        


                    }
                }
            }

            
        }



    }


    private void OnTriggerStay2D(Collider2D other)
    {
        time += Time.deltaTime;
        
        if (ability.name == "DragonBreath" && activeTime > 0)
        {
            if (other.TryGetComponent<EntityFaction>(out EntityFaction faction))
            {

                
                if (faction.IsAlly(Factions.Yakuza))
                {
                    if (time >= 1)
                    {
                        if (other.TryGetComponent<HealthComponent>(out HealthComponent health))
                        {

                            health.TakeDamage();
                            time = 0;

                            if ((int)gameObject.GetComponent<PlayerCombo>()._currentComboProperty == 0)
                            {
                                gameObject.GetComponent<PlayerCombo>().score += 100;
                            }
                            if ((int)gameObject.GetComponent<PlayerCombo>()._currentComboProperty > 0)
                            {
                                gameObject.GetComponent<PlayerCombo>().score += 100 * (int)gameObject.GetComponent<PlayerCombo>()._currentComboProperty;
                            }

                        }
                    }
                }
            }
        }
    }




}
