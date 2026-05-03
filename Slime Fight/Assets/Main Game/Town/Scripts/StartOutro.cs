using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartOutro : MonoBehaviour
{
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private GameObject outro;
   

    public void StartOutroSequence()
    {
        outro.SetActive(true);
    }

}
