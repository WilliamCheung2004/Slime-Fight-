using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InBounds : MonoBehaviour
{
    [SerializeField] private BoxCollider worldBounds;
    private Transform player;
    private float buffer = 0.01f;

    IEnumerator Start()
    {
        while (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                player = p.transform;
            }
            yield return null;
        }
    }

    void LateUpdate()
    {
        Bounds b = worldBounds.bounds;
        
        Vector3 pos = player.position;

        pos.x = Mathf.Clamp(pos.x, b.min.x - buffer, b.max.x + buffer);
        pos.y = Mathf.Clamp(pos.y, b.min.y - buffer, b.max.y + buffer);
        pos.z = Mathf.Clamp(pos.z, b.min.z - buffer, b.max.z + buffer);

        player.position = pos;
    }

}
