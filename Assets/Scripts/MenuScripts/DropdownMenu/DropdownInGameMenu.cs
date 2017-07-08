using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DropdownInGameMenu : MonoBehaviour {

    private DropdownMenu _menu;

    public GameObject NextBtn;
    public GameObject ShareBtn;
    public GameObject OptionsBtn;
    public GameObject MainMenuBtn;
    public GameObject RestartBtn;
    public GameObject ResumeBtn;
    public GameObject LevelSelectBtn;

    private struct TextType
    {
        public RectTransform RectTransform;
        public Text Text;
    }
    private TextType _menuHeader;
    private TextType _subText;

    private MothStar[] stars;
    private int firstInactiveStarIndex;
    
    private void Start ()
    {
        _menu = FindObjectOfType<DropdownMenu>();
        GetMenuObjects();
    }

    private void GetMenuObjects()
    {
        _menuHeader.RectTransform = GameObject.Find("MenuHeader").GetComponent<RectTransform>();
        _menuHeader.Text = _menuHeader.RectTransform.GetComponent<Text>();

        _subText.RectTransform = GameObject.Find("SubText").GetComponent<RectTransform>();
        _subText.Text = _subText.RectTransform.GetComponent<Text>();
    }

    public void LevelComplete(string levelText)
    {
        NextBtn.SetActive(true);
        ShareBtn.SetActive(true);
        MainMenuBtn.SetActive(true);
        RestartBtn.SetActive(true);
        OptionsBtn.SetActive(false);
        ResumeBtn.SetActive(false);
        LevelSelectBtn.SetActive(false);

        PositionMenuBtn(RestartBtn, GetButtonPosX(1, 4));
        PositionMenuBtn(MainMenuBtn, GetButtonPosX(2, 4));
        PositionMenuBtn(ShareBtn, GetButtonPosX(3, 4));
        PositionMenuBtn(NextBtn, GetButtonPosX(4, 4));

        _menuHeader.Text.text = "LEVEL COMPLETE!";
        _subText.Text.text = levelText;
        StartCoroutine(LevelCompleteRoutine());
    }

    private IEnumerator LevelCompleteRoutine()
    {
        AddStars();
        yield return _menu.StartCoroutine(_menu.PanelDropAnim(true));
        StartCoroutine(ShowStars());
    }

    private void AddStars()
    {
        GameObject mothStar = Resources.Load<GameObject>("UIElements/MothStar");

        int newStars = GameData.Instance.NewStars;
        int activeStars = GameData.Instance.TotalStars - newStars;
        int setStars = 0;

        stars = new MothStar[3];
        
        for (int i = 0; i < 3; i++)
        {
            stars[i] = Instantiate(mothStar).GetComponent<MothStar>();
            stars[i].transform.SetParent(transform);
            stars[i].transform.position = _subText.RectTransform.transform.position + Vector3.right * (2 + i);
            if (setStars < activeStars)
            {
                stars[i].SetActive();
                setStars++;
                firstInactiveStarIndex = i + 1;
            }
            else
            {
                stars[i].SetInactive();
            }
        }
    }

    private IEnumerator ShowStars()
    {
        yield return new WaitForSeconds(0.5f);
        int newStars = GameData.Instance.NewStars;
        while (newStars > 0)
        {
            // TODO play sound
            yield return stars[firstInactiveStarIndex].StartCoroutine(stars[firstInactiveStarIndex].AnimateToActive());
            newStars--;
            firstInactiveStarIndex++;
        }
    }

    public void PauseMenu()
    {
        NextBtn.SetActive(false);
        ShareBtn.SetActive(false);
        MainMenuBtn.SetActive(true);
        RestartBtn.SetActive(true);
        OptionsBtn.SetActive(true);
        ResumeBtn.SetActive(true);
        LevelSelectBtn.SetActive(false);

        PositionMenuBtn(RestartBtn, GetButtonPosX(1, 4));
        PositionMenuBtn(MainMenuBtn, GetButtonPosX(2, 4));
        PositionMenuBtn(OptionsBtn, GetButtonPosX(3, 4));
        PositionMenuBtn(ResumeBtn, GetButtonPosX(4, 4));

        _menuHeader.Text.text = "GAME PAUSED";
        _subText.Text.text = "Clumsy will wait for you...";
        _menu.StartCoroutine("PanelDropAnim", true);
    }

    public void GameOver()
    {
        NextBtn.SetActive(false);
        ShareBtn.SetActive(false);
        MainMenuBtn.SetActive(true);
        RestartBtn.SetActive(true);
        OptionsBtn.SetActive(false);
        ResumeBtn.SetActive(false);
        LevelSelectBtn.SetActive(true);

        PositionMenuBtn(RestartBtn, GetButtonPosX(1, 3));
        PositionMenuBtn(MainMenuBtn, GetButtonPosX(2, 3));
        PositionMenuBtn(LevelSelectBtn, GetButtonPosX(3, 3));
        
        _menuHeader.Text.text = "GAME OVER";
        _subText.Text.text = "Clumsy didn't make it...";
        _menu.StartCoroutine("PanelDropAnim", true);
    }

    private void PositionMenuBtn(GameObject btn, float buttonXPos)
    {
        RectTransform btnRt = btn.GetComponent<RectTransform>();
        btnRt.position = new Vector3(buttonXPos, btnRt.position.y, btnRt.position.z);
    }

    private float GetButtonPosX(int btnNum, int numButtons)
    {
        float buttonPosX = 0;
        if (numButtons == 3)
        {
            switch (btnNum)
            {
                case 1:
                    buttonPosX = -2;
                    break;
                case 2:
                    buttonPosX = 0;
                    break;
                case 3:
                    buttonPosX = 2;
                    break;
            }
        }
        else if (numButtons == 4)
        {
            switch (btnNum)
            {
                case 1:
                    buttonPosX = -3f;
                    break;
                case 2:
                    buttonPosX = -1f;
                    break;
                case 3:
                    buttonPosX = 1f;
                    break;
                case 4:
                    buttonPosX = 3f;
                    break;
            }
        }
        
        return buttonPosX + _menuHeader.RectTransform.position.x;
    }
}
