using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class TooltipHandler : MonoBehaviour {

    private PlayerController PlayerControl;
    private StatsHandler Stats;
    private SwipeManager InputManager;

    private GameObject ToolTipOverlay = null;
    private RectTransform ToolTipTextBox = null;
    
    private Dictionary<TriggerHandler.EventID, string> TooltipDict = new Dictionary<TriggerHandler.EventID, string>();

    void Awake()
    {
        ToolTipOverlay = (GameObject)Instantiate(Resources.Load("ToolTipOverlay"));
        ToolTipOverlay.GetComponent<Canvas>().worldCamera = FindObjectOfType<Camera>();
        ToolTipTextBox = GetToolTipTextBox();
        ToolTipOverlay.SetActive(false);

        SetToolTipText();
    }

    private void SetToolTipText()
    {
        TooltipDict.Add(TriggerHandler.EventID.FirstJump, "Tap anywhere to flap!");
        TooltipDict.Add(TriggerHandler.EventID.FirstMoth, "It's getting dark! Collect moths to fuel the lantern.");
    }

    void Start ()
    {
        // TODO set this up better...
        PlayerControl = FindObjectOfType<PlayerController>();
        Stats = FindObjectOfType<StatsHandler>();
        InputManager = PlayerControl.GetInputManager();
	}
	
    public void ShowTooltip(TriggerHandler.EventID TooltipID)
    {
        if (!Stats.Settings.Tooltips) { return; }
        StartCoroutine("DisplayTooltip", TooltipID);
        
    }

    private IEnumerator DisplayTooltip(TriggerHandler.EventID ttID)
    {
        PlayerControl.WaitForTooltip(true);
        EnableTooltip(ttID);

        const float TooltipPauseDuration = 0.7f;
        yield return new WaitForSeconds(TooltipPauseDuration);

        PlayerControl.WaitForTooltip(false);
        ShowTapToResume();

        while (!InputManager.TapRegistered() && !Input.GetKeyUp("w"))
        {
            yield return null;
        }

        PlayerControl.TooltipResume();
        HideToolTip();
    }


    public void EnableTooltip(TriggerHandler.EventID ttID)
    {
        ToolTipOverlay.SetActive(true);
        GameObject.Find("TapToResume").GetComponent<Text>().enabled = false;
        ToolTipTextBox.GetComponent<Text>().text = TooltipDict[ttID];
    }
    
    public void ShowTapToResume()
    {
        GameObject.Find("TapToResume").GetComponent<Text>().enabled = true;
    }

    public void HideToolTip()
    {
        ToolTipOverlay.SetActive(false);
    }
    
    private RectTransform GetToolTipTextBox()
    {
        foreach (RectTransform Panel in ToolTipOverlay.GetComponent<RectTransform>())
        {
            if (Panel.name == "ToolTipPanel")
            {
                foreach (RectTransform RT in Panel.GetComponent<RectTransform>())
                {
                    if (RT.name == "ToolTipTextBox")
                    {
                        return RT;
                    }
                }
            }
        }
        return null;
    }
}
