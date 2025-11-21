using UnityEngine;
using ithappy.Animals_FREE;

public class DeerController : MonoBehaviour
{
    public CreatureMover mover;
    private bool shouldWalk = true;
    private bool shouldRun = false;

    // Debug
    public Animator animator;

    private void Start()
    {
        // Start with a walk
        mover.SetInput(
            axis: new Vector2(0, 1),
            target: transform.position + transform.forward,
            isRun: false,
            isJump: false
        );
        StartCoroutine(WalkCycle());
    }

    private System.Collections.IEnumerator WalkCycle()
    {
        // Walk into view
        yield return new WaitForSeconds(4f);

        // Idle full animation
        shouldWalk = false;
        shouldRun = false;
        yield return new WaitForSeconds(10.29f);

        // Run away
        shouldRun = true;
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
        // Vert handles "speed" --> ~0 when idle, ~1 when walking/running, State differentiates walk/run (~0 walk, ~1 run)
        // Debug.Log(animator.GetFloat("Vert"));
        // Debug.Log(animator.GetFloat("State"));
    }
}
