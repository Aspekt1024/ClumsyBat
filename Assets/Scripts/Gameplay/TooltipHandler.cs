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

    private Dictionary<EventID, string> TooltipDict = new Dictionary<EventID, string>();
    private Dictionary<EventID, DialogueID[]> DialogueSet = new Dictionary<EventID, DialogueID[]>();
    private Dictionary<DialogueID, string> DialogueDict = new Dictionary<DialogueID, string>();

    public enum EventID
    {
        FirstJump,
        FirstMoth,
        FirstDeath,
        StalLevel,
        FirstStalDrop
    }

    public enum DialogueID
    {
        StalDrop1,
        StalDrop2
    }

    void Awake()
    {
        ToolTipOverlay = (GameObject)Instantiate(Resources.Load("ToolTipOverlay"));
        ToolTipOverlay.GetComponent<Canvas>().worldCamera = FindObjectOfType<Camera>();
        ToolTipTextBox = GetToolTipTextBox();
        ToolTipOverlay.SetActive(false);

        SetToolTipText();
        SetDialogueIDs();
        SetDialogueText();
    }

    private void SetToolTipText()
    {
        TooltipDict.Add(EventID.FirstJump, "Tap anywhere to flap!");
        TooltipDict.Add(EventID.FirstMoth, "It's getting dark! Collect moths to fuel the lantern.");
        TooltipDict.Add(EventID.StalLevel, "This is as far as I've ever been. Be careful!");
    }

    private void SetDialogueIDs()
    {
        DialogueSet.Add(EventID.FirstStalDrop, new DialogueID[] { DialogueID.StalDrop1, DialogueID.StalDrop2 } );
    }

    private void SetDialogueText()
    {
        DialogueDict.Add(DialogueID.StalDrop1, "Did you see that?!");
        DialogueDict.Add(DialogueID.StalDrop2, "Oh right, you're a bat. Of course you didn't.");
    }

    void Start ()
    {
        // TODO set this up better...
        PlayerControl = FindObjectOfType<PlayerController>();
        Stats = PlayerControl.Level.Stats;
        InputManager = PlayerControl.GetInputManager();
	}
	
    public void ShowTooltip(EventID TooltipID)
    {
        if (!Stats.Settings.Tooltips) { return; }
        StartCoroutine("DisplayTooltip", TooltipID);
    }

    private IEnumerator DisplayTooltip(EventID ttID)
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
    
    public void EnableTooltip(EventID ttID)
    {
        ToolTipOverlay.SetActive(true);
        GameObject.Find("TapToResume").GetComponent<Text>().enabled = false;
        ToolTipTextBox.GetComponent<Text>().text = TooltipDict[ttID];
    }
    
    public void ShowDialogue(EventID EventID)
    {
        DialogueID[] Dialogue = DialogueSet[EventID];
        StartCoroutine("DisplayDialogue", Dialogue);
    }

    public IEnumerator DisplayDialogue(DialogueID[] Dialogue)
    {
        PlayerControl.WaitForTooltip(true);
        foreach (DialogueID Piece in Dialogue)
        {
            yield return StartCoroutine("EnableDialogue", Piece);
        }
        PlayerControl.WaitForTooltip(false);
        PlayerControl.TooltipResume();
        HideToolTip();
    }

    public IEnumerator EnableDialogue(DialogueID Piece)
    {
        ToolTipOverlay.SetActive(true);
        GameObject.Find("TapToResume").GetComponent<Text>().enabled = false;
        ToolTipTextBox.GetComponent<Text>().text = DialogueDict[Piece];
        
        const float TooltipPauseDuration = 0.7f;
        yield return new WaitForSeconds(TooltipPauseDuration);
        ShowTapToResume();
        
        while (!InputManager.TapRegistered() && !Input.GetKeyUp("w"))
        {
            yield return null;
        }
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
