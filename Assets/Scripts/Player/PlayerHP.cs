using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    [SerializeField] private int maxHP;
    [SerializeField] private int currentHP;
    [SerializeField] DynamicTrigger deathCutscene; //when the player dies, trigger this cutscene
    public int CurrentHP => currentHP;
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Hurtbox")
        {
            takeDamage(0);
        }
    }
    public void takeDamage(int amount)
    {
        currentHP--;
        if(currentHP <= 0)
        {
            deathCutscene.externalTriggerActivated();
        }
    }
}
