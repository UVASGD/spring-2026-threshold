using UnityEngine;

public class LockpickingSystem : MonoBehaviour
{
    //each lock should have a desired turn and a sweet spot
    [SerializeField] int numberOfPicks; //number of bones the player has to pick locks
    [SerializeField] GameObject boneModel;
    [SerializeField] Transform lockTransform;
    public bool AttemptLockPick(float inputDegree, float desiredDegree, float tolerance, float turnDegreeStep)
    {
        float allowedTurn;
        float distanceFromDesired = Mathf.Abs(desiredDegree - inputDegree);
        if(distanceFromDesired <= tolerance)
        {
            allowedTurn = 90; //90 degrees permitted
        }
        else
        {
            allowedTurn = 90 - (10 * (distanceFromDesired / turnDegreeStep));
        }

        //now rotate the lockpick that many degrees

        rotateLockPick();

        //now open the lock if the allowed turn is 90

        if(allowedTurn == 90)
        {
            return true;
        }
        else
        {
            return false; //the lockpick was not successfully done
        }
    }

    public void rotateLockPick()
    {
        
    }
}