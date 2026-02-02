using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshFilter))]
public class LightSwitch : MonoBehaviour, IInteract, ISFXGenerator
{
    [SerializeField] bool switchOn = false;
    [SerializeField] List<Light> associatedLights;

    [SerializeField] Mesh onMesh;
    [SerializeField] Mesh offMesh;
    [SerializeField] AudioClip switchSFX;
    private MeshFilter mRenderer; //used to swap between on and off meshes
    private AudioSource audioSource;
    void Awake()
    {
        mRenderer = GetComponent<MeshFilter>();
        audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        if (switchOn)
        {
            mRenderer.mesh = onMesh;
        }
        else
        {
            mRenderer.mesh = offMesh;
        }
    }
    public void OnPlayerInteract()
    {
        ToggleSwitch();
    }
    public void ToggleSwitch()
    {
        switchOn = !switchOn;
        foreach(var lights in associatedLights)
        {
            lights.gameObject.SetActive(switchOn);
        }

        PlayLocalSFX(switchSFX);

        //set the mesh of the switch depending on toggled status
        if (switchOn)
        {
            mRenderer.mesh = onMesh;
        }
        else
        {
            mRenderer.mesh = offMesh;
        }
    }

    public void PlayLocalSFX(AudioClip clip)
    {
        PlayerSFX.i.PlaySFX(clip);
    }
}