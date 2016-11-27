using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour {
    
    public GameObject MenuButtons;
    private GameObject StatsOverlay;
    private GameObject RuntimeScripts;
    public StatsHandler Stats;

    private GameObject Backgrounds;
    private Transform Caves;
    private Transform MidBG;

    private GameObject MainPanel;
    private GameObject LevelSelectPanel;

    private const float TileSizeX = 19.2f;
    private const float AnimDuration = 0.5f;


    void Awake()
    {
        RuntimeScripts = new GameObject();
        RuntimeScripts.name = "Runtime Scripts";
        Stats = RuntimeScripts.AddComponent<StatsHandler>();

        GetBackgrounds();
    }

    void Start()
    {
        //GetComponent<AudioSource>().Play();
        SetupLevelSelect();
        if (Toolbox.Instance.MenuScreen == Toolbox.MenuSelector.LevelSelect)
        {
            Caves.position = new Vector3(-TileSizeX, 0f, 0f);
            MidBG.position = new Vector3(-TileSizeX / 3, 0f, 0f);
            MainPanel.transform.position = new Vector3(-TileSizeX, 0f, 0f);
            LevelSelectPanel.transform.position = new Vector3(0f, 0f, 0f);
        }
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

    private void GetBackgrounds()
    {
        Backgrounds = GameObject.Find("Background");
        foreach (Transform ChildObj in Backgrounds.transform)
        {
            if (ChildObj.name == "CavePieces")
            {
                Caves = ChildObj;
            }
            if (ChildObj.name == "MidBackground")
            {
                MidBG = ChildObj;
            }
        }
        MainPanel = GameObject.Find("MainScreen");
        LevelSelectPanel = GameObject.Find("LevelSelectScreen");
    }

    void Update()
    {
        Stats.IdleTime += Time.deltaTime;
    }

    public void PlayButtonClicked()
    {
        Stats.SaveStats();
        StartCoroutine("MoveToLevelSelect");
    }

    public void ReturnToMainScreen()
    {
        StartCoroutine("MoveToMainMenu");
    }

    private IEnumerator MoveToMainMenu()
    {
        float AnimTime = 0f;
        float StartX = Caves.position.x;
        const float EndX = 0f;
        float XPos = StartX;

        while (AnimTime < AnimDuration)
        {
            XPos = StartX - (StartX - EndX) * (AnimTime / AnimDuration);
            Caves.position = new Vector3(XPos, 0f, 0f);
            MidBG.position = new Vector3(XPos/3, 0f, 0f);
            MainPanel.transform.position = new Vector3(XPos, 0f, 0f);
            LevelSelectPanel.transform.position = new Vector3(XPos + TileSizeX, 0f, 0f);
            AnimTime += Time.deltaTime;
            yield return null;
        }
        Caves.position = new Vector3(EndX, 0f, 0f);
        MidBG.position = new Vector3(EndX, 0f, 0f);
        MainPanel.transform.position = new Vector3(EndX, 0f, 0f);
        LevelSelectPanel.transform.position = new Vector3(EndX + TileSizeX, 0f, 0f);
    }

    private IEnumerator MoveToLevelSelect()
    {
        float AnimTime = 0f;
        float XPos = 0f;
        while (AnimTime < AnimDuration)
        {
            XPos = -TileSizeX * (AnimTime / AnimDuration);
            Caves.position = new Vector3(XPos, 0f, 0f);
            MidBG.position = new Vector3(XPos/3, 0f, 0f);
            MainPanel.transform.position = new Vector3(XPos, 0f, 0f);
            LevelSelectPanel.transform.position = new Vector3(XPos + TileSizeX, 0f, 0f);

            AnimTime += Time.deltaTime;
            yield return null;
        }
        Caves.position = new Vector3(-TileSizeX, 0f, 0f);
        MidBG.position = new Vector3(-TileSizeX / 3, 0f, 0f);
        MainPanel.transform.position = new Vector3(-TileSizeX, 0f, 0f);
        LevelSelectPanel.transform.position = new Vector3(0f, 0f, 0f);
    }

    public void QuitButtonClicked()
    {
        Stats.SaveStats();
        Application.Quit();
    }

    public void ClearDataButtonClicked()
    {
        // TODO setup menu to ask "Are you sure?"
        Stats.ClearPlayerPrefs();
    }

    public void StatsButtonClicked()
    {
        MenuButtons.SetActive(false);
        StatsOverlay = (GameObject)Instantiate(Resources.Load("StatsOverlay"));
        StatsOverlay.GetComponent<Canvas>().worldCamera = FindObjectOfType<Camera>();
        StatsOverlay.GetComponent<StatsScript>().CreateStatText(this);
    }

    public void StatsOKButtonClicked()
    {
        MenuButtons.SetActive(true);
        Destroy(StatsOverlay);
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
