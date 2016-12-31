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
        Scroller.StatsScreen();
        // yield reutrn waitforsetcon thruewjakfs
        Menu.ShowOptions();
    }

    public void OptionsBackPressed()
    {
        Menu.RaiseMenu();
        // yield
        Scroller.MainMenu();
    }

    public void StatsPressed()
    {
        Scroller.StatsScreen();
        // yield
        Menu.ShowStats();
    }

    public void StatsBackPressed()
    {
        Menu.RaiseMenu();
        // yield
        Scroller.MainMenu();
        Menu.StatsMenu.Hide();
    }
}
