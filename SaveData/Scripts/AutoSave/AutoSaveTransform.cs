using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSaveTransform : MonoBehaviour
{
    [Tooltip("obj save key")]
    public string Key;
    [Range(5, 60)][Tooltip("Time cycle for save")]
    public int Timer = 15;
    public TransformSettings AdditionalSettings;
    [HideInInspector]
    public string OldKey;
    [HideInInspector]
    public bool Add, delete;

    List<TransformData> transformDatas = new List<TransformData>();

    void OnValidate()
    {
        if (!Application.isPlaying)
        {
            if (!Add)
            {
                Add = true;
                gameObject.name = "AutoSaveTransform";
                Key = "AutoSaveTransform";
                OldKey = Key;
            }
            if (OldKey.Length >= 2 && OldKey.Length <= 32 && delete == false)
            {
                if(SD.Exists(OldKey))
                {
                    delete = true;
                }
                else
                {
                    delete = false;
                }
            }
        }
    }

    void Awake()
    {
        if (SD.Exists(OldKey))
        {
            Load();
        }
        else
        {
            Save();
        }
        if (AdditionalSettings.AutoSave)
        {
            StartCoroutine(timer());
        }
    }

    IEnumerator timer()
    {
        yield return new WaitForSeconds(Timer);
        Save();
        if (AdditionalSettings.AutoSave)
        {
            StartCoroutine(timer());
        }
    }

    public void Save()
    {
        if(AdditionalSettings.DynamicGameObjects.Count > 0)
        {
            transformDatas = new List<TransformData>();

            for (int i = 0; i < AdditionalSettings.DynamicGameObjects.Count; i++)
            {
                TransformData transformData = new TransformData();
                transformData.position = AdditionalSettings.DynamicGameObjects[i].transform.position;
                transformData.rotation = AdditionalSettings.DynamicGameObjects[i].transform.rotation.eulerAngles;
                transformData.localScale = AdditionalSettings.DynamicGameObjects[i].transform.localScale;

                transformDatas.Add(transformData);
            }
            SD.Save(OldKey, transformDatas);
        }
    }

    public void Load()
    {
        transformDatas = SD.Load<List<TransformData>>(OldKey);
        if(transformDatas.Count == AdditionalSettings.DynamicGameObjects.Count)
        {
            for (int i = 0; i < transformDatas.Count; i++)
            {
                AdditionalSettings.DynamicGameObjects[i].transform.position = transformDatas[i].position;
                AdditionalSettings.DynamicGameObjects[i].transform.rotation = Quaternion.Euler(transformDatas[i].rotation);
                AdditionalSettings.DynamicGameObjects[i].transform.localScale = transformDatas[i].localScale;
            }
        }
    }

    void GetDynamicGameObjects()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        AdditionalSettings.DynamicGameObjects = new List<GameObject>();
        for (int i = 0; i < allObjects.Length; i++)
        {
            if(allObjects[i].isStatic == false)
            {
                bool NotTag = false;
                for (int a = 0; a < AdditionalSettings.DontSaveTags.Length; a++)
                {
                    if(allObjects[i].CompareTag(AdditionalSettings.DontSaveTags[a]) || allObjects[i] == gameObject)
                    {
                        NotTag = true;
                        break;
                    }
                }
                if(!NotTag)
                {
                    AdditionalSettings.DynamicGameObjects.Add(allObjects[i]);
                }
            }
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause && AdditionalSettings.StopOrExit)
        {
            Save();
        }
    }

    void OnApplicationQuit()
    {
        if (AdditionalSettings.StopOrExit)
        {
            Save();
        }
    }

    public void Edit()
    {
        if(OldKey != Key)
        {
            if(Key.Length >= 2 && Key.Length <= 32)
            {
                if (SD.Exists(OldKey))
                {
                    SD.EditKey(OldKey, Key);
                }
                OldKey = Key;
            }
            else
            {
                Debug.LogError("The key must not be smaller than 2 and larger than 32 characters");
            }
        }
        GetDynamicGameObjects();
    }

    public void Delete()
    {
        SD.Delete(OldKey);
        delete = false;
    }
}
