using UnityEngine;
using ithappy.Animals_FREE;

public class DeerController : MonoBehaviour
{
    public CreatureMover mover;

    [Header("Running / Gallop Audio")]
    public AudioSource audioSource;
    public AudioClip[] gallopClips;

    private bool shouldWalk = false;
    private bool shouldRun = false;
    private bool gallopPlaying = false;

    // For debug
    public Animator animator;


    public void StartDeerSequence()
    {
        StartCoroutine(WalkCycle());
    }


    private System.Collections.IEnumerator WalkCycle()
    {
        // Start with a walk
        shouldWalk = true;
        mover.SetInput(
            axis: new Vector2(0, 1),
            target: transform.position + transform.forward,
            isRun: false,
            isJump: false
        );
        yield return new WaitForSeconds(4f);

        // Idle animation
        shouldWalk = false;
        shouldRun = false;
        yield return new WaitForSeconds(9f);

        // Run away
        shouldRun = true;
    }

    private System.Collections.IEnumerator PlayGallopSounds()
    {
        gallopPlaying = true;

        // Need at least 2 clips to avoid repeats
        if (gallopClips.Length >= 2)
        {
            int first = Random.Range(0, gallopClips.Length);

            // Pick second random (ensure different from first)
            int second;
            do 
            {
                second = Random.Range(0, gallopClips.Length);
            }
            while (second == first);

            // Play first, wait, then play second
            audioSource.PlayOneShot(gallopClips[first]);
            yield return new WaitForSeconds(0.08f);
            audioSource.PlayOneShot(gallopClips[second]);
        }

        // Small cooldown before next gallop pair
        yield return new WaitForSeconds(0.3f);

        gallopPlaying = false;
    }


    private void Update()
    {
        if (shouldRun)
        {
            mover.SetInput(
                axis: new Vector2(0, 1),
                target: transform.position + transform.forward,
                isRun: true,
                isJump: false
            );
            // Play gallop audio
            if (!gallopPlaying && audioSource != null && gallopClips.Length > 1)
            {
                StartCoroutine(PlayGallopSounds());
            }
        }
        else if (shouldWalk)
        {
            mover.SetInput(
                axis: new Vector2(0, 1),
                target: transform.position + transform.forward,
                isRun: false,
                isJump: false
            );
        }
        else
        {
            mover.SetInput(
                axis: Vector2.zero,
                target: transform.position,
                isRun: false,
                isJump: false
            );
        }
        
        // Deer eventually runs off map, disable
        if (transform.position.y < -100)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

        // Vert handles "speed" --> ~0 when idle, ~1 when walking/running, State differentiates walk/run (~0 walk, ~1 run)
        // Debug.Log(animator.GetFloat("Vert"));
        // Debug.Log(animator.GetFloat("State"));
    }
}
