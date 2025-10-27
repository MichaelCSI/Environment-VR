using UnityEngine;
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

    [Header("Objects to Fade (e.g. flowers)")]
    public SpriteRenderer[] objectsToFade;
    public float fadeSpeed = 0.4f; // how fast to fade objects in/out

    private bool garbageInBin = false;

    // Track individual fade coroutines for each SpriteRenderer
    private Dictionary<SpriteRenderer, Coroutine> fadeCoroutines = new Dictionary<SpriteRenderer, Coroutine>();

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

            foreach (var sr in objectsToFade)
            {
                if (sr == null) continue;

                // Stop existing coroutine for this SpriteRenderer (if any)
                if (fadeCoroutines.TryGetValue(sr, out var running))
                    StopCoroutine(running);

                // Start new fade for this SpriteRenderer
                fadeCoroutines[sr] = StartCoroutine(FadeSprite(sr, garbageInBin ? 1f : 0f));
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
}
