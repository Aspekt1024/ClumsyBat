using UnityEngine;
using System.Collections;

public class DropdownStatsMenu : MonoBehaviour {

    private CanvasGroup StatsPanel = null;
    private CanvasGroup StatsMask = null;

	void Start ()
    {
        StatsMask = GameObject.Find("StatsMask").GetComponent<CanvasGroup>();
        StatsPanel = GameObject.Find("StatsPanel").GetComponent<CanvasGroup>();
	}

    void Update()
    {
        //StatsPanel.GetComponent<RectTransform>().position = GameObject.Find("ContentPanel").GetComponent<RectTransform>().position;
    }

    public void Show()
    {
        StatsMask.alpha = 1f;
        StatsMask.blocksRaycasts = true;
        StatsMask.interactable = true;
    }

    public void Hide()
    {
        StatsMask.alpha = 0f;
        StatsMask.blocksRaycasts = false;
        StatsMask.interactable = false;
    }

    public void CreateStats()
    {
        StatsPanel.GetComponent<StatsUI>().CreateStatText();
    }
}
