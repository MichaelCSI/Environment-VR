using UnityEngine;
using System.Collections;

public class FishJump : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpHeight = 1f;
    public float jumpDuration = 1f;
    public float sideTilt = 90f; // degrees tilt mid-air

    [Header("Jump Timing & Random Offset")]
    public float interval = 5f; // seconds between jumps
    public Vector2 startXOffsetRange = new Vector2(-2.0f, 2.0f);
    public Vector2 startZOffsetRange = new Vector2(-2.0f, 2.0f);
    public Vector2 targetXOffsetRange = new Vector2(-2f, 2f);
    public Vector2 targetZOffsetRange = new Vector2(-2f, 2f);

    [Header("Audio")]
    public AudioClip[] jumpSounds; // audio clips to play
    private AudioSource audioSource;

    private Vector3 startPos;
    private Quaternion startRot;


    // Fish gets enabled when garbage is out of water
    void OnEnable()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        audioSource = GetComponent<AudioSource>();

        StartCoroutine(JumpLoop());
    }
    void OnDisable()
    {
        StopAllCoroutines();
    }



    IEnumerator JumpLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);

            // Start jump at random position near origin
            float startX = Random.Range(startXOffsetRange.x, startXOffsetRange.y);
            float startZ = Random.Range(startZOffsetRange.x, startZOffsetRange.y);
            Vector3 jumpStartPos = startPos + new Vector3(startX, 0, startZ);

            // Pick random target to land to, ensure distance is not too small
            float targetX = Random.Range(targetXOffsetRange.x, targetXOffsetRange.y);
            float targetZ = Random.Range(targetZOffsetRange.x, targetZOffsetRange.y);
            Vector3 targetPos = jumpStartPos + new Vector3(targetX, 0, targetZ);
            
            // If target is too close, just jump a bit past start pos
            if (Vector3.Distance(jumpStartPos, targetPos) < 0.2f)
            {
                targetPos = jumpStartPos + new Vector3(0.2f, 0f, 0.2f);
            }

            yield return StartCoroutine(JumpRoutine(jumpStartPos, targetPos));
        }
    }

    IEnumerator JumpRoutine(Vector3 start, Vector3 target)
    {
        AudioClip startClip = null;
        int startIndex = -1;

        // Play jump sound
        if (jumpSounds.Length > 0)
        {
            startIndex = Random.Range(0, jumpSounds.Length);
            startClip = jumpSounds[startIndex];
            audioSource.PlayOneShot(startClip);
        }

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / jumpDuration;

            // Lerp horizontal position
            Vector3 horizontalPos = Vector3.Lerp(start, target, t);

            // Parabolic height
            float height = 4 * jumpHeight * t * (1 - t);

            transform.position = horizontalPos + Vector3.up * height;

            // Rotation tilt
            float tilt = Mathf.Sin(t * Mathf.PI) * sideTilt;
            transform.rotation = startRot * Quaternion.Euler(0, 0, tilt);

            yield return null;
        }

        // Landing sound
        if (jumpSounds.Length > 1)
        {
            int endIndex;
            do { endIndex = Random.Range(0, jumpSounds.Length); }
            while (endIndex == startIndex);

            audioSource.PlayOneShot(jumpSounds[endIndex]);
        }

        // Reset only rotation (keep position)
        transform.rotation = startRot;
    }

}
