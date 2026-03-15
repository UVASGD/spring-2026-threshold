using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DynamicTrigger : MonoBehaviour
{
    [SerializeField] bool retrigger; //if false, disable the trigger after it is triggered once
    [SerializeReference]
    [SerializeField] List<TriggerEffect> triggerEffects;
    public IEnumerator runEffects()
    {
        foreach (var effect in triggerEffects)
        {
            if (effect.Asynchronous)
            {
                //start as another coroutine
                StartCoroutine(effect.onTriggerEffect());
            }
            else
            {
                yield return effect.onTriggerEffect();
            }
        }

        gameObject.SetActive(retrigger); //set to inactive if retrigger is false
    }
    public void addEffect(TriggerEffect effect)
    {
        if(effect != null)
            triggerEffects.Add(effect);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Generic trigger entered");
            StartCoroutine(runEffects());
        }
    }
}