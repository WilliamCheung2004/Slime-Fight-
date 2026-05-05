using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class StartDialogue : MonoBehaviour
{
    [SerializeField] private GameObject Dialogue;

    public void ActivatePanel()
    {
        if (Dialogue != null)
        {
            Dialogue.SetActive(true);
        }
        else
        {
            Debug.LogError("Dialogue GameObject is not assigned in the inspector.");
        }
    }
}
