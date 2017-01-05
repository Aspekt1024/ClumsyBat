using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NavButtonHandler : MonoBehaviour
{

    private GameObject NavButtons;
    private RectTransform BackButton;

    void Awake()
    {
        NavButtons = GameObject.Find("NavButtons");
        foreach (RectTransform RT in NavButtons.GetComponent<RectTransform>())
        {
            switch(RT.name)
            {
                case "BackButton":
                    BackButton = RT;
                    break;
            }
        }
    }
    
    public void SetupNavButtons(MenuScroller.MenuStates MenuState)
    {
        if (!NavButtons) { return; }
        switch (MenuState)
        {
            case MenuScroller.MenuStates.LevelSelect:
                NavButtons.SetActive(true);
                StartCoroutine("AnimateBackArrow");
                break;
            case MenuScroller.MenuStates.MainMenu:
                NavButtons.SetActive(false);
                break;
            case MenuScroller.MenuStates.StatsScreen:
                NavButtons.SetActive(false);
                break;
            default:
                break;
        }
    }

    private IEnumerator AnimateBackArrow()
    {
        float OverlayScale = GameObject.Find("ScrollOverlay").GetComponent<RectTransform>().localScale.x;
        float StartPos = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0f)).x;
        float EndPos = Camera.main.ScreenToWorldPoint(Vector2.zero).x + BackButton.rect.width * OverlayScale;
        const float AnimDuration = 0.4f;

        BackButton.GetComponent<Image>().enabled = false;
        yield return new WaitForSeconds(1.5f);
        BackButton.position = new Vector3(StartPos, BackButton.position.y, BackButton.position.z);
        BackButton.GetComponent<Image>().enabled = true;
        yield return null;

        float AnimTimer = 0f;
        while (AnimTimer < AnimDuration)
        {
            AnimTimer += Time.deltaTime;
            float Ratio = Mathf.Clamp(AnimTimer / AnimDuration, 0f, 1f);
            float XPos = StartPos - (StartPos - EndPos) * Ratio;
            BackButton.position = new Vector3(XPos, BackButton.position.y, BackButton.position.z);
            yield return null;
        }
        BackButton.position = new Vector3(EndPos, BackButton.position.y, BackButton.position.z);
    }
}
