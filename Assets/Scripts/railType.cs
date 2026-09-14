using UnityEngine;
using UnityEngine.UI;

public class railType : MonoBehaviour
{
    public Image railTypeIcon;
    public string type = "";

    private Manager manager;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        manager = FindAnyObjectByType<Manager>();

        if (button != null)
        {
            button.onClick.AddListener(SetSelectedType);
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(SetSelectedType);
        }
    }

    public void SetSelectedType()
    {
        if (manager == null)
        {
            manager = FindAnyObjectByType<Manager>();
        }

        if (manager == null)
        {
            Debug.LogWarning("Manager is not assigned or found in the scene.");
            return;
        }

        manager.selectedType = type;

        if (railTypeIcon != null)
        {
            manager.selectedImage = railTypeIcon.sprite;
        }
        else
        {
            manager.selectedImage = null;
        }
    }
}
