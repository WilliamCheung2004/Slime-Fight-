using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deactivate : MonoBehaviour
{
    [SerializeField] private GameObject Object;

   private void DeactivateObject()
    {
               Object.SetActive(false);
    }
}

