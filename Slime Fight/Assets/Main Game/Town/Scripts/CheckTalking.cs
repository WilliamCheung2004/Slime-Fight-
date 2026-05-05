using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTalking : MonoBehaviour
{
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private string currentCharacter;
    [SerializeField] private Animator characterAnimator;

    public void Update()
    {
        CheckSpeaking();
    }
    private void CheckSpeaking()
    {
        if (dialogue.currentSpeaker == currentCharacter)
        {
           characterAnimator.SetBool("Talking", true);
        }
        else
        {
          characterAnimator.SetBool("Talking", false);
        }
    }

}
