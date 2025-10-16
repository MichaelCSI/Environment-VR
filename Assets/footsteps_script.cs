using UnityEngine;

public class XRFootsteps : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("Step Timing")]
    public float stepInterval = 0.5f;
    public float minSpeed = 0.1f;

    [Header("Surface Clips")]
    public AudioClip[] defaultClips;
    public AudioClip[] waterClips;
    public AudioClip[] grassClips;

    [Header("References")]
    public Transform rigTransform; // Assign the Main Camera of the XR Rig here
    public float raycastDistance = 5f;

    private float stepTimer;
    private Vector3 lastPosition;
    private string currentSurface = "default";
    private AudioClip lastClip; // track last played clip

    void Start()
    {
        lastPosition = rigTransform.position;
    }

    void Update()
    {
        // Calculate horizontal movement
        Vector3 currentPosition = rigTransform.position;
        Vector3 horizontalDelta = new Vector3(
            currentPosition.x - lastPosition.x,
            0,
            currentPosition.z - lastPosition.z
        );

        float speed = horizontalDelta.magnitude / Time.deltaTime;
        lastPosition = currentPosition;

        // Detect surface
        DetectSurfaceBelow();

        // Play footsteps
        if (speed > minSpeed)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    // Cast ray below player, get tag of ray collision object/surface
    void DetectSurfaceBelow()
    {
        if (Physics.Raycast(rigTransform.position, Vector3.down, out RaycastHit hit, raycastDistance))
        {
            currentSurface = hit.collider.tag;
        }
        else
        {
            currentSurface = "default";
        }
    }

    void PlayFootstep()
    {
        AudioClip[] clipsToUse;

        switch (currentSurface)
        {
            case "water":
                clipsToUse = waterClips;
                break;
            case "grass":
                clipsToUse = grassClips;
                break;
            default:
                clipsToUse = defaultClips;
                break;
        }

        if (clipsToUse == null || clipsToUse.Length == 0) return;

        AudioClip clipToPlay;

        // If only one clip, use it
        if (clipsToUse.Length == 1)
        {
            clipToPlay = clipsToUse[0];
        }
        else
        {
            // Pick a random clip that isn’t the same as last time
            do
            {
                clipToPlay = clipsToUse[Random.Range(0, clipsToUse.Length)];
            }
            while (clipToPlay == lastClip);
        }

        audioSource.PlayOneShot(clipToPlay);
        lastClip = clipToPlay;
    }
}
