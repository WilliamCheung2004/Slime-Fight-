using System.Threading;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class ThrowWeapon : MonoBehaviour
{
    [SerializeField] private GameObject bow;
    [SerializeField] private GameObject sword;
    [SerializeField] private Animator swordAnimator;
    [SerializeField] private Animator bowAnimator;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private GameObject outro;
    [SerializeField] private SaveManager save;
    private bool weaponPlayed = false;
    private bool movementPlayed = false;
    void Start()
    {
        save.CheckSave();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Current animation: " + dialogue.currentAnimation);
        if (!weaponPlayed || !movementPlayed && dialogue.currentAnimation != null)
        {
            PlayAnimation();
        }
    }

    //Play animation given selection
    public void PlayAnimation()
    {
        if (dialogue.currentAnimation == "Throwing" && !weaponPlayed)
        {
            if (save.currentData.playerType == "Sword")
            {
                sword.SetActive(true);
                Debug.Log("THOWING SWORD");
                swordAnimator.SetBool("Throwing", true);
                weaponPlayed = true;
                StopAnimation();
            }
            else
            {
                if (save.currentData.playerType == "Bow")
                {
                    bow.SetActive(true);
                    Debug.Log("THOWING BOW");
                    bowAnimator.SetBool("Throwing", true);
                    weaponPlayed = true;
                    StopAnimation();
                }
            }
        }
        else
        {
            if (dialogue.currentAnimation == "Moving" && !movementPlayed)
            {
                Debug.Log("Playing movement animation");
                playerAnimator.SetBool("Moving", true);
                movementPlayed = true;
                StopAnimation();
            }
        }
    }

    public void StopAnimation()
    {
        //and animation has finished, stop animation and hide weapon
        if (save.currentData.playerType == "Sword" && swordAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            sword.SetActive(false);
            swordAnimator.SetBool("Throwing", false);
        }
        else
        {
            if (save.currentData.playerType == "Bow" && bowAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                bow.SetActive(false);
                bowAnimator.SetBool("Throwing", false);
            }
        }
    }
}