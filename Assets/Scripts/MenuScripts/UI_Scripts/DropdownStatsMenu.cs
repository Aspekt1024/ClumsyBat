using UnityEngine;
using System.Collections;

public class DropdownStatsMenu : MonoBehaviour {

    private StatsUI StatsScript = null;
    private CanvasGroup StatsMask = null;

    void Awake()
    {
        StatsScript = gameObject.AddComponent<StatsUI>();
    }

	void Start ()
    {
        StatsMask = GameObject.Find("StatsMask").GetComponent<CanvasGroup>();
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
        StatsScript.CreateStatText();
    }
}
