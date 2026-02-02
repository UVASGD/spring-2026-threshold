using UnityEngine;

public class TestCubeInteraction : MonoBehaviour, IInteract
{
    public Material materialA;
    public Material materialB;

    private MeshRenderer meshRenderer;
    private bool usingA = true;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = materialA;  
    }

    public void OnPlayerInteract()
    {
        // Swap the material
        usingA = !usingA;
        meshRenderer.material = usingA ? materialA : materialB;

        Debug.Log("Material swapped!");
    }
}