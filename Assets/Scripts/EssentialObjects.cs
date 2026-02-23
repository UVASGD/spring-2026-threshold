using UnityEngine;

public class EssentialObjects : MonoBehaviour
{
    public static EssentialObjects i;
    void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
