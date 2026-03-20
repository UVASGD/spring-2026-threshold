using System;
using UnityEngine;
public class Hurtbox : MonoBehaviour
{
    public float damageDealt;

    [Header("For physics objects:")]
    [SerializeField] bool velocityDependent;
    [SerializeField] float minimumDamage;
    [SerializeField] float terminalVelocity;
    public int dealDamage()
    {
        if(!velocityDependent) return (int)damageDealt;
        else
        {
            //find what the velocity 

            return (int)minimumDamage;
        }   
    }
}