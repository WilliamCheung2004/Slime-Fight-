using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KingdomScene : MonoBehaviour
{
    [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private GameObject Outro;

    bool canContinue = false;
    bool playedAnimation = false;

    void Start()
    {
        StartCoroutine(ActivateCanvas(1f));
    }

    void Update()
    {
        if(dialogue.currentAnimation == "Walk" && !playedAnimation && dialogue.lineFinished)
        {
            dialogue.canAdvanceText = false;
            playerAnimator.SetBool("Walk", true);
            StartCoroutine(Wait(1));
        }

        if (dialogue.textFinished)
        {
            Outro.SetActive(true);
        }
    }

    void ContinueDialogue()
    {
        dialogueCanvas.SetActive(true);
        dialogue.NextLine();
        dialogue.canAdvanceText = true;
    }

    public void LoadHome()
    {
        SceneManager.LoadScene("Home");
    }

    IEnumerator ActivateCanvas(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        dialogueCanvas.SetActive(true);
    }

    IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        dialogueCanvas.SetActive(false);
        playedAnimation = true;
    }
}
