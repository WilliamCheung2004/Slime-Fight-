using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObject : MonoBehaviour
{
    [SerializeField] GameObject objectToDisable;
    public void Disable()
    {
        objectToDisable.SetActive(false);
    }
}
