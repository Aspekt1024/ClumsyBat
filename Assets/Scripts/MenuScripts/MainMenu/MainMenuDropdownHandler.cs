using UnityEngine;
using System.Collections;

public class MainMenuDropdownHandler : MonoBehaviour {
    
    private DropdownMenu dropdownMenu ;
    private MainMenu mainMenu;

    private void Start ()
    {
        mainMenu = FindObjectOfType<MainMenu>();
        dropdownMenu = FindObjectOfType<DropdownMenu>();
        dropdownMenu.Hide();
        dropdownMenu.StatsMenu.CreateStats();
    }

    public void OptionsPressed()
    {
        StartCoroutine(ShowOptions());
    }

    private IEnumerator ShowOptions()
    {
        yield return new WaitForSeconds(1f);
        dropdownMenu.ShowOptions();
    }

    public void OptionsBackPressed()
    {
        mainMenu.ReturnToMainScreen();
        StartCoroutine(HideDropdown(false));
    }

    private IEnumerator HideDropdown(bool bStatsMenu)
    {
        float WaitTime = dropdownMenu.RaiseMenu();
        yield return new WaitForSeconds(WaitTime);
        if (bStatsMenu)
        {
            dropdownMenu.StatsMenu.Hide();
        }
    }

    public void StatsPressed()
    {
        StartCoroutine(ShowStats());
    }

    private IEnumerator ShowStats()
    {
        yield return new WaitForSeconds(1f);
        dropdownMenu.ShowStats();
    }

    public void StatsBackPressed()
    {
        mainMenu.ReturnToMainScreen();
        StartCoroutine(HideDropdown(true));
    }
}
