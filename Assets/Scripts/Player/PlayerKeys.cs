using System.Collections.Generic;
using UnityEngine;

public class PlayerKeys : MonoBehaviour
{
    public static PlayerKeys i;
    void Awake()
    {
        if(i == null)
        {
            i = this;
        }
    }
    
    //playerKeys stores a list of DoorKey items (ints) that are compared with the door's unlockID to determine if the player can unlock the door
    [SerializeField] List<DoorKey> ownedKeys = new List<DoorKey>();

    public bool CheckUnlock(int unlockID)
    {
        foreach(var key in ownedKeys)
        {
            if(key.keyID == unlockID && !key.used)
            {
                //mark the key used if it is unreuseable
                if (!key.reuseable)
                {
                    key.used = true;
                }
                return true;
            }
        }

        return false;
    }

    public void KeyPickup(DoorKey newKey)
    {
        ownedKeys.Add(newKey);
    }
}

[System.Serializable]
public class DoorKey
{
    public bool reuseable {get; private set;} //this allows for branching paths, where only a certain number of doors are unlockable
    public bool used;
    public int keyID;
    public DoorKey(int keyID)
    {
        this.keyID = keyID;
        reuseable = true;
    }
}