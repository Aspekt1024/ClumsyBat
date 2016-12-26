using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour {
    
    public GameObject MenuButtons;
    public GameObject NavButtons;
    private GameObject RuntimeScripts;
    public StatsHandler Stats;

    private MenuScroller Scroller;

    void Awake()
    {
        RuntimeScripts = new GameObject("Runtime Scripts");
        Stats = RuntimeScripts.AddComponent<StatsHandler>();
        Scroller = RuntimeScripts.AddComponent<MenuScroller>();
    }

    void Start()
    {
        //GetComponent<AudioSource>().Play();
        SetupLevelSelect();
        SetupStatsScreen();
        NavButtons.SetActive(false);
    }

    private void SetupLevelSelect()
    {
        RectTransform LvlButtons = GameObject.Find("LevelButtons").GetComponent<RectTransform>();
        foreach (RectTransform LvlButton in LvlButtons)
        {
            int Level = int.Parse(LvlButton.name.Substring(2, LvlButton.name.Length - 2));
            if (Stats.CompletionData.IsUnlocked(Level) || Level == 1)
            {
                LvlButton.GetComponent<Image>().enabled = true;
                LvlButton.GetComponent<Button>().enabled = true;
            }
            else
            {
                LvlButton.GetComponent<Image>().enabled = false;
                LvlButton.GetComponent<Button>().enabled = false;
            }
        }
    }

    private void SetupStatsScreen()
    {
        GameObject.Find("StatsPanel").GetComponent<StatsUI>().CreateStatText();
    }

    void Update()
    {
        Stats.IdleTime += Time.deltaTime;
    }

    public void PlayButtonClicked()
    {
        Stats.SaveStats();
        Scroller.LevelSelect();
        NavButtons.SetActive(true);
    }

    public void ReturnToMainScreen()
    {
        Scroller.MainMenu();
        NavButtons.SetActive(false);
    }

    public void QuitButtonClicked()
    {
        Stats.SaveStats();
        Application.Quit();
    }

    public void StatsButtonClicked()
    {
        Scroller.StatsScreen();
        NavButtons.SetActive(true);
    }

    public void ClearDataButtonClicked()
    {
        // TODO setup menu to ask "Are you sure?"
        Stats.ClearPlayerPrefs();
    }

    // Level Selects
    private void LoadLevel(int LevelNum)
    {
        Toolbox.Instance.Level = LevelNum;
        SceneManager.LoadScene("Levels");
    }

    public void LvEndlessButtonClicked() { LoadLevel(-1); }
    public void Lv1BtnClick() { LoadLevel(1); }
    public void Lv2BtnClick() { LoadLevel(2); }
    public void Lv3BtnClick() { LoadLevel(3); }
    public void Lv4BtnClick() { LoadLevel(4); }
    public void Lv5BtnClick() { LoadLevel(5); }
    public void Lv6BtnClick() { LoadLevel(6); }
    public void Lv7BtnClick() { LoadLevel(7); }
    public void Lv8BtnClick() { LoadLevel(8); }
    public void Lv9BtnClick() { LoadLevel(9); }
}
