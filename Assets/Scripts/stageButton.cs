using UnityEngine;
using UnityEngine.UI;

public class stageButton : MonoBehaviour
{
    public Text stageText;
    public int stageNumber;

    public void Setup(int stageNumber)
    {
        this.stageNumber = stageNumber;

        if (stageText != null)
        {
            stageText.text = stageNumber.ToString();
        }
    }
}
