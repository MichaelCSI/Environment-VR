using UnityEngine;

public class GarbageInWater : MonoBehaviour
{
    [Header("Object Transforms (1 or more fish)")]
    public Transform[] fishTransforms;

    [Header("Buoyancy Settings")]
    public Transform waterSurface;       // Reference to water plane transform
    public float bounceDamping = 9.815f; // Buoyancy force
    public float waterLevelOffset = 2.67f;
    public float raycastDistance = 5f;

    [Header("Water Settings")]
    public Material waterMaterial;       // Water material that changes color
    public string deepColorProperty = "Color_7D9A58EC"; // Shader property name
    public Color startColor = new Color(0.11f, 0.4f, 0.13f); // Default color, greenish
    public Color cleanColor = new Color(0.11f, 0.22f, 0.4f); // More blue when garbage is out of water
    public float colorLerpSpeed = 0.01f; // Transition speed between colors

    private Rigidbody sodaRigidBody;

    void Start()
    {
        SetAllFishActive(false);
        sodaRigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float waterLevel = waterSurface.position.y + waterLevelOffset;
        float depth = waterLevel - transform.position.y;

        // Apply buoyancy when below the water surface
        if (depth > 0f)
        {
            float depthFactor = Mathf.Clamp01(depth / 0.05f);
            sodaRigidBody.AddForce(Vector3.up * bounceDamping * depthFactor, ForceMode.Acceleration);
        }

        // Raycast downward to detect water presence, if no ray hits, assume clean
        bool canInWater = false;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, raycastDistance))
        {
            if (hit.collider.CompareTag("water"))
            {
                ResetWaterColor();
                canInWater = true;
            }
            else
            {
                CleanWaterColor();
            }
        }
        else
        {
            CleanWaterColor();
        }

        // Update fish status based on if can is in the water
        if (canInWater)
        {
            SetAllFishActive(false);
        }
        else
        {
            SetAllFishActive(true);
        }
    }

    void CleanWaterColor()
    {
        if (waterMaterial == null || !waterMaterial.HasProperty(deepColorProperty)) return;

        // Smoothly lerp from startColor to cleanColor
        Color current = waterMaterial.GetColor(deepColorProperty);
        Color target = cleanColor;
        Color lerped = Color.Lerp(current, target, colorLerpSpeed);
        waterMaterial.SetColor(deepColorProperty, lerped);
    }

    void ResetWaterColor()
    {
        if (waterMaterial == null || !waterMaterial.HasProperty(deepColorProperty)) return;

        // Smoothly lerp back to startColor
        Color current = waterMaterial.GetColor(deepColorProperty);
        Color target = startColor;
        Color lerped = Color.Lerp(current, target, colorLerpSpeed);
        waterMaterial.SetColor(deepColorProperty, lerped);
    }

    // Enable/disable all fish
    private void SetAllFishActive(bool active)
    {
        if (fishTransforms == null) return;

        foreach (Transform fish in fishTransforms)
        {
            if (fish != null && fish.gameObject.activeSelf != active)
                fish.gameObject.SetActive(active);
        }
    }
}
