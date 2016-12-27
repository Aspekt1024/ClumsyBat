using UnityEngine;
using System.Collections;

public class NavButtonHandler : MonoBehaviour
{

    private GameObject NavButtons;

    void Awake()
    {
        NavButtons = GameObject.Find("NavButtons");
    }


    void Update()
    {

    }

    public void SetupNavButtons(MenuScroller.MenuStates MenuState)
    {
        if (!NavButtons) { return; }
        switch (MenuState)
        {
            case MenuScroller.MenuStates.LevelSelect:
                NavButtons.SetActive(true);
                break;
            case MenuScroller.MenuStates.MainMenu:
                NavButtons.SetActive(false);
                break;
            case MenuScroller.MenuStates.StatsScreen:
                NavButtons.SetActive(true);
                break;
            default:

                break;
        }
    }
}
