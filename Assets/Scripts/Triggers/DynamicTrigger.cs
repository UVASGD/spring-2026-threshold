using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicTrigger : MonoBehaviour
{
    [SerializeField] bool retrigger; //if false, disable the trigger after it is triggered once
    [SerializeReference]
    [SerializeField] List<TriggerEffect> triggerEffects = new List<TriggerEffect>();
    private Coroutine activeEffectsCoroutine;

    public bool IsRunning => activeEffectsCoroutine != null;

    public void StartEffects()
    {
        if(activeEffectsCoroutine != null) return;
        activeEffectsCoroutine = StartCoroutine(runEffects());
    }

    public void StopEffects()
    {
        if(activeEffectsCoroutine == null) return;

        StopAllCoroutines();
        activeEffectsCoroutine = null;
    }

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
        activeEffectsCoroutine = null;
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
            Debug.Log("Trigger entered");
            StartEffects();
        }
    }

    public void externalTriggerActivated()
    {
        OnTriggerEnter(FirstPersonController.i.gameObject.GetComponent<Collider>()); //get the player's collider and use it for the trigger (bad hardcode hack)
    }
}