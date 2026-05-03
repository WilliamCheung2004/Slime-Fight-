using UnityEngine;

public class TriggerBoss : MonoBehaviour
{
    [SerializeField] private Spawner spawner;
    [SerializeField] private GameObject Dialogue;
    [SerializeField] private GameObject BossSpawn;

    private Dialogue dialogueScript;
    private bool initialisedDialogue = false;

    private void Start()
    {
        dialogueScript = Dialogue.GetComponent<Dialogue>();
    }

    void Update()
    {
        CanSpawnBoss();
    }

    private void CanSpawnBoss()
    {
        if (spawner.StartBoss)
        {
            if (!initialisedDialogue)
            {
                Dialogue.SetActive(true);
                initialisedDialogue = true;
            }
            if (dialogueScript.textFinished)
            {
                BossSpawn.SetActive(true);
            }
        }
    }
    
}
