using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;

public class FindScriptReferences
{
    [MenuItem("Tools/Find Initialise Sound References")]
    public static void FindReferences()
    {
        string targetScriptName = "Initialise Sound";

        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");

        Debug.Log("=== SEARCHING SCENES ===");

        foreach (string guid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);

            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            GameObject[] rootObjects = scene.GetRootGameObjects();

            foreach (GameObject root in rootObjects)
            {
                MonoBehaviour[] scripts = root.GetComponentsInChildren<MonoBehaviour>(true);

                foreach (MonoBehaviour script in scripts)
                {
                    if (script == null)
                        continue;

                    if (script.GetType().Name == targetScriptName)
                    {
                        Debug.Log(
                            $"FOUND IN SCENE: {scenePath}\nGameObject: {script.gameObject.name}",
                            script.gameObject
                        );
                    }
                }
            }
        }

        Debug.Log("=== SEARCHING PREFABS ===");

        foreach (string guid in prefabGuids)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(guid);

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            MonoBehaviour[] scripts = prefab.GetComponentsInChildren<MonoBehaviour>(true);

            foreach (MonoBehaviour script in scripts)
            {
                if (script == null)
                    continue;

                if (script.GetType().Name == targetScriptName)
                {
                    Debug.Log(
                        $"FOUND IN PREFAB: {prefabPath}\nGameObject: {script.gameObject.name}",
                        script.gameObject
                    );
                }
            }
        }

        Debug.Log("=== SEARCH COMPLETE ===");
    }
}