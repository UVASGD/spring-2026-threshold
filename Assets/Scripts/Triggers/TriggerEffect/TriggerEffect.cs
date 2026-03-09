using System;
using UnityEngine;
[Serializable]
public abstract class TriggerEffect
{
    [SerializeField] string effectName;
    /// <summary>
    /// onTriggerEffect is an abstract function that is customized and overwritten by each triggereffect
    /// </summary>
    public abstract void onTriggerEffect();
}