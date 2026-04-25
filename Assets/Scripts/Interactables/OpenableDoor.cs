using System.Collections;
using UnityEngine;

public class OpenableDoor : MonoBehaviour, IInteract, ISFXGenerator
{
    [SerializeField] private bool locked = false;
    [SerializeField] private int unlockID; //id of the key required for this door
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 3f;
    [SerializeField] private Vector3 hingeAxis = Vector3.up;
    [SerializeField] private bool invertOpenDirection = false;
    [SerializeField] private Transform hingeTransform = null;

    [Header("Audio")]
    [SerializeField] AudioSource doorSource;
    [SerializeField] AudioClip openDoor;
    [SerializeField] AudioClip closeDoor;
    [SerializeField] AudioClip doorRattle;
    [SerializeField] AudioClip doorUnlock;

    [SerializeField] string openMessage;
    [SerializeField] string lockedMessage = "Locked."; //if there needs to be a custom locked message for the door

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private float openSignedAngle = 0f;
    private float hingeAngleCurrent = 0f;
    private Coroutine activeDoorMotion;

    private void Start()
    {
        closedRotation = transform.localRotation;
        openSignedAngle = invertOpenDirection ? -openAngle : openAngle;
        openRotation = closedRotation * Quaternion.AngleAxis(openSignedAngle, hingeAxis.normalized);
        hingeAngleCurrent = 0f;

        if (hingeTransform != null)
        {
            float dist = Vector3.Distance(hingeTransform.position, transform.position);
        }
    }

    public void OnPlayerInteract()
    {
        if (locked)
        {
            if (CheckKeys())
            {
                locked = false;
                Debug.Log($"Door id {unlockID} is now unlocked");
                PlayLocalSFX(doorUnlock);
                if (!openMessage.Equals("")) //if the message is not blank
                {
                    ShowMessage(openMessage, 1);
                }
                return;
            }
            else
            {
                PlayLocalSFX(doorRattle);
                ShowMessage(lockedMessage, 2);
                return;
            }
        }

        isOpen = !isOpen; // toggle state

        if (isOpen)
        {
            PlayLocalSFX(openDoor);
        }
        else
        {
            PlayLocalSFX(closeDoor);
        }

        if (hingeTransform != null)
        {
            StartDoorMotion(RotateDoorAroundHinge(isOpen ? openSignedAngle : 0f));
        }
        else
        {
            StartDoorMotion(RotateDoor(isOpen ? openRotation : closedRotation));
        }
    }

    private void ShowMessage(string message, int seconds)
    {
        if (MessageManager.i == null || string.IsNullOrEmpty(message)) return;
        MessageManager.i.StartCoroutine(MessageManager.i.DisplayText(message, seconds));
    }

    private void StartDoorMotion(IEnumerator routine)
    {
        if (activeDoorMotion != null)
        {
            StopCoroutine(activeDoorMotion);
        }

        activeDoorMotion = StartCoroutine(routine);
    }
    private bool CheckKeys()
    {
        //check the playerkeys singleton and see if the key is owned
        if (PlayerKeys.i.CheckUnlock(unlockID))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private IEnumerator RotateDoor(Quaternion targetRot)
    {
        while (Quaternion.Angle(transform.localRotation, targetRot) > 0.1f)
        {
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                targetRot,
                Time.deltaTime * openSpeed
            );
            yield return null;
        }

        transform.localRotation = targetRot; // snap to final precise rotation
    }

    private IEnumerator RotateDoorAroundHinge(float targetAngle)
    {
        // targetAngle is the signed angle from closed (0) position
        float remaining = targetAngle - hingeAngleCurrent;

        Vector3 axisWorld;
        if (hingeTransform != null)
        {
            if (hingeAxis == Vector3.up) axisWorld = hingeTransform.up;
            else if (hingeAxis == Vector3.right) axisWorld = hingeTransform.right;
            else if (hingeAxis == Vector3.forward) axisWorld = hingeTransform.forward;
            else axisWorld = hingeTransform.TransformDirection(hingeAxis.normalized);
        }
        else
        {
            axisWorld = transform.TransformDirection(hingeAxis.normalized);
        }

        // interpret openSpeed as a speed factor; convert to degrees/sec for RotateAround
        float degreesPerSecond = openSpeed * 60f;

        while (Mathf.Abs(remaining) > 0.1f)
        {
            float step = Mathf.Sign(remaining) * degreesPerSecond * Time.deltaTime;
            if (Mathf.Abs(step) > Mathf.Abs(remaining)) step = remaining;

            transform.RotateAround(hingeTransform.position, axisWorld, step);
            hingeAngleCurrent += step;
            remaining = targetAngle - hingeAngleCurrent;
            yield return null;
        }

        // snap final remaining rotation
        if (Mathf.Abs(remaining) > 0.001f)
        {
            transform.RotateAround(hingeTransform.position, axisWorld, remaining);
            hingeAngleCurrent = targetAngle;
        }
    }

    public void PlayLocalSFX(AudioClip clip)
    {
        if (doorSource == null || clip == null) return;
        PlayerSFX.i.PlaySFX(clip);
    }
    public void ForceCloseDoor()
    {
        //used to close a door automatically from a trigger
        if (!isOpen) return;
        else
        {
            isOpen = false;
            if (hingeTransform != null)
            {
                PlayLocalSFX(closeDoor);
                StartDoorMotion(RotateDoorAroundHinge(isOpen ? openSignedAngle : 0f));
            }
        }
    }
}