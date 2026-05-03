using UnityEngine;

public class EventStart : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject Dialogue;
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return; 

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger!");
            Dialogue.SetActive(true);
            triggered = true;
        }
    }
}

