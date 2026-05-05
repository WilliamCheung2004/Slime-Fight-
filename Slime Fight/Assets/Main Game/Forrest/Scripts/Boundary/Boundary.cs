using UnityEngine;
using UnityEngine.Splines;

public class SplineBoundaryWalls : MonoBehaviour
{
    public SplineContainer spline;
    public GameObject wallPrefab;
    public int segments = 200;

    [Header("Wall Offset")]
    public float offset = 0.5f; 

    void Start()
    {
        for (int i = 0; i < segments; i++)
        {
            float t1 = i / (float)segments;
            float t2 = (i + 1) / (float)segments;

            Vector3 p1 = spline.EvaluatePosition(t1);
            Vector3 p2 = spline.EvaluatePosition(t2);

            Vector3 dir = (p2 - p1).normalized; 
            Vector3 normal = Vector3.Cross(dir, Vector3.up).normalized;
            Vector3 mid = (p1 + p2) * 0.5f + normal * offset;

            GameObject wall = Instantiate(wallPrefab, mid, Quaternion.identity, transform);

            wall.transform.forward = dir;

            float distance = Vector3.Distance(p1, p2);
            wall.transform.localScale = new Vector3(1, 5, distance);
        }
    }
}