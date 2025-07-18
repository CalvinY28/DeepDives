using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxScript : MonoBehaviour
{
    public Transform cameraTransform;
    public float parallaxFactor = 0.5f;

    public Vector2 minPosition;
    public Vector2 maxPosition;

    private Vector3 startPosition;

    void Start()
    {
        if (cameraTransform == null)
        {
            Camera cam = GameObject.FindObjectOfType<Camera>();
            if (cam != null)
                cameraTransform = cam.transform;
            else
                Debug.LogWarning("No camera found in scene!");
        }

        startPosition = transform.position;
    }

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        Vector3 cameraDelta = cameraTransform.position;
        Vector3 targetPos = new Vector3(
            startPosition.x + cameraDelta.x * parallaxFactor,
            startPosition.y + cameraDelta.y * parallaxFactor,
            transform.position.z
        );

        // Clamp the position to stay within limits
        targetPos.x = Mathf.Clamp(targetPos.x, minPosition.x, maxPosition.x);
        targetPos.y = Mathf.Clamp(targetPos.y, minPosition.y, maxPosition.y);

        transform.position = targetPos;
    }
}
