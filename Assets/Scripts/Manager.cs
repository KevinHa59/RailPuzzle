using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public Sprite selectedImage;
    public string selectedType = "";
    public bool isEditMode = true;
    public bool setHint = false;
    public int stageNumber = 1;
    public GameObject spawnStageDataButton;
    public Toggle hintToggle;
    public Text stageLabel;

    private Button spawnStageDataButtonComponent;
    private cellManager grid;

    private void Awake()
    {
        grid = FindAnyObjectByType<cellManager>();

        if (grid != null)
        {
            grid.currentStage = stageNumber;
        }

        if (hintToggle != null)
        {
            hintToggle.onValueChanged.RemoveListener(SetHint);
            hintToggle.onValueChanged.AddListener(SetHint);
            SetHint(hintToggle.isOn);
        }

        if (spawnStageDataButton != null)
        {
            spawnStageDataButtonComponent = spawnStageDataButton.GetComponent<Button>();

            if (spawnStageDataButtonComponent != null)
            {
                spawnStageDataButtonComponent.onClick.RemoveListener(SpawnStageDataJson);
                spawnStageDataButtonComponent.onClick.AddListener(SpawnStageDataJson);
            }
        }
    }

    private void Start()
    {
        if (spawnStageDataButton != null)
        {
            spawnStageDataButton.SetActive(isEditMode);
        }
        if (hintToggle != null)
        {
            hintToggle.gameObject.SetActive(isEditMode);
        }
        if (stageLabel != null)
        {
            stageLabel.text = "Stage " + stageNumber;
        }

        LoadSelectedStage();
    }

    public void LoadSelectedStage()
    {
        if (grid == null)
        {
            grid = FindAnyObjectByType<cellManager>();
        }

        if (grid == null)
        {
            return;
        }

        grid.currentStage = stageNumber;

        if (isEditMode)
        {
            grid.LoadStageForEdit(stageNumber);
            return;
        }

        grid.LoadStage(stageNumber);
    }

    public void SetHint(bool value)
    {
        setHint = value;
    }

    public void SpawnStageDataJson()
    {
        var grid = FindAnyObjectByType<cellManager>();

        if (grid == null)
        {
            Debug.LogWarning("CellManager was not found.");
            return;
        }

        grid.currentStage = stageNumber;

        var missionEntries = new List<cellManager.MissionEntry>();

        foreach (Transform child in grid.transform)
        {
            var cellComponent = child.GetComponent<cell>();

            if (cellComponent == null || string.IsNullOrEmpty(cellComponent.placedType))
            {
                continue;
            }

            bool hintValue = grid.GetHintForCell(cellComponent.row + 1, cellComponent.column + 1);

            missionEntries.Add(new cellManager.MissionEntry
            {
                row = cellComponent.row + 1,
                column = cellComponent.column + 1,
                type = cellComponent.placedType,
                hint = hintValue
            });
        }

        var stageDataExport = new cellManager.StageData
        {
            stage = stageNumber,
            completed = false,
            completionTimeMission = new[]
            {
                new cellManager.CompletionTimeMissionEntry { time = 30, reward = 3 },
                new cellManager.CompletionTimeMissionEntry { time = 60, reward = 2 },
                new cellManager.CompletionTimeMissionEntry { time = 90, reward = 1 }
            },
            completeRailMission = missionEntries.ToArray()
        };

        string saveFilePath = Path.Combine(Application.persistentDataPath, "savedStageData.json");

        cellManager.StageData[] existingStages = null;
        if (File.Exists(saveFilePath))
        {
            string existingJson = File.ReadAllText(saveFilePath);

            if (!string.IsNullOrWhiteSpace(existingJson) && existingJson.Trim() != "{}")
            {
                var wrappedData = JsonUtility.FromJson<cellManager.StageDataWrapper>(existingJson);

                if (wrappedData != null && wrappedData.stages != null && wrappedData.stages.Length > 0)
                {
                    existingStages = wrappedData.stages;
                }
            }
        }

        var allStages = new List<cellManager.StageData>();

        if (existingStages != null)
        {
            allStages.AddRange(existingStages);
        }

        bool stageExists = false;
        for (int i = 0; i < allStages.Count; i++)
        {
            if (allStages[i].stage == stageDataExport.stage)
            {
                stageExists = true;
                allStages[i] = new cellManager.StageData
                {
                    stage = stageDataExport.stage,
                    completed = allStages[i].completed,
                    completionTimeMission = stageDataExport.completionTimeMission,
                    completeRailMission = stageDataExport.completeRailMission
                };
                break;
            }
        }

        if (!stageExists)
        {
            allStages.Add(new cellManager.StageData
            {
                stage = stageDataExport.stage,
                completed = false,
                completionTimeMission = stageDataExport.completionTimeMission,
                completeRailMission = stageDataExport.completeRailMission
            });
        }

        var saveData = new cellManager.StageDataWrapper
        {
            stages = allStages.ToArray()
        };

        string jsonText = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveFilePath, jsonText);

        Debug.Log("Saved stage data to: " + saveFilePath);
        Debug.Log(jsonText);
    }
}
