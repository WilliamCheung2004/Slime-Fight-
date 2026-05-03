using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using System.Net;


[System.Serializable]
public class DialogueLine
{
    public string speaker;
    public string text;
    public string animation;
}

[System.Serializable]
public class DialogueData
{
    public DialogueLine[] Dialogue;
}

public class Dialogue : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private TextMeshProUGUI characterComponent;
    public bool textFinished = false;
    public string currentSpeaker;

    [Header("Settings")]
    [SerializeField] private float textSpeed = 0.03f;

    [Header("Save")]
    [SerializeField] private SaveManager save;

    private DialogueLine[] lines;
    private int index;
    [SerializeField] private string jsonFileName;

    public bool scriptDone = false;
    public string currentScene;
    public string currentAnimation;

    void Start()
    {
        PlayerPrefs.SetInt("DialogueStatus", 1); 

        if (jsonFileName == "")
        {
            Debug.LogError("JSON file name is not set in the inspector.");
            return;
        }

        LoadConversation(jsonFileName);
        StartDialogue();

        currentScene = SceneManager.GetActiveScene().name;

    }

    //private void OnEnable()
    //{
    //    LoadConversation(jsonFileName);
    //    StartDialogue();
    //}

    public void LoadConversation(string jsonFileName)
    {
        save.GetComponent<SaveManager>().CheckSave();
        if (jsonFileName == "Town")
        {
            if (save.currentData.playerType == "Sword")
            {
                jsonFileName = "Town_Sword";
            }
            else if (save.currentData.playerType == "Bow")

                {
                    jsonFileName = "Town_Bow";
            }
            else
            {
                Debug.LogWarning("Error no weapon type found defaulting to Town dialogue.");
            }
        }

        string path = "Dialogue/Script/" + jsonFileName;
        TextAsset jsonText = Resources.Load<TextAsset>(path);
        if (jsonText != null)
        {
            DialogueData data = JsonUtility.FromJson<DialogueData>(jsonText.text);
            lines = data.Dialogue;
            Debug.Log("Loaded JSON: " + path);
        }
        else
        {
            Debug.LogError("Dialogue JSON not found at path: " + path);
            lines = new DialogueLine[0];
        }
    }

    public void StartDialogue()
    {
        PlayerPrefs.SetInt("DialogueStatus", 0);
        index = 0;
        StartCoroutine(TypeLine());
    }

    private void Update()
    {
        if (lines == null || lines.Length == 0) return;

        if (textComponent.text == lines[index].text)
        {
            currentSpeaker = "None";
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[index].text)
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index].text;
                scriptDone = true;
            }
        }
    }

    private IEnumerator TypeLine()
    {
        textComponent.text = string.Empty;
        characterComponent.text = "(" + lines[index].speaker + ")";
        currentSpeaker = lines[index].speaker;

        if (!string.IsNullOrEmpty(lines[index].animation))
        {
            Debug.Log("Setting current animation to: " + lines[index].animation);
            currentAnimation = lines[index].animation;
        }

        foreach (char c in lines[index].text.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    private void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            StartCoroutine(TypeLine());
            textFinished = false;
            PlayerPrefs.SetInt("DialogueStatus", textFinished ? 1 : 0);
        }
        else
        {
            textFinished = true;
            PlayerPrefs.SetInt("DialogueStatus", textFinished ? 1 : 0);
            gameObject.SetActive(false);
        }
    }

    public void EndDialogue()
    {
        textComponent.text = "";
        characterComponent.text = "";
        currentSpeaker = "None";
        gameObject.SetActive(false);
    }
}