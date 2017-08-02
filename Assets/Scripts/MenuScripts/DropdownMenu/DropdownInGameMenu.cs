using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DropdownInGameMenu : MonoBehaviour {

    private DropdownMenu _menu;

    public GameObject StarsContainer;
    public GameObject ContinueButtonObject;

    public GameObject NextBtn;
    public GameObject ShareBtn;
    public GameObject OptionsBtn;
    public GameObject MainMenuBtn;
    public GameObject RestartBtn;
    public GameObject ResumeBtn;
    public GameObject LevelSelectBtn;

    private bool continueButtonPressed;

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

        stars = new MothStar[3];
        foreach(Transform tf in StarsContainer.transform)
        {
            if (tf.name.Contains("1")) stars[0] = tf.GetComponent<MothStar>();
            if (tf.name.Contains("2")) stars[1] = tf.GetComponent<MothStar>();
            if (tf.name.Contains("3")) stars[2] = tf.GetComponent<MothStar>();
        }
        StarsContainer.SetActive(false);
        ContinueButtonObject.SetActive(false);
    }

    public void LevelComplete(string levelText)
    {
        _menuHeader.Text.text = "LEVEL COMPLETE!";
        _subText.Text.text = levelText;
        StartCoroutine(LevelCompleteRoutine());
    }

    private IEnumerator LevelCompleteRoutine()
    {
        HideAllButtons();

        LoadStars();
        yield return _menu.StartCoroutine(_menu.PanelDropAnim(true));
        StartCoroutine(PopInObject(ContinueButtonObject.GetComponent<RectTransform>()));
        SetStarsActive();
        yield return StartCoroutine(ShowStars());

        continueButtonPressed = false;
        while (!continueButtonPressed)
        {
            yield return null;
        }
        StartCoroutine(PopOutObject(ContinueButtonObject.GetComponent<RectTransform>()));
        yield return StartCoroutine(PopOutStars());
        
        PositionMenuBtn(RestartBtn, GetButtonPosX(1, 4));
        PositionMenuBtn(MainMenuBtn, GetButtonPosX(2, 4));
        PositionMenuBtn(ShareBtn, GetButtonPosX(3, 4));
        PositionMenuBtn(NextBtn, GetButtonPosX(4, 4));
        
        StartCoroutine(PopInObject(NextBtn.GetComponent<RectTransform>()));
        StartCoroutine(PopInObject(ShareBtn.GetComponent<RectTransform>()));
        StartCoroutine(PopInObject(MainMenuBtn.GetComponent<RectTransform>()));
        StartCoroutine(PopInObject(RestartBtn.GetComponent<RectTransform>()));
    }

    public void ContinueButton()
    {
        continueButtonPressed = true;
    }

    private void HideAllButtons()
    {
        ContinueButtonObject.SetActive(false);
        NextBtn.SetActive(false);
        ShareBtn.SetActive(false);
        MainMenuBtn.SetActive(false);
        RestartBtn.SetActive(false);
        OptionsBtn.SetActive(false);
        ResumeBtn.SetActive(false);
        LevelSelectBtn.SetActive(false);
    }

    private void LoadStars()
    {
        int newStars = GameData.Instance.NewStars;
        int activeStars = GameData.Instance.TotalStars - newStars;
        int setStars = 0;
       
        for (int i = 0; i < 3; i++)
        {
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
            stars[i].gameObject.SetActive(false);
        }

        if (GameData.Instance.Level.ToString().Contains("Boss"))
        {
            stars[1].SetText("Under 2 Damage");
            stars[2].SetText("No Damage");
        }
    }

    private void SetStarsActive()
    {
        for (int i = 0; i < 3; i++)
        {
            stars[i].gameObject.SetActive(true);
        }
    }

    private IEnumerator PopOutStars()
    {
        StartCoroutine(PopOutObject(stars[0].GetComponent<RectTransform>()));
        yield return new WaitForSeconds(0.05f);
        StartCoroutine(PopOutObject(stars[1].GetComponent<RectTransform>()));
        yield return new WaitForSeconds(0.05f);
        yield return StartCoroutine(PopOutObject(stars[2].GetComponent<RectTransform>()));
    }

    private IEnumerator ShowStars()
    {
        StarsContainer.SetActive(true);
        ContinueButtonObject.SetActive(true);
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

    private IEnumerator PopInObject(RectTransform rt)
    {
        float timer = 0f;
        float duration = 0.2f;

        Vector3 originalScale = rt.localScale;
        rt.gameObject.SetActive(true);
        while (timer < duration)
        {
            timer += Time.deltaTime;
            rt.localScale = Vector3.Lerp(Vector3.one * 0.1f, originalScale * 1.1f, timer / duration);
            yield return null;
        }

        timer = 0f;
        duration = 0.08f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            rt.localScale = Vector3.Lerp(originalScale * 1.1f, originalScale, timer / duration);
            yield return null;
        }
        rt.localScale = originalScale;
    }

    private IEnumerator PopOutObject(RectTransform rt)
    {
        float timer = 0f;
        float duration = 0.08f;

        Vector3 originalScale = rt.localScale;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            rt.localScale = Vector3.Lerp(originalScale, originalScale * 1.1f, timer / duration);
            yield return null;
        }

        timer = 0f;
        duration = 0.2f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            rt.localScale = Vector3.Lerp(originalScale * 1.1f, Vector3.one * 0.1f, timer / duration);
            yield return null;
        }
        rt.gameObject.SetActive(false);
        rt.localScale = originalScale;
    }
}
