using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomChaos : MonoBehaviour
{
    [SerializeField] List<float> chaosThresholds; //each threshold has an associated event that happens in the room
    [SerializeField] List<UnityEvent> thresholdEvents;
    [SerializeField] List<ChaosObject> roomObjects;
    [SerializeField] float checkInterval = 1.0f;

    private float chaosLevel = 0f;

    void Start()
    {
        StartCoroutine(CalculateRoomChaos());
    }

    IEnumerator CalculateRoomChaos()
    {
        while (true)
        {
            float totalChaos = 0f;

            foreach(var member in roomObjects)
            {
                if (member.isAwake())
                {
                    member.updateChaosContribution();
                }

                totalChaos += member.ChaosContribution;
            }

            chaosLevel = totalChaos;
            //update the chaos meter
            ChaosMeter.i.updateChaosMeter(totalChaos);
            
            //now compare it to each chaos threhsold.
            for(int i = 0; i < chaosThresholds.Count; i++)
            {
                if(totalChaos >= chaosThresholds[i])
                {
                    bool hasEventAtIndex = i < thresholdEvents.Count;

                    if(hasEventAtIndex) //only do so if the thresholdEvents member exists
                    {
                        thresholdEvents[i]?.Invoke(); //invoke the unity event at that index
                        Debug.Log("Invoked chaos threshold event");
                    }
                    else
                    {
                        Debug.LogWarning($"Chaos threshold hit at index {i}, but no matching threshold event exists.");
                    }
                    
                    Debug.Log($"Chaos threshold of {chaosThresholds[i]} has been hit.");
                    //then remove the event and threshold
                    if(hasEventAtIndex)
                    {
                        thresholdEvents.RemoveAt(i);
                    }

                    chaosThresholds.RemoveAt(i);

                    i--; //decrement if the event was removed
                }
            }

            //TODO: Add UI feedback here as well

            yield return new WaitForSeconds(checkInterval);
        }
    }
}