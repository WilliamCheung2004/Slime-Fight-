using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class PlayerPrefsSearch : EditorWindow
{
    [MenuItem("Tools/Find PlayerPrefs Usage")]
    public static void SearchPlayerPrefs()
    {
        string[] scriptPaths = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
        List<string> matches = new List<string>();

        foreach (string path in scriptPaths)
        {
            string content = File.ReadAllText(path);

            if (content.Contains("PlayerPrefs"))
            {
                matches.Add(path);
            }
        }

        Debug.Log($"Found {matches.Count} scripts using PlayerPrefs:");

        foreach (string match in matches)
        {
            Debug.Log(match);
        }
    }
}
