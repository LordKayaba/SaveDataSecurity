using System.IO;
using UnityEngine;
using UnityEditor;

public class SaveSettings : EditorWindow
{

    [MenuItem("Window/Save Data")]
    public static void ShowWindow()
    {
        SaveSettings Window = GetWindow<SaveSettings>("Save Data");
        Window.minSize = new Vector2(400, 100);
        Window.maxSize = new Vector2(400, 100);
    }

    void OnGUI()
    {
        GUILayout.Space(10);

        GUILayout.Label("Application.persistentDataPath", EditorStyles.boldLabel);
        if (GUILayout.Button("Open save folder"))
        {
            Application.OpenURL(Application.persistentDataPath);
        }

        GUILayout.Space(10);

        GUILayout.Label("Clear all saved data and keys (this includes PlayerPrefs)", EditorStyles.boldLabel);
        if (GUILayout.Button("Clear all"))
        {
            PlayerPrefs.DeleteAll();
            DeleteFilesInDirectory(Application.persistentDataPath);
            EditorUtility.DisplayDialog("Clear data", "The save data and keys have been deleted successfully", "OK");
        }
    }

    void DeleteFilesInDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            try
            {
                string[] files = Directory.GetFiles(path);

                foreach (string file in files)
                {
                    File.Delete(file);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error deleting files: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("Directory not found: " + path);
        }
    }

}