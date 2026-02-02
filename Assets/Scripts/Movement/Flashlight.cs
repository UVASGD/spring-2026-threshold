using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class Flashlight : MonoBehaviour
{
    public static Flashlight i;
    [Header("Light")]
    public Light flashlightLight;

    [Header("Battery")]
    public float maxBattery = 100f;
    public float batteryDrainPerSecond = 1f;
    public bool startOn = false;

    [Header("Flicker")]
    public bool enableFlicker = true;
    public float lowBatteryThreshold = 20f; // percent of maxBattery
    public float flickerChancePerSecond = 0.2f;
    public float flickerDuration = 0.08f;

    [Header("Audio")]
    public AudioClip toggleOnClip;
    public AudioClip toggleOffClip;
    public AudioClip emptyClip;

    private AudioSource audioSource;
    private Coroutine flickerCoroutine;
    [SerializeField] private float battery; //for editor visualization
    private bool isOn = false;
    private bool isEmpty = false;

    [Header("Interface")]
    [SerializeField] List<Sprite> flashlightSprites;
    [SerializeField] List<Sprite> batterySprites;
    [SerializeField] Image lightIndicator;
    [SerializeField] Image batteryIndicator;

    void Awake()
    {
        if(i==null)
        {
            i = this;
        }
        
        audioSource = GetComponent<AudioSource>();
        if (flashlightLight == null)
            flashlightLight = GetComponentInChildren<Light>();

        BatteryPickup.onBatteryPickup += TopOffBattery;

        battery = maxBattery;
        isOn = startOn;
        ApplyLight();
    }

    private PlayerInputActions input;

    void OnEnable()
    {
        if (input == null) input = new PlayerInputActions();
        input.Player.Enable();
        input.Player.Flashlight.performed += OnFlashlightPerformed;
    }

    void OnDisable()
    {
        if (input != null)
        {
            input.Player.Flashlight.performed -= OnFlashlightPerformed;
            input.Player.Disable();
            input.Dispose();
            input = null;
        }
    }

    private void OnFlashlightPerformed(InputAction.CallbackContext ctx)
    {
        Toggle();
    }

    void Update()
    {
        if (isOn && !isEmpty)
        {
            battery -= batteryDrainPerSecond * Time.deltaTime;
            batteryIndicator.sprite = batterySprites[(int)BatteryPercent / 10]; //Divide by 10 to determine appropriate sprite

            if (battery <= 0f)
            {
                battery = 0f;
                isEmpty = true;
                TurnOff();
                if (audioSource != null && emptyClip != null)
                    audioSource.PlayOneShot(emptyClip);
            }
            else if (enableFlicker && battery <= (maxBattery * lowBatteryThreshold / 100f))
            {
                if (flickerCoroutine == null)
                    flickerCoroutine = StartCoroutine(LowBatteryFlicker());
            }
        }
        else
        {
            if (flickerCoroutine != null)
            {
                StopCoroutine(flickerCoroutine);
                flickerCoroutine = null;
            }
        }
    }

    public void Toggle()
    {
        if (isOn) TurnOff(); else TurnOn();
    }

    public void TurnOn()
    {
        lightIndicator.sprite = flashlightSprites[0];
        if (isEmpty)
        {
            if (audioSource != null && emptyClip != null)
                audioSource.PlayOneShot(emptyClip);
            return;
        }

        isOn = true;
        ApplyLight();
        if (audioSource != null && toggleOnClip != null)
            audioSource.PlayOneShot(toggleOnClip);
    }

    public void TurnOff()
    {
        lightIndicator.sprite = flashlightSprites[1];
        isOn = false;
        ApplyLight();
        if (audioSource != null && toggleOffClip != null)
            audioSource.PlayOneShot(toggleOffClip);
    }

    private void ApplyLight()
    {
        if (flashlightLight != null)
            flashlightLight.enabled = isOn;
    }

    private IEnumerator LowBatteryFlicker()
    {
        while (isOn && !isEmpty)
        {
            if (Random.value < flickerChancePerSecond * Time.deltaTime)
            {
                if (flashlightLight != null)
                    flashlightLight.enabled = false;

                yield return new WaitForSeconds(flickerDuration);

                if (flashlightLight != null)
                    flashlightLight.enabled = true;
            }
            yield return null;
        }
        isOn = false;
        lightIndicator.sprite = flashlightSprites[1];
        flickerCoroutine = null;
    }

    public void Recharge(float amount)
    {
        battery = Mathf.Clamp(battery + amount, 0f, maxBattery);
        if (battery > 0f) isEmpty = false;
    }

    public float BatteryPercent => maxBattery > 0f ? (battery / maxBattery) * 100f : 0f;
    public void TopOffBattery()
    {
        battery = maxBattery;
        isEmpty = false;
        batteryIndicator.sprite = batterySprites[10]; //reset indicator
    }
}
