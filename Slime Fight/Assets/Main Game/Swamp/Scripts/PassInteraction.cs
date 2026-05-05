using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxInteractor : MonoBehaviour
{
    [SerializeField] private Spawner spawner;
    [SerializeField] private Dialogue dialogue;
    private bool finished;

    private void Update()
    {
        bool finished = dialogue.textFinished;

        if (finished)
        {
            spawner.interacted = true;
            if (spawner.interacted)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
