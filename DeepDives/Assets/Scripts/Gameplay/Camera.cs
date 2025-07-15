using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Smooth Speeds")]
    public float landSmoothSpeed = 5f;
    public float waterSmoothSpeed = 2f;

    [Header("Deadzone")]
    public Vector2 deadzoneSize = new Vector2(2f, 1.5f); // Width & height of the deadzone box

    private PlayerMovement playerMovement;

    void Start()
    {
        if (target != null)
            playerMovement = target.GetComponent<PlayerMovement>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        float smoothSpeed = landSmoothSpeed;
        if (playerMovement != null && playerMovement.IsSwimming())
        {
            smoothSpeed = waterSmoothSpeed;
        }

        Vector3 cameraPos = transform.position;
        Vector3 targetPos = target.position + offset;

        Vector2 delta = targetPos - cameraPos;

        // Only update if outside the deadzone
        if (Mathf.Abs(delta.x) > deadzoneSize.x || Mathf.Abs(delta.y) > deadzoneSize.y)
        {
            // Clamp to the edge of the deadzone
            float moveX = Mathf.Abs(delta.x) > deadzoneSize.x
                ? delta.x - Mathf.Sign(delta.x) * deadzoneSize.x
                : 0f;

            float moveY = Mathf.Abs(delta.y) > deadzoneSize.y
                ? delta.y - Mathf.Sign(delta.y) * deadzoneSize.y
                : 0f;

            Vector3 moveDelta = new Vector3(moveX, moveY, 0f);
            Vector3 newPos = cameraPos + moveDelta;
            transform.position = Vector3.Lerp(cameraPos, newPos, smoothSpeed * Time.deltaTime);
        }
    }
}
