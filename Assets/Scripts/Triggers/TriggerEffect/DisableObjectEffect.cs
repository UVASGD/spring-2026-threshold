using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObjectEffect : TriggerEffect
{
    [SerializeField] bool enable;
    [SerializeField] List<GameObject> objects;
    public override IEnumerator onTriggerEffect()
    {
        foreach(var item in objects)
        {
            item.gameObject.SetActive(enable);
        }

        yield return null;
    }
}