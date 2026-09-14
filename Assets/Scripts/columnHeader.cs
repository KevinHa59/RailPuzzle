using UnityEngine;
using UnityEngine.UI;

public class columnHeader : MonoBehaviour
{
    public Text[] columnTexts = new Text[8];
    public cellManager grid;

    private void Start()
    {
        FindGrid();
        UpdateColumnCounts();
    }

    private void FindGrid()
    {
        if (grid == null)
        {
            grid = FindAnyObjectByType<cellManager>();
        }
    }

    public void UpdateColumnCounts()
    {
        FindGrid();

        if (grid == null)
        {
            return;
        }

        for (int i = 0; i < columnTexts.Length; i++)
        {
            if (columnTexts[i] != null)
            {
                columnTexts[i].text = i < grid.columnCounts.Length ? grid.columnCounts[i].ToString() : "0";
            }
        }
    }
}
