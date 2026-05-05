using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CheckDialogue : MonoBehaviour
{
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private Dialogue dialogue2;
    [SerializeField] private Dialogue dialogue3;
    [SerializeField] private Dialogue dialogue4;

    public bool isDialogueActive = false;

    private void Update()
    {
        isDialogueActive = dialogue.isActiveAndEnabled || dialogue2.isActiveAndEnabled || dialogue3.isActiveAndEnabled || dialogue4.isActiveAndEnabled;
    }
}

