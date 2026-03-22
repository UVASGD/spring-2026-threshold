using System;
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
    }
    void Update()
    {
        timeSinceDamage += Time.deltaTime; //time since last damage updates with time passed to ensure invincibility frames are provided fairly
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Hurtbox")
        {
            Debug.Log("Hurtbox hit");
            takeDamage(other.gameObject.GetComponent<Hurtbox>().dealDamage());
        }
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
}
