using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NavButtonHandler : MonoBehaviour
{
    private RectTransform backButton;
    
    void Awake()
    {
        foreach (RectTransform RT in GetComponent<RectTransform>())
        {
            if (RT.name == "BackButton")
            {
                backButton = RT;
                backButton.gameObject.SetActive(false);
            }
        }
    }

    public void DisableNavButtons()
    {
        DisableBackButton();
    }
    
    public void SetNavButtons(CamPositioner.Positions camPosition)
    {
        switch (camPosition)
        {
            case CamPositioner.Positions.LevelSelect:
                EnableBackButton();
                break;
            case CamPositioner.Positions.Main:
                //EnableBackButton();
                break;
            case CamPositioner.Positions.DropdownArea:
                //EnableBackButton();
                break;
            default:
                break;
        }
    }

    private void EnableBackButton()
    {
        UIObjectAnimator.Instance.PopInObject(backButton);
    }

    private void DisableBackButton()
    {
        UIObjectAnimator.Instance.PopOutObject(backButton);
    }


}
