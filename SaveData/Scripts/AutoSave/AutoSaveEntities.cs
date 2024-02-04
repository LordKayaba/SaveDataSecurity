using System.Collections;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEditor;

[Serializable]
public class TransformSettings
{
    [Tooltip("Tags that should not be saved")]
    public string[] DontSaveTags = { "Player", "MainCamera" };
    [Tooltip("Dynamic gameobjects (save list)")]
    public List<GameObject> DynamicGameObjects;
    [Tooltip("If it is true, it will be saved automatically")]
    public bool AutoSave = true;
    [Tooltip("If it is true, it will be saved when stopping the game or when leaving the game")]
    public bool StopOrExit = true;
}

public class TransformData
{
    [JsonProperty("a")]
    public Vector3 position;
    [JsonProperty("b")]
    public Vector3 rotation;
    [JsonProperty("c")]
    public Vector3 localScale;
}

[CustomEditor(typeof(AutoSaveTransform))]
public class TransformEditor : Editor
{
    AutoSaveTransform AutoSaveTransform;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AutoSaveTransform = (AutoSaveTransform)target;

        if (GUILayout.Button("Update"))
        {
            AutoSaveTransform.Edit();
            if(AutoSaveTransform.AdditionalSettings.DynamicGameObjects.Count == 0)
            {
                Debug.LogError("No dynamic game objects were found. Please make the desired game objects dynamic");
                EditorUtility.DisplayDialog("Get Dynamic Game Objects", "No dynamic game objects were found. Please make the desired game objects dynamic", "OK");
            }
        }
        if(AutoSaveTransform.Key != AutoSaveTransform.OldKey)
        {
            if(AutoSaveTransform.Key.Length >= 2 && AutoSaveTransform.Key.Length <= 32)
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.normal.textColor = Color.yellow;
                GUILayout.Label("Please click the update button", style);
            }
            else
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.normal.textColor = Color.red;
                GUILayout.Label("The key must not be smaller than 2 and larger than 32 characters", style);
            }
        }
        if (AutoSaveTransform.delete)
        {
            if(GUILayout.Button("Delete Save"))
            {
                AutoSaveTransform.Delete();
            }
        }
    }
}
