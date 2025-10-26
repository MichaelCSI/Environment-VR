
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;

public class BoundingBox : MonoBehaviour
{
    [Tooltip("Area where the sound will be active")]
    public Collider Area;

    [Tooltip("Player object to track, for XR rig, assign main camera")]
    public Transform rigTransform;

    [Tooltip("Reference to the XR Origin (NOT the camera)")]
    public XROrigin xrOrigin;

    void Update()
    {
        if (Area == null || rigTransform == null) return;

        Vector3 trackedPosition = rigTransform.position;
        Vector3 closestPoint = Area.ClosestPoint(trackedPosition);
        bool isOutside = Vector3.Distance(trackedPosition, closestPoint) > 0.01f;

        if (isOutside)
        {
            // Teleport player to nearest point within bounding box
            Vector3 nearestPoint = Area.ClosestPoint(trackedPosition);
            xrOrigin.MoveCameraToWorldLocation(nearestPoint);
        }
    }
}
