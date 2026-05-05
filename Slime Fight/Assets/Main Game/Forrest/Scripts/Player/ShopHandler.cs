using UnityEngine;

public class ShopHandler : MonoBehaviour
{
    public Canvas targetCanvas; 

    private void Start()
    {
        if (targetCanvas != null)
            targetCanvas.enabled = false; 
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (targetCanvas != null)
                targetCanvas.enabled = !targetCanvas.enabled; 
        }
    }
}