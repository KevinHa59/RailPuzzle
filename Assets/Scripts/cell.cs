using UnityEngine;
using UnityEngine.UI;

public class cell : MonoBehaviour
{
    public Image icon;
    public int row;
    public int column;
    public string placedType = "";
    public bool isLocked;

    private Manager manager;
    private Button button;
    private bool isHandlingClick;

    private void Awake()
    {
        button = GetComponent<Button>();
        manager = FindAnyObjectByType<Manager>();

        // if (button != null)
        // {
        //     button.onClick.RemoveAllListeners();
        //     button.onClick.AddListener(ApplySelectedImage);
        // }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(ApplySelectedImage);
        }
    }

    public void ApplySelectedImage()
    {
        if (isHandlingClick)
        {
            return;
        }

        isHandlingClick = true;

        try
        {
            if (icon == null)
            {
                Debug.LogWarning("Cell icon Image is not assigned.");
                return;
            }

            if (manager == null)
            {
                manager = FindAnyObjectByType<Manager>();
            }

            if (manager == null)
            {
                Debug.LogWarning("Manager reference is not assigned.");
                return;
            }

            var grid = FindAnyObjectByType<cellManager>();

            if (grid == null)
            {
                Debug.LogWarning("CellManager is not assigned or found in the scene.");
                return;
            }

            if (manager.setHint)
            {
                grid.ToggleHint(this);
                return;
            }

            if (isLocked)
            {
                return;
            }

            grid.ApplyCellPlacement(this, manager.selectedType, manager.selectedImage);
        }
        finally
        {
            isHandlingClick = false;
        }
    }
}
