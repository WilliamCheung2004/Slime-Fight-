using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Camera cam;
    private float xRotation = 0f;

    public float xSensitivity = 100f; // higher, feels natural without Time.deltaTime
    public float ySensitivity = 100f;

    public float smoothTime = 0.05f; // smoothing factor

    private Vector2 currentMouseDelta;
    private Vector2 smoothMouseDelta;
    private Vector2 smoothMouseVelocity;

    public void ProcessLook(Vector2 input)
    {
        // Apply sensitivity (without Time.deltaTime here)
        currentMouseDelta = input * new Vector2(xSensitivity, ySensitivity);

        // Smooth the input
        smoothMouseDelta = Vector2.SmoothDamp(smoothMouseDelta, currentMouseDelta, ref smoothMouseVelocity, smoothTime);

        // Vertical rotation
        xRotation -= smoothMouseDelta.y * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        // Horizontal rotation
        transform.Rotate(Vector3.up * smoothMouseDelta.x * Time.deltaTime);
    }
}