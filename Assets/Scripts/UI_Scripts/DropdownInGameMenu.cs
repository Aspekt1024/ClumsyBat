using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DropdownInGameMenu : MonoBehaviour {

    private DropdownMenu Menu;

    public GameObject NextBtn = null;
    public GameObject ShareBtn = null;
    public GameObject OptionsBtn = null;
    public GameObject MainMenuBtn = null;
    public GameObject RestartBtn = null;
    public GameObject ResumeBtn = null;

    private struct TextType
    {
        public RectTransform RectTransform;
        public Text Text;
    }
    private TextType MenuHeader;
    private TextType SubText;

    void Awake()
    {
        Menu = FindObjectOfType<DropdownMenu>();
    }

    // Use this for initialization
    void Start ()
    {
        //GetMenuButtons();
        GetMenuObjects();
    }

    private void GetMenuObjects()
    {
        MenuHeader.RectTransform = GameObject.Find("MenuHeader").GetComponent<RectTransform>();
        MenuHeader.Text = MenuHeader.RectTransform.GetComponent<Text>();

        SubText.RectTransform = GameObject.Find("SubText").GetComponent<RectTransform>();
        SubText.Text = SubText.RectTransform.GetComponent<Text>();
    }

    public void LevelComplete(string LevelText)
    {
        NextBtn.SetActive(true);
        ShareBtn.SetActive(true);
        MainMenuBtn.SetActive(true);
        RestartBtn.SetActive(true);
        OptionsBtn.SetActive(false);
        ResumeBtn.SetActive(false);

        PositionMenuBtn(RestartBtn, GetButtonPosX(1, 4));
        PositionMenuBtn(MainMenuBtn, GetButtonPosX(2, 4));
        PositionMenuBtn(ShareBtn, GetButtonPosX(3, 4));
        PositionMenuBtn(NextBtn, GetButtonPosX(4, 4));

        MenuHeader.Text.text = "LEVEL COMPLETE!";
        SubText.Text.text = LevelText;
        Menu.StartCoroutine("PanelDropAnim", true);
    }

    public void PauseMenu()
    {
        NextBtn.SetActive(false);
        ShareBtn.SetActive(false);
        MainMenuBtn.SetActive(true);
        RestartBtn.SetActive(true);
        OptionsBtn.SetActive(true);
        ResumeBtn.SetActive(true);

        PositionMenuBtn(RestartBtn, GetButtonPosX(1, 4));
        PositionMenuBtn(MainMenuBtn, GetButtonPosX(2, 4));
        PositionMenuBtn(OptionsBtn, GetButtonPosX(3, 4));
        PositionMenuBtn(ResumeBtn, GetButtonPosX(4, 4));

        MenuHeader.Text.text = "GAME PAUSED";
        SubText.Text.text = "Clumsy will wait for you...";
        Menu.StartCoroutine("PanelDropAnim", true);
    }

    public void GameOver()
    {
        NextBtn.SetActive(false);
        ShareBtn.SetActive(false);
        MainMenuBtn.SetActive(true);
        RestartBtn.SetActive(true);
        OptionsBtn.SetActive(true);
        ResumeBtn.SetActive(false);

        PositionMenuBtn(RestartBtn, GetButtonPosX(1, 3));
        PositionMenuBtn(MainMenuBtn, GetButtonPosX(2, 3));
        PositionMenuBtn(OptionsBtn, GetButtonPosX(3, 3));
        
        MenuHeader.Text.text = "GAME OVER";
        SubText.Text.text = "Clumsy didn't make it...";
        Menu.StartCoroutine("PanelDropAnim", true);
    }

    private void PositionMenuBtn(GameObject Btn, float ButtonXPos)
    {
        RectTransform BtnRT = Btn.GetComponent<RectTransform>();
        BtnRT.position = new Vector3(ButtonXPos, BtnRT.position.y, BtnRT.position.z);
    }

    private float GetButtonPosX(int BtnNum, int NumButtons)
    {
        float ButtonPosX = 0;
        if (NumButtons == 3)
        {
            switch (BtnNum)
            {
                case 1:
                    ButtonPosX = -2;
                    break;
                case 2:
                    ButtonPosX = 0;
                    break;
                case 3:
                    ButtonPosX = 2;
                    break;
            }
        }
        else if (NumButtons == 4)
        {
            switch (BtnNum)
            {
                case 1:
                    ButtonPosX = -3f;
                    break;
                case 2:
                    ButtonPosX = -1f;
                    break;
                case 3:
                    ButtonPosX = 1f;
                    break;
                case 4:
                    ButtonPosX = 3f;
                    break;
            }
        }
        return ButtonPosX;
    }
}
