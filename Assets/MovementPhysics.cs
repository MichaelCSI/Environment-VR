using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(CharacterController))]
public class MovementPhysics : MonoBehaviour
{
    [Header("References")]
    public Transform rigTransform; // Assign the Main Camera of the XR Rig here
    public float raycastDistance = 2f;
    public float slideSpeed = 3f;
    public float slideThreshold = 10f;

    // Not using for now, using transform directly
    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Physics.Raycast(rigTransform.position, Vector3.down, out RaycastHit hit, raycastDistance))
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

            // Steep slippery surface, slide down (e.g. world boundary)
            if (hit.collider.CompareTag("slippery") && slopeAngle > slideThreshold)
            {
                // Normalize slope so it's 0 at threshold and 1 at 90 degrees - adjust slide speed based on slope
                float slopeFactor = Mathf.InverseLerp(slideThreshold, 90f, slopeAngle);
                Vector3 slideDirection = Vector3.ProjectOnPlane(Vector3.down, hit.normal).normalized;
                transform.position += slideDirection * (slideSpeed * slopeFactor) * Time.deltaTime;
            }

            // Going uphill - shift rig upwards to account for Y difference, account for camera offset
            Vector3 targetPosition = transform.position;
            float targetY = hit.point.y - rigTransform.localPosition.y + 1.3f;
            if(targetY > targetPosition.y)
            {
                if(hit.collider.CompareTag("water")){
                    // "Waist-deep" for water
                    targetPosition.y = Mathf.Lerp(transform.position.y, targetY - 0.3f, 0.2f);
                    transform.position = targetPosition;
                } else {
                    targetPosition.y = Mathf.Lerp(transform.position.y, targetY, 0.2f);
                    transform.position = targetPosition;
                }
            }
        }
    }
}