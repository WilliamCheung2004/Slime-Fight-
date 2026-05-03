using UnityEngine;

public class StopAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void StopWeaponAnimation()
    {
        animator.SetBool("Throwing", false);
    }
}
