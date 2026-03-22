using System;
using UnityEngine;
public class Hurtbox : MonoBehaviour
{
    public float damageDealt;

    [Header("For physics objects:")]
    [SerializeField] bool velocityDependent;
    [SerializeField] float minimumDamage;
    private Rigidbody rb;
    [Header("Velocity for maximum damage")]
    [SerializeField] float terminalVelocity;
    [SerializeField] AudioClip damageClip;
    void Start()
    {
        if(velocityDependent) rb = GetComponent<Rigidbody>();
    }
    public int dealDamage()
    {
        if(damageClip != null) PlayerSFX.i.PlaySFX(damageClip);
        if(!velocityDependent) return (int)damageDealt;
        else
        {
            //find what the velocity of the rigidbody is
            float velocity = rb.linearVelocity.magnitude;
            if(velocity > terminalVelocity) velocity = 1;
            else velocity = velocity / terminalVelocity;

            return Math.Max((int)(damageDealt * velocity), (int)minimumDamage); //either deal the minimum damage or the damage dealt, whichever is higher
        }   
    }
}