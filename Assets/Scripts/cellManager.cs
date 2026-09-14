using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class cellManager : MonoBehaviour
{
    [System.Serializable]
    public class PlacedRail
    {
        public int row;
        public int column;
        public string type;
    }

    [System.Serializable]
    public class PlacedRailList
    {
        public PlacedRail[] placedRails;
    }

    [System.Serializable]
    public class MissionEntry
    {
        public int row;
        public int column;
        public string type;
        public bool hint;
    }

    [System.Serializable]
    public class CompletionTimeMissionEntry
    {
        public int time;
        public int reward;
    }

    [System.Serializable]
    public class StageData
    {
        public int stage;
        public bool completed;
        public CompletionTimeMissionEntry[] completionTimeMission;
        public MissionEntry[] completeRailMission;
    }

    [System.Serializable]
    public class StageDataWrapper
    {
        public StageData[] stages;
    }

    public GameObject cellPrefab;
    public int rows = 8;
    public int columns = 8;
    public int currentStage = 1;

    public int[] rowCounts;
    public int[] columnCounts;

    private StageData[] stageDataList;
    private StageData currentStageData;
    private Manager manager;
    private bool missionCompletedLogged;

    private void Start()
    {
        manager = FindAnyObjectByType<Manager>();
        GenerateCells();

        if (manager == null || !manager.isEditMode)
        {
            LoadStage(currentStage);
        }

        RefreshHeaderCounts();
    }

    public void GenerateCells()
    {
        if (cellPrefab == null)
        {
            Debug.LogWarning("cellPrefab is not assigned.");
            return;
        }

        rowCounts = new int[rows];
        columnCounts = new int[columns];

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                GameObject cellObj = Instantiate(cellPrefab, transform);
                cellObj.name = "Row" + row + "Column" + column;

                cell cellComponent = cellObj.GetComponent<cell>();

                if (cellComponent != null)
                {
                    cellComponent.row = row;
                    cellComponent.column = column;
                    cellComponent.isLocked = false;
                    cellComponent.placedType = "";
                }
            }
        }
    }

    public void LoadStage(int stageNumber)
    {
        if (stageDataList == null)
        {
            stageDataList = LoadStageDataFromFile();
        }

        if (stageDataList == null)
        {
            Debug.LogWarning("Stage data file could not be loaded.");
            return;
        }

        StageData stage = null;

        foreach (var item in stageDataList)
        {
            if (item != null && item.stage == stageNumber)
            {
                stage = item;
                break;
            }
        }

        if (stage == null)
        {
            Debug.LogWarning("Stage " + stageNumber + " was not found in stage data.");
            return;
        }

        currentStageData = stage;
        missionCompletedLogged = false;

        for (int i = 0; i < rowCounts.Length; i++)
        {
            rowCounts[i] = 0;
        }

        for (int i = 0; i < columnCounts.Length; i++)
        {
            columnCounts[i] = 0;
        }

        foreach (Transform child in transform)
        {
            var cellComponent = child.GetComponent<cell>();

            if (cellComponent == null)
            {
                continue;
            }

            cellComponent.isLocked = false;
            cellComponent.placedType = "";

            if (cellComponent.icon != null)
            {
                cellComponent.icon.sprite = null;
                cellComponent.icon.gameObject.SetActive(false);
            }
        }

        if (stage.completeRailMission == null)
        {
            RefreshHeaderCounts();
            return;
        }

        foreach (var entry in stage.completeRailMission)
        {
            if (entry == null)
            {
                continue;
            }

            if (!TryGetStageCellIndex(entry, out int rowIndex, out int columnIndex))
            {
                continue;
            }

            if (rowIndex >= 0 && rowIndex < rowCounts.Length)
            {
                rowCounts[rowIndex]++;
            }

            if (columnIndex >= 0 && columnIndex < columnCounts.Length)
            {
                columnCounts[columnIndex]++;
            }

            var targetCell = GetCell(rowIndex, columnIndex);
            if (targetCell == null || targetCell.icon == null)
            {
                continue;
            }

            if (entry.hint)
            {
                Sprite sprite = GetRailSprite(entry.type);
                targetCell.icon.sprite = sprite;
                targetCell.icon.gameObject.SetActive(true);
                targetCell.isLocked = true;
                targetCell.placedType = entry.type;
            }
        }

        RefreshHeaderCounts();
    }

    public void LoadStageForEdit(int stageNumber)
    {
        if (stageDataList == null)
        {
            stageDataList = LoadStageDataFromFile();
        }

        if (stageDataList == null)
        {
            Debug.LogWarning("Stage data file could not be loaded.");
            return;
        }

        StageData stage = null;

        foreach (var item in stageDataList)
        {
            if (item != null && item.stage == stageNumber)
            {
                stage = item;
                break;
            }
        }

        if (stage == null)
        {
            Debug.LogWarning("Stage " + stageNumber + " was not found in stage data.");
            return;
        }

        currentStageData = stage;
        missionCompletedLogged = false;

        for (int i = 0; i < rowCounts.Length; i++)
        {
            rowCounts[i] = 0;
        }

        for (int i = 0; i < columnCounts.Length; i++)
        {
            columnCounts[i] = 0;
        }

        foreach (Transform child in transform)
        {
            var cellComponent = child.GetComponent<cell>();

            if (cellComponent == null)
            {
                continue;
            }

            cellComponent.isLocked = false;
            cellComponent.placedType = "";

            if (cellComponent.icon != null)
            {
                cellComponent.icon.sprite = null;
                cellComponent.icon.gameObject.SetActive(false);
            }
        }

        if (stage.completeRailMission == null)
        {
            RefreshHeaderCounts();
            return;
        }

        foreach (var entry in stage.completeRailMission)
        {
            if (entry == null)
            {
                continue;
            }

            if (!TryGetStageCellIndex(entry, out int rowIndex, out int columnIndex))
            {
                continue;
            }

            if (rowIndex >= 0 && rowIndex < rowCounts.Length)
            {
                rowCounts[rowIndex]++;
            }

            if (columnIndex >= 0 && columnIndex < columnCounts.Length)
            {
                columnCounts[columnIndex]++;
            }

            var targetCell = GetCell(rowIndex, columnIndex);
            if (targetCell == null || targetCell.icon == null)
            {
                continue;
            }

            Sprite sprite = GetRailSprite(entry.type);
            targetCell.icon.sprite = sprite;
            targetCell.icon.gameObject.SetActive(true);
            targetCell.isLocked = entry.hint;
            targetCell.placedType = entry.type;
        }

        RefreshHeaderCounts();
    }

    public cell GetCell(int row, int column)
    {
        foreach (Transform child in transform)
        {
            var cellComponent = child.GetComponent<cell>();
            if (cellComponent != null && cellComponent.row == row && cellComponent.column == column)
            {
                return cellComponent;
            }
        }

        return null;
    }

    private Sprite GetRailSprite(string type)
    {
        var railTypes = FindObjectsByType<railType>(FindObjectsInactive.Include);

        foreach (var rail in railTypes)
        {
            if (rail != null && rail.type == type)
            {
                return rail.railTypeIcon != null ? rail.railTypeIcon.sprite : null;
            }
        }

        return null;
    }

    private StageData[] LoadStageDataFromFile()
    {
        string saveFilePath = Path.Combine(Application.persistentDataPath, "savedStageData.json");
        StageData[] savedStages = LoadStageDataFromJsonFile(saveFilePath);

        if (savedStages != null && savedStages.Length > 0)
        {
            return savedStages;
        }

        string filePath = Application.dataPath + "/Scripts/data.json";
        StageData[] defaultStages = LoadStageDataFromJsonFile(filePath);

        if (defaultStages == null || defaultStages.Length == 0)
        {
            Debug.LogWarning("Stage data file not found or could not be parsed: " + filePath);
            return null;
        }

        return defaultStages;
    }

    private StageData[] LoadStageDataFromJsonFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return null;
        }

        string json = File.ReadAllText(filePath);
        return LoadStageDataFromJsonText(json);
    }

    private StageData[] LoadStageDataFromJsonText(string json)
    {
        if (string.IsNullOrWhiteSpace(json) || json.Trim() == "{}")
        {
            return null;
        }

        var wrappedData = JsonUtility.FromJson<StageDataWrapper>(json);

        if (wrappedData != null && wrappedData.stages != null && wrappedData.stages.Length > 0)
        {
            return wrappedData.stages;
        }

        StageData[] directArray = null;

        try
        {
            directArray = JsonUtility.FromJson<StageData[]>(json);
        }
        catch
        {
            directArray = null;
        }

        if (directArray != null && directArray.Length > 0)
        {
            return directArray;
        }

        var singleStage = JsonUtility.FromJson<StageData>(json);

        if (singleStage != null)
        {
            return new[] { singleStage };
        }

        return null;
    }

    public bool GetHintForCell(int row, int column)
    {
        if (currentStageData == null || currentStageData.completeRailMission == null)
        {
            return false;
        }

        for (int i = 0; i < currentStageData.completeRailMission.Length; i++)
        {
            var entry = currentStageData.completeRailMission[i];

            if (entry != null && entry.row == row && entry.column == column)
            {
                return entry.hint;
            }
        }

        return false;
    }

    public void ToggleHint(cell targetCell)
    {
        if (targetCell == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(targetCell.placedType) || targetCell.placedType == "xMark")
        {
            return;
        }

        if (currentStageData == null)
        {
            Debug.LogWarning("No stage is currently loaded, so hint state cannot be toggled.");
            return;
        }

        if (currentStageData.completeRailMission == null)
        {
            currentStageData.completeRailMission = new MissionEntry[0];
        }

        bool foundEntry = false;
        for (int i = 0; i < currentStageData.completeRailMission.Length; i++)
        {
            var entry = currentStageData.completeRailMission[i];

            if (entry == null)
            {
                continue;
            }

            if (entry.row == targetCell.row + 1 && entry.column == targetCell.column + 1)
            {
                entry.hint = !entry.hint;
                targetCell.isLocked = entry.hint;
                foundEntry = true;
                break;
            }
        }

        if (!foundEntry)
        {
            var list = new List<MissionEntry>(currentStageData.completeRailMission)
            {
                new MissionEntry
                {
                    row = targetCell.row + 1,
                    column = targetCell.column + 1,
                    type = targetCell.placedType,
                    hint = true
                }
            };

            currentStageData.completeRailMission = list.ToArray();
            targetCell.isLocked = true;
        }

    }

    public void ApplyCellPlacement(cell targetCell, string selectedType, Sprite selectedImage)
    {
        if (targetCell == null)
        {
            return;
        }

        if (targetCell.isLocked)
        {
            return;
        }

        bool hasRail = targetCell.icon != null && targetCell.icon.gameObject.activeSelf && targetCell.icon.sprite != null;
        bool shouldUpdateCounts = manager == null || manager.isEditMode;

        if (selectedType == "remove")
        {
            if (hasRail)
            {
                if (shouldUpdateCounts)
                {
                    if (targetCell.row >= 0 && targetCell.row < rowCounts.Length)
                    {
                        rowCounts[targetCell.row] = Mathf.Max(0, rowCounts[targetCell.row] - 1);
                    }

                    if (targetCell.column >= 0 && targetCell.column < columnCounts.Length)
                    {
                        columnCounts[targetCell.column] = Mathf.Max(0, columnCounts[targetCell.column] - 1);
                    }
                }

                targetCell.icon.sprite = null;
                targetCell.icon.gameObject.SetActive(false);
                targetCell.placedType = "";
            }
        }
        else if (selectedType == "xMark")
        {
            if (!hasRail && selectedImage != null && targetCell.icon != null)
            {
                targetCell.icon.sprite = selectedImage;
                targetCell.icon.gameObject.SetActive(true);
                targetCell.placedType = selectedType;
            }
        }
        else if (selectedType != "")
        {
            if (!hasRail && shouldUpdateCounts)
            {
                if (targetCell.row >= 0 && targetCell.row < rowCounts.Length)
                {
                    rowCounts[targetCell.row]++;
                }

                if (targetCell.column >= 0 && targetCell.column < columnCounts.Length)
                {
                    columnCounts[targetCell.column]++;
                }
            }

            if (selectedImage != null && targetCell.icon != null)
            {
                targetCell.icon.sprite = selectedImage;
                targetCell.icon.gameObject.SetActive(true);
                targetCell.placedType = selectedType;
            }
        }

        if (shouldUpdateCounts)
        {
            RefreshHeaderCounts();
        }

        if (manager != null && !manager.isEditMode)
        {
            CheckMissionCompletion();
        }

        LogPlacedRails();
    }

    private void CheckMissionCompletion()
    {
        if (currentStageData == null || currentStageData.completeRailMission == null)
        {
            return;
        }

        bool isMissionComplete = true;

        foreach (var entry in currentStageData.completeRailMission)
        {
            if (entry == null)
            {
                continue;
            }

            if (!TryGetStageCellIndex(entry, out int rowIndex, out int columnIndex))
            {
                isMissionComplete = false;
                break;
            }

            var targetCell = GetCell(rowIndex, columnIndex);

            if (targetCell == null)
            {
                Debug.LogWarning($"Mission check failed: could not find cell at row {entry.row}, column {entry.column}.");
                isMissionComplete = false;
                break;
            }

            if (string.IsNullOrEmpty(targetCell.placedType) || targetCell.placedType != entry.type)
            {
                Debug.LogWarning($"Mission check failed: expected cell ({entry.row}, {entry.column}) to contain '{entry.type}', but found '{(string.IsNullOrEmpty(targetCell.placedType) ? "empty" : targetCell.placedType)}'.");
                isMissionComplete = false;
                break;
            }
        }

        if (isMissionComplete && !missionCompletedLogged)
        {
            Debug.Log("Mission Completed");
            missionCompletedLogged = true;
        }
        else if (!isMissionComplete)
        {
            missionCompletedLogged = false;
        }
    }

    private bool TryGetStageCellIndex(MissionEntry entry, out int rowIndex, out int columnIndex)
    {
        rowIndex = -1;
        columnIndex = -1;

        if (entry == null || entry.row <= 0 || entry.column <= 0)
        {
            return false;
        }

        rowIndex = entry.row - 1;
        columnIndex = entry.column - 1;

        return rowIndex >= 0 && rowIndex < rows && columnIndex >= 0 && columnIndex < columns;
    }

    public void RefreshHeaderCounts()
    {
        var rowHeaders = FindObjectsByType<rowHeader>(FindObjectsInactive.Include);
        foreach (var header in rowHeaders)
        {
            header.UpdateRowCounts();
        }

        var columnHeaders = FindObjectsByType<columnHeader>(FindObjectsInactive.Include);
        foreach (var header in columnHeaders)
        {
            header.UpdateColumnCounts();
        }
    }

    public void LogPlacedRails()
    {
        var placedRails = new List<PlacedRail>();

        foreach (Transform child in transform)
        {
            var cellComponent = child.GetComponent<cell>();
            if (cellComponent == null || string.IsNullOrEmpty(cellComponent.placedType))
            {
                continue;
            }

            placedRails.Add(new PlacedRail
            {
                row = cellComponent.row + 1,
                column = cellComponent.column + 1,
                type = cellComponent.placedType
            });
        }

        var logData = new PlacedRailList
        {
            placedRails = placedRails.ToArray()
        };

        // Debug.Log(JsonUtility.ToJson(logData, true));
    }
}
