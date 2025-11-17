using UnityEngine;

public class XRGlowToggle : MonoBehaviour
{
    public Renderer targetRenderer;
    public Material highlightMaterial;
    private Material originalMaterial;

    void Start()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        originalMaterial = targetRenderer.material;
    }

    public void HighlightOn()
    {
        targetRenderer.material = highlightMaterial;
    }

    public void HighlightOff()
    {
        targetRenderer.material = originalMaterial;
    }
}
