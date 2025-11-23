using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;
using System.Collections.Generic;


public class GarbageInside : MonoBehaviour
{
    [Header("Target to check (e.g., the object you're detecting)")]
    public Transform target;

    [Header("Detection Settings")]
    public float detectionRadius = 0.3f;   // Horizontal distance from the can’s center
    public float heightTolerance = 0.5f;   // Vertical range allowed
    public float prefabHeightOffset = 0.2f;       // Offset for starting height 

    [Header("Sprites to Fade (e.g. flowers) when object enters bin")]
    public SpriteRenderer[] objectsToFade;
    public float fadeSpeed = 0.4f; // how fast to fade objects in/out

    [Header("Optional Audio to play when object enters bin")]
    public AudioSource audioSource;    // Optional audio source

    [Header("Post Processing (URP) (for 1st bin only)")]
    public Volume postProcessVolume;  // Assign your Global Volume
    private ColorAdjustments colorAdjustments;
    public float colorFadeSpeed = 2f; // How fast saturation changes

    [Header("Deer Controller (for 3rd bin only)")]
    public DeerController deer;


    private bool garbageInBin = false;

    // Track individual object fade and audio coroutines
    private Dictionary<SpriteRenderer, Coroutine> fadeCoroutines = new Dictionary<SpriteRenderer, Coroutine>();
    private Coroutine saturationCoroutine;

    void Start()
    {
        // Grab Color Adjustments from the Volume
        if (postProcessVolume != null)
        {
            if (postProcessVolume.profile.TryGet(out ColorAdjustments ca))
            {
                colorAdjustments = ca;
                // Start scene in black & white
                colorAdjustments.saturation.value = -100f;
            }
            else
            {
                Debug.LogWarning("Color Adjustments override missing from Volume.");
            }
        }
    }


    void Update()
    {
        if (target == null) return;

        // Horizontal (XZ-plane) distance
        Vector3 horizontalCanPos = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 horizontalTargetPos = new Vector3(target.position.x, 0f, target.position.z);
        float horizontalDistance = Vector3.Distance(horizontalCanPos, horizontalTargetPos);

        // Vertical (Y-axis) difference
        float canCenterY = transform.position.y + prefabHeightOffset;
        float heightDifference = Mathf.Abs(target.position.y - canCenterY);

        bool inBin = horizontalDistance < detectionRadius && heightDifference < heightTolerance && target.position.y > canCenterY;

        // If garbage enters the bin
        if (inBin != garbageInBin)
        {
            garbageInBin = inBin;

            // Fade in objects
            foreach (var sr in objectsToFade)
            {
                if (sr == null) continue;

                // Stop existing coroutine for this SpriteRenderer (if any)
                if (fadeCoroutines.TryGetValue(sr, out var running))
                    StopCoroutine(running);

                // Start new fade for this SpriteRenderer
                fadeCoroutines[sr] = StartCoroutine(FadeSprite(sr, garbageInBin ? 1f : 0f));
            }

            // Play audio if present
            if (audioSource != null)
            {
                audioSource.Play();
            }

            // Trigger saturation change (black & white to color)
            if (colorAdjustments != null)
            {
                if (saturationCoroutine != null)
                    StopCoroutine(saturationCoroutine);

                saturationCoroutine = StartCoroutine(LerpSaturation(garbageInBin ? 0f : -100f));
            }

            // For 3rd bin, trigger deer sequence when garbage is in bin
            if (garbageInBin && deer != null)
            {
                deer.StartDeerSequence();
            }
        }
    }

    System.Collections.IEnumerator FadeSprite(SpriteRenderer sr, float targetAlpha)
    {
        Color color = sr.color;
        while (!Mathf.Approximately(color.a, targetAlpha))
        {
            color.a = Mathf.MoveTowards(color.a, targetAlpha, fadeSpeed * Time.deltaTime);
            sr.color = color;
            yield return null;
        }
    }

    IEnumerator LerpSaturation(float target)
    {
        float start = colorAdjustments.saturation.value;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * colorFadeSpeed;
            colorAdjustments.saturation.value = Mathf.Lerp(start, target, t);
            yield return null;
        }

        colorAdjustments.saturation.value = target;
    }

}
