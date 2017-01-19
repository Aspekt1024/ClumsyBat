using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TooltipHandler : MonoBehaviour {

    private PlayerController _playerControl;
    private SwipeManager _inputManager;
    private TooltipController _tooltipControl;
    
    private readonly Dictionary<DialogueId, TooltipId[]> _dialogueSet = new Dictionary<DialogueId, TooltipId[]>();
    private readonly Dictionary<TooltipId, string> _dialogueDict = new Dictionary<TooltipId, string>();

    private bool _bFirstDialogue;
    private bool _bLastDialogue;

    // DialogueID is used for the DialogueSet Dictionary
    // Referenced in the editor
    public enum DialogueId
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
    public enum TooltipId
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
        GameObject toolTipOverlay = (GameObject)Instantiate(Resources.Load("ToolTipOverlay"));
        _tooltipControl = toolTipOverlay.GetComponent<TooltipController>();

        SetDialogueIDs();
        SetDialogueText();

        if (!Toolbox.Instance.TooltipCompletionPersist)
        {
            Toolbox.Instance.ResetTooltips();
        }
    }

    private void SetDialogueIDs()
    {
        _dialogueSet.Add(DialogueId.FirstJump, new[] { TooltipId.FirstJump });
        _dialogueSet.Add(DialogueId.SecondJump, new[] { TooltipId.SecondJump });
        _dialogueSet.Add(DialogueId.FirstMoth, new[] { TooltipId.FirstMoth } );
        _dialogueSet.Add(DialogueId.AllOnYourOwn, new[] { TooltipId.AllOnYourOwn1, TooltipId.AllOnYourOwn2 } );
        _dialogueSet.Add(DialogueId.StalLevel, new[] { TooltipId.StalLevel } );
        _dialogueSet.Add(DialogueId.FirstStalDrop, new[] { TooltipId.StalDrop1, TooltipId.StalDrop2, TooltipId.StalDrop3 } );
        _dialogueSet.Add(DialogueId.NoMoreStals, new[] { TooltipId.NoMoreStals });
        _dialogueSet.Add(DialogueId.ActuallyMoreStals, new[] { TooltipId.ActuallyMoreStals });
        _dialogueSet.Add(DialogueId.ThatGotReal, new[] { TooltipId.ThatGotReal });
        //DialogueSet.Add(DialogueID, new TooltipID[] {  });
    }

    private void SetDialogueText()
    {
        _dialogueDict.Add(TooltipId.FirstDeath, "Don't worry, we can try again!");
        _dialogueDict.Add(TooltipId.FirstJump, "Tap anywhere to flap!");
        _dialogueDict.Add(TooltipId.SecondJump, "Keep tapping to keep Clumsy in the air.");
        _dialogueDict.Add(TooltipId.FirstMoth, "It's getting dark! Collect moths to fuel the lantern.");
        _dialogueDict.Add(TooltipId.AllOnYourOwn1, "You made it! I mean, of course you made it!");
        _dialogueDict.Add(TooltipId.AllOnYourOwn2, "The path to the village is just through here.");
        _dialogueDict.Add(TooltipId.StalLevel, "This is as far as I've ever been. Be careful!");
        _dialogueDict.Add(TooltipId.StalDrop1, "Did you see that!?");
        _dialogueDict.Add(TooltipId.StalDrop2, "Oh right, you're a bat. Of course you didn't.");
        _dialogueDict.Add(TooltipId.StalDrop3, "Watch for falling objects!" );
        _dialogueDict.Add(TooltipId.NoMoreStals, "Whew, we got through it! Wasn't that easy!?");
        _dialogueDict.Add(TooltipId.ActuallyMoreStals, "... I was wrong. I think it's going to get a lot harder.");
        _dialogueDict.Add(TooltipId.ThatGotReal, "Well that got real! Keep going, we're not far away.");
        _dialogueDict.Add(TooltipId.FirstGoldMoth1, "Oh wow! A gold moth!");
        _dialogueDict.Add(TooltipId.FirstGoldMoth2, "Rumor has it that these possess incredible power");
        //DialogueDict.Add(TooltipID, "");
    }

    void Start ()
    {
        // TODO set this up better...
        _playerControl = FindObjectOfType<PlayerController>();
        _inputManager = _playerControl.GetInputManager();
	}
    
    public void ShowDialogue(DialogueId eventId)
    {
        if (!_playerControl.ThePlayer.IsAlive() || Toolbox.Instance.TooltipCompleted(eventId)) { return; }
        Toolbox.Instance.SetTooltipComplete(eventId);
        TooltipId[] dialogue = _dialogueSet[eventId];
        StartCoroutine("SetupDialogue", dialogue);
    }

    public IEnumerator SetupDialogue(DialogueId[] dialogue)
    {
        _playerControl.WaitForTooltip(true);
        _bFirstDialogue = true;
        _bLastDialogue = false;
        int numItems = dialogue.Length;
        foreach (TooltipId speech in dialogue)
        {
            numItems--;
            if (numItems == 0)
            {
                _bLastDialogue = true;
            }
            if (_bFirstDialogue)
            {
                yield return _tooltipControl.StartCoroutine("OpenTooltip");
            }
            yield return StartCoroutine("ShowTooltip", speech);
            _bFirstDialogue = false;
        }
        _playerControl.WaitForTooltip(false);
        _playerControl.TooltipResume();
        _tooltipControl.StartCoroutine("CloseTooltip");
    }

    private IEnumerator ShowTooltip(TooltipId speech)
    {
        if (_bFirstDialogue)
        {
            _tooltipControl.SetText(_dialogueDict[speech]);
            _tooltipControl.StartCoroutine("ShowText", true);
        }
        else
        {
            yield return _tooltipControl.StartCoroutine("ShowText", false);
            _tooltipControl.SetText(_dialogueDict[speech]);
            _tooltipControl.StartCoroutine("ShowText", true);
        }
        
        const float tooltipPauseDuration = 0.7f;
        yield return new WaitForSeconds(tooltipPauseDuration);

        if (_bLastDialogue)
        {
            _tooltipControl.ShowTapToPlay();
        }
        else
        {
            _tooltipControl.ShowTapToResume();
        }

        _inputManager.ClearInput();
        while (!_inputManager.TapRegistered() && !Input.GetKeyUp("w"))
        {
            yield return null;
        }
        _tooltipControl.HideResumeImages();
    }
}
