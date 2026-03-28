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

            //now compare it to each chaos threhsold.
            for(int i = 0; i < chaosThresholds.Count; i++)
            {
                if(totalChaos > chaosThresholds[i])
                {
                    if(thresholdEvents.Count > i + 1) //only do so if the thresholdEvents member exists
                        thresholdEvents[i]?.Invoke(); //invoke the unity event at that index
                    
                    Debug.Log($"Chaos threshold of {chaosThresholds[i]} has been hit.");
                    //then remove the event and threshold
                    thresholdEvents.Remove(thresholdEvents[i]);
                    chaosThresholds.Remove(chaosThresholds[i]);

                    i--; //decrement if the event was removed
                }
            }

            //TODO: Add UI feedback here as well

            yield return new WaitForSeconds(checkInterval);
        }
    }
}