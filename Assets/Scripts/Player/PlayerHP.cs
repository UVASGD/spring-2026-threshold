using System;
using System.Collections;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    [SerializeField] private int maxHP;
    [SerializeField] private int currentHP;
    [SerializeField] DynamicTrigger deathCutscene; //when the player dies, trigger this cutscene
    [SerializeField] DynamicTrigger hitFlash;
    [SerializeField] float invincibilitySeconds;

    public int CurrentHP => currentHP;
    private float timeSinceDamage;
    void Start()
    {
        timeSinceDamage = invincibilitySeconds; //set it equal at start so the player can take damage immediately.
    
        HealthBar.i.setMaxHealth(maxHP);
        HealthBar.i.updateCurrentHealth(currentHP);
    
        StartCoroutine(regenHealth()); //start the regen health process asynchronously.
    }
    void Update()
    {
        timeSinceDamage += Time.deltaTime; //time since last damage updates with time passed to ensure invincibility frames are provided fairly
    }
    void OnTriggerEnter(Collider other)
    {
        TryTakeHurtboxDamage(other);
    }
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        TryTakeHurtboxDamage(hit.collider);
    }
    private void TryTakeHurtboxDamage(Collider other)
    {
        if(!other.CompareTag("Hurtbox")) return;

        Hurtbox hurtbox = other.GetComponent<Hurtbox>();
        if(hurtbox == null) hurtbox = other.GetComponentInParent<Hurtbox>();
        if(hurtbox == null) return;

        Debug.Log("Hurtbox hit");
        takeDamage(hurtbox.dealDamage());
    }
    public void takeDamage(int amount)
    {
        if(timeSinceDamage < invincibilitySeconds){
            Debug.Log($"Player should not take damage yet due to {invincibilitySeconds - timeSinceDamage} remaining iFrames");
            return;   
        }

        timeSinceDamage = 0f; //reset time since last damage

        currentHP -= amount;
        Debug.Log($"Player took {amount} damage and now has {currentHP} remaining.");
        
        HealthBar.i.updateCurrentHealth(currentHP);

        if(currentHP <= 0)
        {
            deathCutscene.externalTriggerActivated();
        }
        else
        {
            //play the damage red flash
            hitFlash.externalTriggerActivated();
        }
    }
    public void healHealth(int amount)
    {
        currentHP = Math.Min(maxHP, currentHP + amount); //cap at maximum health

        HealthBar.i.updateCurrentHealth(currentHP);
    }
    private IEnumerator regenHealth()
    {
        //player innately regains health up to half their bar (like GTAV),
        //in increments of 1 every 10sec. (subject to balancing changes)

        float timeSinceHealIncrement = 0f;
        const float healTimeThreshold = 10f;
        const int healAmount = 1;
        while(currentHP > 0)
        {
            timeSinceHealIncrement += Time.deltaTime;
            if(timeSinceHealIncrement >= healTimeThreshold && currentHP < (maxHP / 2))
            {
                timeSinceHealIncrement = 0f;
                healHealth(healAmount);
            }

            yield return null;
        }
    }
}