using UnityEngine;

[DefaultExecutionOrder(-100)]
public class EssentialObjects : MonoBehaviour
{
    public static EssentialObjects i;
    void Awake()
    {
        DontDestroyOnLoad(this);
        i = this;
    }
}
