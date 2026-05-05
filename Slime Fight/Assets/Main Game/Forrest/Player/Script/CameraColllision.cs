using UnityEngine;
public class CameraCollision : MonoBehaviour
{
    public Transform cameraTransform;
    public float minDistance = 0.2f;
    public float maxDistance = 0.5f;
    public float smooth = 10f;
    float currentDistance;

    void Start()
    {
        currentDistance = maxDistance;
    }

    void LateUpdate()
    {
        Vector3 direction = cameraTransform.localPosition.normalized;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction, out hit, maxDistance))
            currentDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        else
            currentDistance = maxDistance;

        cameraTransform.localPosition = Vector3.Lerp(
            cameraTransform.localPosition,
            direction * currentDistance,
            Time.deltaTime * smooth
        );
    }
}
