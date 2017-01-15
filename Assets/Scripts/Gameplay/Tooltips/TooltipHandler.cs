using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TooltipHandler : MonoBehaviour {

    private PlayerController PlayerControl;
    private SwipeManager InputManager;
    private TooltipController TooltipControl = null;
    
    private Dictionary<DialogueID, TooltipID[]> DialogueSet = new Dictionary<DialogueID, TooltipID[]>();
    private Dictionary<TooltipID, string> DialogueDict = new Dictionary<TooltipID, string>();

    bool bFirstDialogue = false;
    bool bLastDialogue = false;

    // DialogueID is used for the DialogueSet Dictionary
    // Referenced in the editor
    public enum DialogueID
    {
        FirstDeath,
        FirstJump,
        SecondJump,
        FirstMoth,
        AllOnYourOwn,
        StalLevel,
        NoMoreStals,
        ActuallyMoreStals,
        FirstStalDrop,
        ThatGotReal,
    }

    // TooltipID references the individual pieces of dialogue defined in the DialogueSet
    public enum TooltipID
    {
        FirstDeath,
        FirstJump,
        SecondJump,
        FirstMoth,
        AllOnYourOwn1,
        AllOnYourOwn2,
        StalLevel,
        StalDrop1,
        StalDrop2,
        StalDrop3,
        NoMoreStals,
        ActuallyMoreStals,
        ThatGotReal,
        FirstGoldMoth1,
        FirstGoldMoth2,
    }

    void Awake()
    {
        GameObject ToolTipOverlay = (GameObject)Instantiate(Resources.Load("ToolTipOverlay"));
        TooltipControl = ToolTipOverlay.GetComponent<TooltipController>();

        SetDialogueIDs();
        SetDialogueText();

        if (!Toolbox.Instance.TooltipCompletionPersist)
        {
            Toolbox.Instance.ResetTooltips();
        }
    }

    private void SetDialogueIDs()
    {
        DialogueSet.Add(DialogueID.FirstJump, new TooltipID[] { TooltipID.FirstJump });
        DialogueSet.Add(DialogueID.SecondJump, new TooltipID[] { TooltipID.SecondJump });
        DialogueSet.Add(DialogueID.FirstMoth, new TooltipID[] { TooltipID.FirstMoth } );
        DialogueSet.Add(DialogueID.AllOnYourOwn, new TooltipID[] { TooltipID.AllOnYourOwn1, TooltipID.AllOnYourOwn2 } );
        DialogueSet.Add(DialogueID.StalLevel, new TooltipID[] { TooltipID.StalLevel } );
        DialogueSet.Add(DialogueID.FirstStalDrop, new TooltipID[] { TooltipID.StalDrop1, TooltipID.StalDrop2, TooltipID.StalDrop3 } );
        DialogueSet.Add(DialogueID.NoMoreStals, new TooltipID[] { TooltipID.NoMoreStals });
        DialogueSet.Add(DialogueID.ActuallyMoreStals, new TooltipID[] { TooltipID.ActuallyMoreStals });
        DialogueSet.Add(DialogueID.ThatGotReal, new TooltipID[] { TooltipID.ThatGotReal });
        //DialogueSet.Add(DialogueID, new TooltipID[] {  });
    }

    private void SetDialogueText()
    {
        DialogueDict.Add(TooltipID.FirstDeath, "Don't worry, we can try again!");
        DialogueDict.Add(TooltipID.FirstJump, "Tap anywhere to flap!");
        DialogueDict.Add(TooltipID.SecondJump, "Keep tapping to keep Clumsy in the air.");
        DialogueDict.Add(TooltipID.FirstMoth, "It's getting dark! Collect moths to fuel the lantern.");
        DialogueDict.Add(TooltipID.AllOnYourOwn1, "You made it! I mean, of course you made it!");
        DialogueDict.Add(TooltipID.AllOnYourOwn2, "The path to the village is just through here.");
        DialogueDict.Add(TooltipID.StalLevel, "This is as far as I've ever been. Be careful!");
        DialogueDict.Add(TooltipID.StalDrop1, "Did you see that!?");
        DialogueDict.Add(TooltipID.StalDrop2, "Oh right, you're a bat. Of course you didn't.");
        DialogueDict.Add(TooltipID.StalDrop3, "Watch for falling objects!" );
        DialogueDict.Add(TooltipID.NoMoreStals, "Whew, we got through it! Wasn't that easy!?");
        DialogueDict.Add(TooltipID.ActuallyMoreStals, "... I was wrong. I think it's going to get a lot harder.");
        DialogueDict.Add(TooltipID.ThatGotReal, "Well that got real! Keep going, we're not far away.");
        DialogueDict.Add(TooltipID.FirstGoldMoth1, "Oh wow! A gold moth!");
        DialogueDict.Add(TooltipID.FirstGoldMoth2, "Rumor has it that these possess incredible power");
        //DialogueDict.Add(TooltipID, "");
    }

    void Start ()
    {
        // TODO set this up better...
        PlayerControl = FindObjectOfType<PlayerController>();
        InputManager = PlayerControl.GetInputManager();
	}
    
    public void ShowDialogue(DialogueID EventID)
    {
        if (!PlayerControl.ThePlayer.IsAlive() || Toolbox.Instance.TooltipCompleted(EventID)) { return; }
        Toolbox.Instance.SetTooltipComplete(EventID);
        TooltipID[] Dialogue = DialogueSet[EventID];
        StartCoroutine("SetupDialogue", Dialogue);
    }

    public IEnumerator SetupDialogue(DialogueID[] Dialogue)
    {
        PlayerControl.WaitForTooltip(true);
        bool bFirstDialogue = true;
        bLastDialogue = false;
        int NumItems = Dialogue.Length;
        foreach (TooltipID Speech in Dialogue)
        {
            NumItems--;
            if (NumItems == 0)
            {
                bLastDialogue = true;
            }
            if (bFirstDialogue)
            {
                yield return TooltipControl.StartCoroutine("OpenTooltip");
            }
            yield return StartCoroutine("ShowTooltip", Speech);
            bFirstDialogue = false;
        }
        PlayerControl.WaitForTooltip(false);
        PlayerControl.TooltipResume();
        TooltipControl.StartCoroutine("CloseTooltip");
    }

    private IEnumerator ShowTooltip(TooltipID Speech)
    {
        if (bFirstDialogue)
        {
            TooltipControl.SetText(DialogueDict[Speech]);
            TooltipControl.StartCoroutine("ShowText", true);
        }
        else
        {
            yield return TooltipControl.StartCoroutine("ShowText", false);
            TooltipControl.SetText(DialogueDict[Speech]);
            TooltipControl.StartCoroutine("ShowText", true);
        }
        
        const float TooltipPauseDuration = 0.7f;
        yield return new WaitForSeconds(TooltipPauseDuration);

        if (bLastDialogue)
        {
            TooltipControl.ShowTapToPlay();
        }
        else
        {
            TooltipControl.ShowTapToResume();
        }

        InputManager.ClearInput();
        while (!InputManager.TapRegistered() && !Input.GetKeyUp("w"))
        {
            yield return null;
        }
        TooltipControl.HideResumeImages();
    }
}
