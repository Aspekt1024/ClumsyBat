using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameMenuOverlay : MonoBehaviour {

    private StatsHandler Stats;

    private RectTransform MenuPanel = null;
    private TextType MenuHeader;
    private TextType SubText;

    private struct TextType
    {
        public RectTransform RectTransform;
        public Text Text;
    }

    enum MenuState
    {
        Pause,
        GameOver,
        Win
    }

	// Use this for initialization
	void Start ()
    {
        Stats = FindObjectOfType<StatsHandler>();
        GetMenuObjects();
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void MenuButtonPressed()
    {
        Stats.SaveStats();
        SceneManager.LoadScene("Play");
    }

    public void RestartButtonPressed()
    {
        Stats.SaveStats();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void GetMenuObjects()
    {
        if (!gameObject.activeSelf) { gameObject.SetActive(true); }

        MenuPanel = GameObject.Find("GameMenuPanel").GetComponent<RectTransform>();
        MenuHeader.RectTransform = GameObject.Find("MenuHeader").GetComponent<RectTransform>();
        MenuHeader.Text = MenuHeader.RectTransform.GetComponent<Text>();
        SubText.RectTransform = GameObject.Find("SubText").GetComponent<RectTransform>();
        SubText.Text = SubText.RectTransform.GetComponent<Text>();
        //TODO RestartButton
        //TODO MainMenuButton
        //TODO Resume button
    }

    public void GameOver()
    {
        gameObject.SetActive(true);
        if (MenuHeader.RectTransform == null)
        {
            GetMenuObjects();
        }
        MenuHeader.Text.text = "GAME OVER";
        SubText.Text.text = "Clumsy didn't make it...";
        StartCoroutine("PanelDropAnim");
    }

    public void PauseGame()
    {
        gameObject.SetActive(true);
        MenuHeader.Text.text = "GAME PAUSED";
        SubText.Text.text = "Clumsy will wait for you...";
        StartCoroutine("PanelDropAnim");
    }

    public void WinGame()
    {
        gameObject.SetActive(true);
        MenuHeader.Text.text = "LEVEL COMPLETE!";
        SubText.Text.text = "Clumsy made it!";
        StartCoroutine("PanelDropAnim");
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator PanelDropAnim()
    {
        const float AnimDuration = 0.28f;
        float AnimTimer = 0f;
        MenuPanel.position = new Vector3(MenuPanel.position.x, 10f, MenuPanel.position.z);
        while (AnimTimer < AnimDuration)
        {
            AnimTimer += Time.deltaTime;
            MenuPanel.position = new Vector3(MenuPanel.position.x, (10f - (AnimTimer / AnimDuration) * 10f), MenuPanel.position.z);
            GameObject.Find("BackPanel").GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.65f * (AnimTimer / AnimDuration));
            yield return null;
        }

        StartCoroutine("Bounce", 1);
        yield return new WaitForSeconds(0.18f);
        StartCoroutine("Bounce", -0.5);

        MenuPanel.position = new Vector3(MenuPanel.position.x, 0f, MenuPanel.position.z);
    }

    private IEnumerator Bounce(float YDist)
    {
        float AnimTimer = 0;
        const float AnimDuration = 0.18f;

        float StartY = MenuPanel.position.y;
        float EndY = MenuPanel.position.y + YDist;

        while (AnimTimer < AnimDuration)
        {
            AnimTimer += Time.deltaTime;
            float AnimRatio = -Mathf.Sin(Mathf.PI * AnimTimer / AnimDuration);
            float YPos = StartY - (AnimRatio) * (StartY - EndY);
            MenuPanel.position = new Vector3(MenuPanel.position.x, YPos, MenuPanel.position.z);
            yield return null;
        }
    }
}
