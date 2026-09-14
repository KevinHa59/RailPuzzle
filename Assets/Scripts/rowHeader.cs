using UnityEngine;
using UnityEngine.UI;

public class rowHeader : MonoBehaviour
{
    public Text[] rowTexts = new Text[8];
    public cellManager grid;

    private void Start()
    {
        FindGrid();
        UpdateRowCounts();
    }

    private void FindGrid()
    {
        if (grid == null)
        {
            grid = FindAnyObjectByType<cellManager>();
        }
    }

    public void UpdateRowCounts()
    {
        FindGrid();

        if (grid == null)
        {
            return;
        }

        for (int i = 0; i < rowTexts.Length; i++)
        {
            if (rowTexts[i] != null)
            {
                rowTexts[i].text = i < grid.rowCounts.Length ? grid.rowCounts[i].ToString() : "0";
            }
        }
    }
}
