using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DynamicTrigger : MonoBehaviour
{
    [SerializeField] bool retrigger; //if false, disable the trigger after it is triggered once
    [SerializeReference]
    [SerializeField] List<TriggerEffect> triggerEffects;
    public void runEffects()
    {
        foreach (var effect in triggerEffects)
        {
            effect.onTriggerEffect();
        }

        this.gameObject.SetActive(retrigger); //set to inactive if retrigger is false
    }
    public void addEffect(TriggerEffect effect)
    {
        triggerEffects.Add(effect);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Generic trigger entered");
            runEffects();
        }
    }
}