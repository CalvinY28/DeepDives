using UnityEngine;

public class ParalaxScript : MonoBehaviour
{
    public Transform cameraTransform;
    public float parallaxFactor = 0.5f;

    public Vector2 minPosition;
    public Vector2 maxPosition;

    private Vector3 startPosition;
    private Vector3 lastCameraPosition;
    private bool hasCameraPosition;

    private void Start()
    {
        if (cameraTransform == null)
        {
            Camera sceneCamera = Camera.main;
            if (sceneCamera == null)
                sceneCamera = FindFirstObjectByType<Camera>();

            if (sceneCamera != null)
                cameraTransform = sceneCamera.transform;
        }

        if (cameraTransform == null)
        {
            Debug.LogWarning("No camera found in scene.", this);
            enabled = false;
            return;
        }

        startPosition = transform.position;
        UpdateParallaxPosition();
    }

    private void LateUpdate()
    {
        if (cameraTransform == null)
        {
            enabled = false;
            return;
        }

        if (hasCameraPosition && cameraTransform.position == lastCameraPosition)
            return;

        UpdateParallaxPosition();
    }

    private void UpdateParallaxPosition()
    {
        Vector3 cameraPosition = cameraTransform.position;
        lastCameraPosition = cameraPosition;
        hasCameraPosition = true;

        Vector3 targetPosition = new Vector3(
            startPosition.x + cameraPosition.x * parallaxFactor,
            startPosition.y + cameraPosition.y * parallaxFactor,
            startPosition.z
        );

        targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x, maxPosition.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);

        transform.position = targetPosition;
    }
}
