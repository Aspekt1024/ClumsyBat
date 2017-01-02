using UnityEngine;
using System.Collections;

public class MainMenuDropdownHandler : MonoBehaviour {
    
    private DropdownMenu Menu = null;
    private MenuScroller Scroller = null;

    void Start ()
    {
        Menu = FindObjectOfType<DropdownMenu>();
        Scroller = FindObjectOfType<MenuScroller>();
        Menu.Hide();
        Menu.StatsMenu.CreateStats();
    }

    public void OptionsPressed()
    {
        StartCoroutine("ShowOptions");
    }

    private IEnumerator ShowOptions()
    {
        float WaitTime = Scroller.StatsScreen();
        yield return new WaitForSeconds(WaitTime);
        Menu.ShowOptions();
    }

    public void OptionsBackPressed()
    {
        StartCoroutine("HideDropdown", false);
    }

    private IEnumerator HideDropdown(bool bStatsMenu)
    {
        float WaitTime = Menu.RaiseMenu();
        yield return new WaitForSeconds(WaitTime);
        Scroller.MainMenu();
        if (bStatsMenu)
        {
            Menu.StatsMenu.Hide();
        }
    }

    public void StatsPressed()
    {
        StartCoroutine("ShowStats");
    }

    private IEnumerator ShowStats()
    {
        float WaitTime = Scroller.StatsScreen();
        yield return new WaitForSeconds(WaitTime);
        Menu.ShowStats();
    }

    public void StatsBackPressed()
    {
        StartCoroutine("HideDropdown", true);
    }
}
