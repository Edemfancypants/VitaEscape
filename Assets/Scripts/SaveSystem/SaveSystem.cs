﻿using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem instance;

    private void Awake()
    {
        SetupInstance();
    }

    public void SetupInstance()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            gameObject.name = "SaveSystem";
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public enum BuildTargetEnum
    {
        Vita,
        PC,
    }

    [Header("Save data reference")]
    public SaveData saveData;

    [Header("DataPath settings")]
    public string saveName = "testSave";
    public string saveExt = ".save";

    [Header("Build Settings")]
    public BuildTargetEnum buildTarget;

    [HideInInspector]
    public bool dataLoaded;

    private void Start()
    {
        if (buildTarget == BuildTargetEnum.Vita)
        {
            string dataPath = "ux0:data/LabRats";

            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
        }
    }

    private string GetSavePath()
    {
        switch (buildTarget)
        {
            case BuildTargetEnum.Vita:
                return "ux0:data/LabRats/" + saveName + saveExt;
            case BuildTargetEnum.PC:
                return Application.persistentDataPath + "/" + saveName + saveExt; ;
            default:
                return string.Empty;
        }
    }

    public void Save()
    {
        Debug.Log("<b>[SaveSystem]</b> Saving data");

        string dataPath = GetSavePath();

        var serializer = new XmlSerializer(typeof(SaveData));
        var stream = new FileStream(dataPath, FileMode.Create);

        serializer.Serialize(stream, saveData);
        stream.Close();
    }

    public void Load(Action onDataLoaded)
    {
        if (dataLoaded == false)
        {
            string dataPath = GetSavePath();

            if (File.Exists(dataPath))
            {
                Debug.Log("<b>[SaveSystem]</b> Loading data");

                var serializer = new XmlSerializer(typeof(SaveData));
                using (var stream = new FileStream(dataPath, FileMode.Open))
                {
                    saveData = serializer.Deserialize(stream) as SaveData;
                }

                dataLoaded = true;

                onDataLoaded?.Invoke();
            }
            else
            {
                Debug.LogWarning("<b>[SaveSystem]</b> Couldn't find data to load, setting default values!");

                ResetSaveData();
                onDataLoaded?.Invoke();
            }
        }
        else
        {
            Debug.Log("<b>[SaveSystem]</b> Data already loaded, using local values");

            onDataLoaded?.Invoke();
        }
    }

    public void ClearSave()
    {
        Debug.Log("<b>[SaveSystem]</b> Deleted save file");
        string dataPath = GetSavePath();

        File.Delete(dataPath);

        ResetSaveData();
    }

    public void ResetSaveData()
    {
        saveData.collectibles.Clear();
        saveData.unlockedLevels.Clear();

        saveData.masterFloat = 1f;
        saveData.bgmFloat = 1f;
        saveData.sfxFloat = 1f;

        saveData.unlockedLevels.Add("Factory_1");

        Save();
        dataLoaded = false;

        Load(() =>
        {
            Debug.Log("<b>[SaveSystem]</b> SaveData successfully reset!");
        });
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SaveSystem))]
public class SaveSystemEditorTest : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SaveSystem saveSystem = (SaveSystem)target;

        GUILayout.Space(10);
        GUILayout.Label("Debug Settings", EditorStyles.boldLabel);
        GUILayout.Space(5);

        DrawButton("Save", saveSystem.Save);
        DrawButton("Load", () => { saveSystem.Load(() => { }); });
        DrawButton("Delete Save", () => { saveSystem.ClearSave(); saveSystem.Load(() => { }); });
        DrawButton("Open Save Location", () => { System.Diagnostics.Process.Start(Application.persistentDataPath); });
        DrawButton("Debug Save", () => { DebugSave(saveSystem); });
    }

    private void DrawButton(string label, Action action)
    {
        GUILayout.Label(label, EditorStyles.boldLabel);
        if (GUILayout.Button(label))
        {
            action();
        }
    }

    private void DebugSave(SaveSystem _saveSystem)
    {
        _saveSystem.saveData.collectibles.Clear();
        _saveSystem.saveData.unlockedLevels.Clear();

        CollectibleType collectible_calculator = new CollectibleType();
        collectible_calculator.id = 0;
        collectible_calculator.name = "Calculator";

        CollectibleType collectible_medpack = new CollectibleType();
        collectible_medpack.id = 1;
        collectible_medpack.name = "Medpack";

        CollectibleType collectible_FireExtinguisher = new CollectibleType();
        collectible_FireExtinguisher.id = 2;
        collectible_FireExtinguisher.name = "FireExtinguisher";

        CollectibleType collectible_Cup = new CollectibleType();
        collectible_Cup.id = 3;
        collectible_Cup.name = "Cup";

        CollectibleType collectible_Mug = new CollectibleType();
        collectible_Mug.id = 4;
        collectible_Mug.name = "Mug";

        CollectibleType collectible_Tablet = new CollectibleType();
        collectible_Tablet.id = 5;
        collectible_Tablet.name = "Tablet";

        CollectibleType collectible_Trash = new CollectibleType();
        collectible_Trash.id = 6;
        collectible_Trash.name = "Trash";

        CollectibleType collectible_PC = new CollectibleType();
        collectible_PC.id = 7;
        collectible_PC.name = "PC";

        CollectibleType collectible_Speaker = new CollectibleType();
        collectible_Speaker.id = 8;
        collectible_Speaker.name = "Speaker";

        CollectibleType collectible_Keyboard = new CollectibleType();
        collectible_Keyboard.id = 9;
        collectible_Keyboard.name = "Keyboard";

        _saveSystem.saveData.collectibles.Add(collectible_calculator);
        _saveSystem.saveData.collectibles.Add(collectible_medpack);
        _saveSystem.saveData.collectibles.Add(collectible_FireExtinguisher);
        _saveSystem.saveData.collectibles.Add(collectible_Cup);
        _saveSystem.saveData.collectibles.Add(collectible_Mug);
        _saveSystem.saveData.collectibles.Add(collectible_Tablet);
        _saveSystem.saveData.collectibles.Add(collectible_Trash);
        _saveSystem.saveData.collectibles.Add(collectible_PC);
        _saveSystem.saveData.collectibles.Add(collectible_Speaker);
        _saveSystem.saveData.collectibles.Add(collectible_Keyboard);

        _saveSystem.saveData.unlockedLevels.Add("Factory_1");
        _saveSystem.saveData.unlockedLevels.Add("Factory_2");
        _saveSystem.saveData.unlockedLevels.Add("Lab_1");
        _saveSystem.saveData.unlockedLevels.Add("Lab_2");

        _saveSystem.Save();
    }
}
#endif