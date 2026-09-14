using System.IO;
using UnityEngine;

public class stageManager : MonoBehaviour
{
    public GameObject stageButtonPrefab;

    private void Start()
    {
        SpawnStageButtons();
    }

    public void SpawnStageButtons()
    {
        if (stageButtonPrefab == null)
        {
            Debug.LogWarning("Stage button prefab is not assigned.");
            return;
        }

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        string saveFilePath = Path.Combine(Application.persistentDataPath, "savedStageData.json");

        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("Saved stage data file was not found at: " + saveFilePath);
            return;
        }

        string json = File.ReadAllText(saveFilePath);

        if (string.IsNullOrWhiteSpace(json) || json.Trim() == "{}")
        {
            return;
        }

        cellManager.StageData[] stages = null;

        var wrappedData = JsonUtility.FromJson<cellManager.StageDataWrapper>(json);

        if (wrappedData != null && wrappedData.stages != null && wrappedData.stages.Length > 0)
        {
            stages = wrappedData.stages;
        }

        if (stages == null || stages.Length == 0)
        {
            try
            {
                stages = JsonUtility.FromJson<cellManager.StageData[]>(json);
            }
            catch
            {
                stages = null;
            }
        }

        if (stages == null || stages.Length == 0)
        {
            Debug.LogWarning("Saved stage data could not be parsed.");
            return;
        }

        foreach (var stage in stages)
        {
            if (stage == null)
            {
                continue;
            }

            GameObject stageButtonObject = Instantiate(stageButtonPrefab, transform);
            stageButtonObject.name = "Stage" + stage.stage;

            var stageButtonComponent = stageButtonObject.GetComponent<stageButton>();
            if (stageButtonComponent != null)
            {
                stageButtonComponent.Setup(stage.stage);
            }
        }
    }
}
